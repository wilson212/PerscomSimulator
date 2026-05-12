using System;
using System.Collections.Generic;
using System.Linq;
using Google.GenAI;
using Google.GenAI.Types;
using System.Text.Json;
using System.Threading.Tasks;
using Telerik.WinControls.UI;
using Telerik.WinControls.UI.ConversationalUI;
using Type = Google.GenAI.Types.Type;

namespace Perscom.AI;

/// <summary>
/// Represents an AI-powered assistant that facilitates the design of military organization systems
/// for a MILSIM Airsoft league simulator. The Advisor provides structured JSON outputs for
/// entity creation based on user input and maintains a conversation history to aid the design process.
/// </summary>
public class Advisor
{
    /// <summary>
    /// The Gemini API client used to interact with the Gemini API.
    /// </summary>
    private Client GemeniClient { get; set; }

    /// <summary>
    /// Represents the predefined context string used as system instructions for the AI-powered assistant.
    /// This context guides the assistant to act as a military organization design assistant for a MILSIM Airsoft league simulator.
    /// It specifies the assistant's role, tasks, and behavior, such as querying database state before making suggestions
    /// and providing structured JSON outputs for confirmed designs.
    /// </summary>
    private const string Context = @"You are a military organization design assistant for a MILSIM Airsoft league simulator.
    You help users create rank systems, unit blueprints, and position blueprints.
    Always use the provided functions to query existing database state before making suggestions.
    When the user confirms a design, output structured JSON for entity creation.

    CRITICAL CONTEXT:
    - When querying ranks, rank classifications, and UnitBlueprints, Ensure the user has provided the Faction ID. A value of 0 indicates no faction is selected.

    CRITICAL RULES:
    - Never assume missing information. If a user asks to build a position but does not specify the parent unit, YOU MUST ask the user which unit it belongs to before calling any creation tools.
    - If the user uses a vague unit name, use your tools to query the database and confirm the exact unit name with the user.
    - Only use plain text when replying to the user. Do not use HTML or markdown.";

    /// <summary>
    /// The Gemini model ID used for generating responses.
    /// </summary>
    protected string ModelId { get; set; }

    /// <summary>
    /// The conversation history maintained by the Advisor.
    /// </summary>
    public List<Content> History { get; set; }
    
    /// <summary>
    /// The configuration for generating responses using the Gemini API.
    /// </summary>
    protected GenerateContentConfig Config { get; set; }
    
    /// <summary>
    /// The function handler used to execute function calls from the AI's responses.'
    /// </summary>
    protected AIFunctionHandler FunctionHandler { get; set; }

    /// <summary>
    /// Creates a new Advisor instance with the specified Gemini API key.
    /// </summary>
    /// <param name="apiKey"></param>
    public Advisor(string apiKey, string modelName, System.Func<int> getFactionId)
    {
        GemeniClient = new Client(apiKey: apiKey);
        ModelId = modelName;
        History = new List<Content>();
        FunctionHandler = new AIFunctionHandler(getFactionId);
        
        Config = new GenerateContentConfig
        {
            Tools = GetTools(),
            SystemInstruction = new Content
            {
                Role = "system",
                Parts = new List<Part> { new Part { Text = Context } }
            },
        };
    }

    /// <summary>
    /// Sends a message to the Gemini AI model and retrieves a response based on the current conversation context.
    /// Maintains a conversation history and appends the user's message and AI's response to it.
    /// </summary>
    /// <param name="userMessage">The message input provided by the user as part of the conversation.</param>
    /// <param name="attachedEntitiesJson"></param>
    /// <returns>A Task representing the asynchronous operation, with a string containing the AI-generated response.</returns>
    public async Task<string> SendMessageAsync(string userMessage, List<string> attachedEntitiesJson = null, RadChat chatWindow = null, Author aiAuthor = null)
    {
        var userParts = new List<Part> { new Part { Text = userMessage } };
        if (attachedEntitiesJson != null)
        {
            foreach (var entityJson in attachedEntitiesJson)
            {
                var mess = "User attached contextual entity: " + entityJson;
                userParts.Add(new Part { Text = mess });
            }
        }

        // Add user message to history
        History.Add(new Content
        {
            Role = "user",
            Parts = userParts
        });

        // Send message to Gemini API
        try
        {
            var response = await GemeniClient.Models.GenerateContentAsync(ModelId, History, Config);
        
            // Function call loop — keep going until Gemini returns plain text
            while (response.Candidates?[0].Content?.Parts?.Any(p => p.FunctionCall != null) == true)
            {
                var modelContent = response.Candidates[0].Content;
                History.Add(modelContent); // Add the model's function call to history

                // Find the function call in the model's response
                var functionCall = modelContent.Parts?.FirstOrDefault(p => p.FunctionCall != null)?.FunctionCall;
                if (functionCall == null || functionCall.Name == null) continue;

                // Update Window
                if (chatWindow != null && aiAuthor != null)
                {
                    AIChatTextMessage message = new AIChatTextMessage($"Executing function call {functionCall.Name}", aiAuthor, DateTime.Now);
                    chatWindow.AddMessage(message);
                }
            
                // Execute the function
                var args = functionCall.Args != null
                    ? JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(JsonSerializer.Serialize(functionCall.Args))
                    : new Dictionary<string, JsonElement>();

                string result = FunctionHandler.HandleFunctionCall(functionCall.Name, args);

                // Add the function response to history
                History.Add(new Content
                {
                    Role = "function",
                    Parts = new List<Part>
                    {
                        new Part
                        {
                            FunctionResponse = new FunctionResponse
                            {
                                Name = functionCall.Name,
                                Response = JsonSerializer.Deserialize<Dictionary<string, object>>(result)
                            }
                        }
                    }
                });

                // Re-send with updated history
                response = await GemeniClient.Models.GenerateContentAsync(ModelId, History, Config);
            }
            
            // Add AI response to history
            string aiAdvice = response.Text ?? "(No response)";
            History.Add(new Content()
            {
                Role = "model", // Notice the role is "model" for the AI's responses
                Parts = new List<Part> { new Part { Text = aiAdvice } }
            });
        
            // Return AI advice
            return aiAdvice;
        }
        catch (ClientError ex) when (ex.Message.Contains("quota", StringComparison.OrdinalIgnoreCase))
        {
            // Remove the user message we just added since the call failed
            History.RemoveAt(History.Count - 1);
            return "⚠ API quota exceeded. Please wait a minute and try again.";
        }
        catch (ClientError ex)
        {
            History.RemoveAt(History.Count - 1);
            return $"⚠ API error: {ex.Message}";
        }
    }
    
    /// <summary>
    /// Returns a list of tools that the AI can use to assist in designing military organization systems.
    /// </summary>
    /// <returns></returns>
    private static List<Tool> GetTools()
    {
        return new List<Tool>
        {
            new Tool
            {
                FunctionDeclarations = new List<FunctionDeclaration>
                {
                    //
                    // == Faction Tools ==
                    //
                    new FunctionDeclaration
                    {
                        Name = "GetSelectedFactionId",
                        Description = "Returns the ID of the faction the user is currently working in, or Zero if no faction is selected.",
                    },
                    
                    new FunctionDeclaration
                    {
                        Name = "GetFactionById",
                        Description = "Returns the Faction entity (Id, Name) for a given faction ID or null if not found.",
                        Parameters = new Schema
                        {
                            Type = Type.Object,
                            Properties = new Dictionary<string, Schema>
                            {
                                ["factionId"] = new Schema { Type = Type.Integer, Description = "The Faction ID to look up." }
                            },
                            Required = ["factionId"]
                        }
                    },
                    
                    //
                    // == Unit Building Tools ==
                    //
                    
                    // Tool 1: The Discovery Tool
                    new FunctionDeclaration
                    {
                        Name = "GetUnitBlueprintSchema",
                        Description = "Call this FIRST to get the strictly formatted JSON template and database rules before building any military unit.",
                    },
        
                    // Tool 2: The Execution Tool
                    new FunctionDeclaration
                    {
                        Name = "BuildMilitaryUnit",
                        Description = "Executes the creation of the unit in the database. You MUST pass the data as a single stringified JSON object matching the schema from GetUnitBlueprintSchema.",
                        Parameters = new Schema 
                        {
                            Type = Type.Object,
                            Properties = new Dictionary<string, Schema>
                            {
                                { 
                                    "blueprintJsonPayload", 
                                    new Schema { Type = Type.String, Description = "The complete, stringified JSON payload." } 
                                }
                            },
                            Required = ["blueprintJsonPayload"]
                        }
                    },
                    
                    new FunctionDeclaration
                    {
                        Name = "GetEchelons",
                        Description = "Returns all Echelon levels (Fire Team, Squad, Platoon, Company, Battalion, etc.).",
                    },
                    
                    //
                    // == Position Building Tools ==
                    //
                    
                    // Tool 1: The Discovery Tool
                    new FunctionDeclaration
                    {
                        Name = "GetPosBlueprintSchema",
                        Description = "Call this FIRST to get the strictly formatted JSON template and database rules before building any unit position blueprint. Requires a faction ID to scope valid ranks, units, and occupations.",
                        Parameters = new Schema
                        {
                            Type = Type.Object,
                            Properties = new Dictionary<string, Schema>
                            {
                                ["factionId"] = new Schema { Type = Type.Integer, Description = "The Faction ID to scope lookups to." }
                            },
                            Required = ["factionId"]
                        }
                    },
        
                    // Tool 2: The Execution Tool
                    new FunctionDeclaration
                    {
                        Name = "BuildMilitaryPos",
                        Description = "Executes the creation of a position for a unit in the database. You MUST pass the data as a single stringified JSON object matching the schema from GetPosBlueprintSchema.",
                        Parameters = new Schema 
                        {
                            Type = Type.Object,
                            Properties = new Dictionary<string, Schema>
                            {
                                { 
                                    "blueprintJsonPayload", 
                                    new Schema { Type = Type.String, Description = "The complete, stringified JSON payload." } 
                                }
                            },
                            Required = ["blueprintJsonPayload"]
                        }
                    },
                    
                    //
                    // == Rank Building Tools ==
                    //
                    
                    new FunctionDeclaration
                    {
                        Name = "GetRankClassifications",
                        Description = "Returns all RankClassification records (pay grade groups like E-1 through E-9, O-1 through O-10) for a specific faction.",
                        Parameters = new Schema
                        {
                            Type = Type.Object,
                            Properties = new Dictionary<string, Schema>
                            {
                                ["factionId"] = new Schema { Type = Type.Integer, Description = "The Faction ID to filter by." }
                            },
                            Required = ["factionId"]
                        }
                    },
                    
                    new FunctionDeclaration
                    {
                        Name = "GetRanks",
                        Description = "Returns all Rank records (such as Private, Sergeant etc) for a specific faction.",
                        Parameters = new Schema
                        {
                            Type = Type.Object,
                            Properties = new Dictionary<string, Schema>
                            {
                                ["factionId"] = new Schema { Type = Type.Integer, Description = "The Faction ID to filter by." }
                            },
                            Required = ["factionId"]
                        }
                    },
                    
                    new FunctionDeclaration
                    {
                        Name = "GetRanksByClassificationId",
                        Description = "Returns all Rank entities belonging to a specific RankClassification by it's Id.",
                        Parameters = new Schema
                        {
                            Type = Type.Object,
                            Properties = new Dictionary<string, Schema>
                            {
                                ["classificationId"] = new Schema { Type = Type.Integer, Description = "The RankClassification ID" }
                            },
                            Required = ["classificationId"]
                        }
                    },
                    
                    //
                    // == Rank Classification Building Tools ==
                    //

                    // Discovery
                    new FunctionDeclaration
                    {
                        Name = "GetRankClassificationSchema",
                        Description = "Call this FIRST to get the strictly formatted JSON template and database rules before building any RankClassification. Requires a faction ID to scope lookups.",
                        Parameters = new Schema
                        {
                            Type = Type.Object,
                            Properties = new Dictionary<string, Schema>
                            {
                                ["factionId"] = new Schema { Type = Type.Integer, Description = "The Faction ID to scope lookups to." }
                            },
                            Required = ["factionId"]
                        }
                    },

                    // Execution
                    new FunctionDeclaration
                    {
                        Name = "BuildRankClassification",
                        Description = "Creates a RankClassification (pay grade group) in the database. You MUST call GetRankClassificationSchema first. Pass the data as a single stringified JSON object matching that schema. Classifications MUST be created before Ranks.",
                        Parameters = new Schema
                        {
                            Type = Type.Object,
                            Properties = new Dictionary<string, Schema>
                            {
                                ["blueprintJsonPayload"] = new Schema { Type = Type.String, Description = "The complete, stringified JSON payload." }
                            },
                            Required = ["blueprintJsonPayload"]
                        }
                    },

                    //
                    // == Rank Building Tools ==
                    //

                    // Discovery
                    new FunctionDeclaration
                    {
                        Name = "GetRankSchema",
                        Description = "Call this FIRST to get the strictly formatted JSON template and database rules before building any Rank. Requires a faction ID to scope valid RankClassifications.",
                        Parameters = new Schema
                        {
                            Type = Type.Object,
                            Properties = new Dictionary<string, Schema>
                            {
                                ["factionId"] = new Schema { Type = Type.Integer, Description = "The Faction ID to scope lookups to." }
                            },
                            Required = ["factionId"]
                        }
                    },

                    // Execution
                    new FunctionDeclaration
                    {
                        Name = "BuildRank",
                        Description = "Creates a Rank in the database. You MUST call GetRankSchema first. Pass the data as a single stringified JSON object matching that schema. The parent RankClassification MUST already exist.",
                        Parameters = new Schema
                        {
                            Type = Type.Object,
                            Properties = new Dictionary<string, Schema>
                            {
                                ["blueprintJsonPayload"] = new Schema { Type = Type.String, Description = "The complete, stringified JSON payload." }
                            },
                            Required = ["blueprintJsonPayload"]
                        }
                    },
                    
                    //
                    // == Update Tools ==
                    //

                    new FunctionDeclaration
                    {
                        Name = "UpdateFaction",
                        Description = "Updates an existing Faction's data (Name, ShortTag, Description, ThemeColorCode). Pass the faction ID and a stringified JSON object with ONLY the fields you want to change.",
                        Parameters = new Schema
                        {
                            Type = Type.Object,
                            Properties = new Dictionary<string, Schema>
                            {
                                ["factionId"] = new Schema { Type = Type.Integer, Description = "The ID of the Faction to update." },
                                ["updateJsonPayload"] = new Schema { Type = Type.String, Description = "Stringified JSON with only the fields to update. Valid keys: name, shortTag, description, themeColorCode." }
                            },
                            Required = ["factionId", "updateJsonPayload"]
                        }
                    },

                    new FunctionDeclaration
                    {
                        Name = "UpdateUnitBlueprint",
                        Description = "Updates an existing UnitBlueprint's data (Name, UnitNameFormat, UnitCodeFormat, EchelonId, PromotionPoolId). Pass the blueprint ID and a stringified JSON object with ONLY the fields you want to change.",
                        Parameters = new Schema
                        {
                            Type = Type.Object,
                            Properties = new Dictionary<string, Schema>
                            {
                                ["unitBlueprintId"] = new Schema { Type = Type.Integer, Description = "The ID of the UnitBlueprint to update." },
                                ["updateJsonPayload"] = new Schema { Type = Type.String, Description = "Stringified JSON with only the fields to update. Valid keys: name, unitNameFormat, unitCodeFormat, echelonId, promotionPoolId." }
                            },
                            Required = ["unitBlueprintId", "updateJsonPayload"]
                        }
                    },

                    new FunctionDeclaration
                    {
                        Name = "UpdatePositionBlueprint",
                        Description = "Updates an existing PositionBlueprint's data. Pass the blueprint ID and a stringified JSON object with ONLY the fields you want to change.",
                        Parameters = new Schema
                        {
                            Type = Type.Object,
                            Properties = new Dictionary<string, Schema>
                            {
                                ["positionBlueprintId"] = new Schema { Type = Type.Integer, Description = "The ID of the PositionBlueprint to update." },
                                ["updateJsonPayload"] = new Schema { Type = Type.String, Description = "Stringified JSON with only the fields to update. Valid keys: name, unitBlueprintId, catagoryId, targetRankId, positionalRankId, flag, promotionEchelonId, occupationId, stature, prestige, minTourLength, maxTourLength, canRetireEarly, canBePromotedEarly, canLateralEarly, waiverable, selectionMethod, demoteOverRanked, autoPromoteInRankRange, supervisorPositionBlueprintId, zIndex." }
                            },
                            Required = ["positionBlueprintId", "updateJsonPayload"]
                        }
                    },
                    
                    //
                    // == Search / Lookup Tools ==
                    //

                    new FunctionDeclaration
                    {
                        Name = "SearchUnitBlueprints",
                        Description = "Searches for UnitBlueprints by name (supports partial and fuzzy matching). Use this when the user references a unit by name and you need to resolve it to an ID.",
                        Parameters = new Schema
                        {
                            Type = Type.Object,
                            Properties = new Dictionary<string, Schema>
                            {
                                ["factionId"] = new Schema { Type = Type.Integer, Description = "The Faction ID to scope the search to." },
                                ["query"] = new Schema { Type = Type.String, Description = "The search term (partial name, abbreviation, or approximate spelling)." }
                            },
                            Required = ["factionId", "query"]
                        }
                    },

                    new FunctionDeclaration
                    {
                        Name = "SearchRanks",
                        Description = "Searches for Ranks by name or abbreviation (supports partial and fuzzy matching). Use this when the user references a rank by name and you need to resolve it to an ID.",
                        Parameters = new Schema
                        {
                            Type = Type.Object,
                            Properties = new Dictionary<string, Schema>
                            {
                                ["factionId"] = new Schema { Type = Type.Integer, Description = "The Faction ID to scope the search to." },
                                ["query"] = new Schema { Type = Type.String, Description = "The search term (partial name, abbreviation, or approximate spelling)." }
                            },
                            Required = ["factionId", "query"]
                        }
                    },

                    new FunctionDeclaration
                    {
                        Name = "SearchPositionBlueprints",
                        Description = "Searches for PositionBlueprints by name (supports partial and fuzzy matching). Use this when the user references a position by name and you need to resolve it to an ID.",
                        Parameters = new Schema
                        {
                            Type = Type.Object,
                            Properties = new Dictionary<string, Schema>
                            {
                                ["factionId"] = new Schema { Type = Type.Integer, Description = "The Faction ID to scope the search to." },
                                ["query"] = new Schema { Type = Type.String, Description = "The search term (partial name, abbreviation, or approximate spelling)." }
                            },
                            Required = ["factionId", "query"]
                        }
                    },
                }
            }
        };
    }
}