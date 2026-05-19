using System;
using System.Collections.Generic;
using System.Linq;
using Google.GenAI;
using Google.GenAI.Types;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Telerik.WinControls.UI;
using Telerik.WinControls.UI.ConversationalUI;
using Telerik.Windows.Diagrams.Core;
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
- When querying ranks, rank classifications, and unit blueprints, Ensure the user has provided the Faction ID. A value of 0 indicates no faction is selected.

CRITICAL RULES:
- Never assume missing information. If a user asks to build a position but does not specify the parent unit, YOU MUST ask the user which unit it belongs to before calling any creation tools.
- If the user uses a vague unit name, use your tools to query the database and confirm the exact unit name with the user.
- Only use plain text when replying to the user. Do not use HTML or markdown.
- ALWAYS complete the user's request fully. After calling a function and receiving results, you MUST use those results to either take the next action or provide a complete answer. Never stop after a single function call without responding to the user.
- If a task requires multiple function calls (e.g., get schema then create), chain them all in one conversation turn. Do not wait for the user to prompt you again.";

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
    /// 
    /// </summary>
    protected AIChatTextMessage _statusMessage = null;

    /// <summary>
    /// Represents an AI-powered advisor for generating content and managing conversation history.
    /// This class serves as a bridge for interacting with the Gemini AI client and provides tools
    /// and configuration for customized AI-driven content generation.
    /// </summary>
    public Advisor(string apiKey, string modelName, Func<int> getFactionId)
    {
        GemeniClient = new Client(apiKey: apiKey);
        ModelId = modelName;
        History = new List<Content>();
        FunctionHandler = new AIFunctionHandler(getFactionId);
        
        Config = new GenerateContentConfig
        {
            Tools = GetTools(),
            ToolConfig = new ToolConfig
            {
                FunctionCallingConfig = new FunctionCallingConfig
                {
                    Mode = FunctionCallingConfigMode.Auto
                }
            },
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

        History.Add(new Content
        {
            Role = "user",
            Parts = userParts
        });
        int historySnapshot = History.Count;

        try
        {
            var response = await GenerateContentWithBackoffAsync();
            response = await ProcessFunctionCallsAsync(response, chatWindow, aiAuthor);

            string aiAdvice = response.Text;

            // Retry loop for empty responses
            int retries = 0;
            while (string.IsNullOrWhiteSpace(aiAdvice) && retries < 3)
            {
                retries++;

                if (response.Candidates?[0]?.Content != null)
                    History.Add(response.Candidates[0].Content);

                History.Add(new Content
                {
                    Role = "user",
                    Parts = new List<Part>
                    {
                        new Part { Text = "Please continue and complete the task. Use the function results you received to provide your answer or take the next action." }
                    }
                });

                response = await GenerateContentWithBackoffAsync();
                response = await ProcessFunctionCallsAsync(response, chatWindow, aiAuthor);

                aiAdvice = response.Text;
            }

            if (string.IsNullOrWhiteSpace(aiAdvice))
                aiAdvice = "(No response after retries)";

            if (_statusMessage != null)
            {
                chatWindow.ChatElement.MessagesViewElement.Items.RemoveLast();
                _statusMessage = null;
            }

            History.Add(new Content
            {
                Role = "model",
                Parts = new List<Part> { new Part { Text = aiAdvice } }
            });

            return aiAdvice;
        }
        catch (ClientError ex) when (ex.Message.Contains("quota", StringComparison.OrdinalIgnoreCase))
        {
            if (History.Count > historySnapshot)
                History.RemoveRange(historySnapshot, History.Count - historySnapshot);
            
            return "⚠ API quota exceeded. Please wait a minute and try again.";
        }
        catch (ServerError ex) when (ex.Message.Contains("high demand", StringComparison.OrdinalIgnoreCase))
        {
            if (History.Count > historySnapshot)
                History.RemoveRange(historySnapshot, History.Count - historySnapshot);
            
            return "⚠ The AI model is currently experiencing high demand. Please try again in a few moments.";
        }
        catch (ClientError ex)
        {
            if (History.Count > historySnapshot)
                History.RemoveRange(historySnapshot, History.Count - historySnapshot);
            
            return $"⚠ API error: {ex.Message}";
        }
    }
    
    /// <summary>
    /// Wraps the Gemini API call with an exponential backoff strategy to handle 503 High Demand server errors.
    /// </summary>
    private async Task<GenerateContentResponse> GenerateContentWithBackoffAsync()
    {
        int maxRetries = 4;
        int baseDelayMilliseconds = 2000; // Start with a 2-second wait
        Random jitterer = new Random();

        for (int retryAttempt = 0; retryAttempt <= maxRetries; retryAttempt++)
        {
            try
            {
                // Execute the actual API call
                return await GemeniClient.Models.GenerateContentAsync(ModelId, History, Config);
            }
            catch (ServerError ex) when (ex.Message.Contains("high demand", StringComparison.OrdinalIgnoreCase) || ex.HResult == unchecked((int)0x80131500))
            {
                if (retryAttempt == maxRetries)
                {
                    // We've exhausted our retries, throw the exception to be caught by SendMessageAsync's try/catch
                    throw; 
                }

                // Calculate exponential backoff: 2s, 4s, 8s, 16s... plus random jitter
                int delay = baseDelayMilliseconds * (int)Math.Pow(2, retryAttempt) + jitterer.Next(0, 1000);
                
                // Wait before trying again
                await Task.Delay(delay);
            }
        }
        
        return null;
    }
    
    /// <summary>
    /// Processes function calls from the model's response in a loop until the model returns plain text.
    /// Returns the final response after all function calls have been handled.
    /// </summary>
    private async Task<GenerateContentResponse> ProcessFunctionCallsAsync(
        GenerateContentResponse response, RadChat chatWindow = null, Author aiAuthor = null)
    {
        while (response.Candidates?[0].Content?.Parts?.Any(p => p.FunctionCall != null) == true)
        {
            var modelContent = response.Candidates[0].Content;
            History.Add(modelContent);

            var functionCall = modelContent.Parts?.FirstOrDefault(p => p.FunctionCall != null)?.FunctionCall;
            if (functionCall == null || functionCall.Name == null) break;

            if (chatWindow != null && aiAuthor != null)
            {
                if (_statusMessage == null)
                {
                    _statusMessage = new AIChatTextMessage($"⚙ Executing function call {functionCall.Name}", aiAuthor, DateTime.Now);
                    chatWindow.AddMessage(_statusMessage);
                }
                else
                {
                    // Update the existing message's text in-place
                    _statusMessage.Message = $"⚙ Executing function call {functionCall.Name}...";
                    chatWindow.Refresh();
                }
            }

            var args = functionCall.Args != null
                ? JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(
                    JsonSerializer.Serialize(functionCall.Args))
                : new Dictionary<string, JsonElement>();

            string result = FunctionHandler.HandleFunctionCall(functionCall.Name, args);

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

            response = await GenerateContentWithBackoffAsync();
        }

        return response;
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
                        Name = "GetSelectedFaction",
                        Description = "Returns the currently selected Faction (Id, Name, ShortTag, Description, ThemeColorCode) that the user is working in, or indicates if no faction is selected.",
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
                        Name = "CreateUnitBlueprints",
                        Description = "Creates one or more Unit Blueprints in the database. You MUST call GetUnitBlueprintSchema first. Pass the data as a stringified JSON ARRAY of objects matching that schema (even for a single item).",
                        Parameters = new Schema
                        {
                            Type = Type.Object,
                            Properties = new Dictionary<string, Schema>
                            {
                                ["blueprintJsonPayload"] = new Schema { Type = Type.String, Description = "A stringified JSON array of Unit Blueprint objects." }
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
                        Name = "CreatePositionBlueprints",
                        Description = "Creates one or more Position Blueprints in the database. You MUST call GetPosBlueprintSchema first. Pass the data as a stringified JSON ARRAY of objects matching that schema (even for a single item).",
                        Parameters = new Schema
                        {
                            Type = Type.Object,
                            Properties = new Dictionary<string, Schema>
                            {
                                ["blueprintJsonPayload"] = new Schema { Type = Type.String, Description = "A stringified JSON array of Position Blueprint objects." }
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
                        Name = "CreateRankClassifications",
                        Description = "Creates one or more Rank Classifications (pay grade groups) in the database. You MUST call GetRankClassificationSchema first. Pass the data as a stringified JSON ARRAY of objects matching that schema (even for a single item). Classifications MUST be created before Ranks.",
                        Parameters = new Schema
                        {
                            Type = Type.Object,
                            Properties = new Dictionary<string, Schema>
                            {
                                ["blueprintJsonPayload"] = new Schema { Type = Type.String, Description = "A stringified JSON array of RankClassification objects." }
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
                    
                    new FunctionDeclaration
                    {
                        Name = "GetRankImages",
                        Description = "Returns a list of all available rank image file paths from the Images/Ranks directory (recursively). " +
                                      "Use the returned 'path' values to set the 'image' field when creating ranks via CreateRanks. " +
                                      "Call this BEFORE creating ranks if you want to assign images.",
                    },

                    // Execution
                    new FunctionDeclaration
                    {
                        Name = "CreateRanks",
                        Description = "Creates one or more Ranks in the database. You MUST call GetRankSchema first. Pass the data as a stringified JSON ARRAY of objects matching that schema (even for a single item). The parent RankClassification MUST already exist. Use 'nextRankAbbreviation' (string) instead of an ID to reference other ranks — this allows referencing ranks within the same payload that haven't been inserted yet.",
                        Parameters = new Schema
                        {
                            Type = Type.Object,
                            Properties = new Dictionary<string, Schema>
                            {
                                ["blueprintJsonPayload"] = new Schema { Type = Type.String, Description = "A stringified JSON array of Rank objects." }
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
                        Description = "Updates an existing UnitBlueprint. Call GetUnitBlueprintSchema FIRST to learn the valid fields. Pass the blueprint ID and a stringified JSON object with ONLY the fields you want to change.",
                        Parameters = new Schema
                        {
                            Type = Type.Object,
                            Properties = new Dictionary<string, Schema>
                            {
                                ["unitBlueprintId"] = new Schema { Type = Type.Integer, Description = "The ID of the UnitBlueprint to update." },
                                ["updateJsonPayload"] = new Schema { Type = Type.String, Description = "Stringified JSON with only the fields to update. Call GetUnitBlueprintSchema first to see valid fields." }
                            },
                            Required = ["unitBlueprintId", "updateJsonPayload"],
                        }
                    },

                    new FunctionDeclaration
                    {
                        Name = "UpdatePositionBlueprint",
                        Description = "Updates an existing PositionBlueprint. Call GetPosBlueprintSchema FIRST to learn the valid fields and lookup values. Pass a stringified JSON object with 'id' (required) and ONLY the fields you want to change.",
                        Parameters = new Schema
                        {
                            Type = Type.Object,
                            Properties = new Dictionary<string, Schema>
                            {
                                ["updateJsonPayload"] = new Schema { Type = Type.String, Description = "Stringified JSON with 'id' (required) and only the fields to update. Call GetPosBlueprintSchema first to see valid fields." }
                            },
                            Required = ["updateJsonPayload"]
                        }
                    },

                    new FunctionDeclaration
                    {
                        Name = "UpdateRankClassification",
                        Description = "Updates an existing RankClassification. Type and PayGrade cannot be changed. Call GetRankClassificationSchema FIRST to learn the valid fields. Pass a stringified JSON object with 'id' (required) and ONLY the fields you want to change.",
                        Parameters = new Schema
                        {
                            Type = Type.Object,
                            Properties = new Dictionary<string, Schema>
                            {
                                ["updateJsonPayload"] = new Schema { Type = Type.String, Description = "Stringified JSON with 'id' (required) and only the fields to update. Call GetRankClassificationSchema first to see valid fields." }
                            },
                            Required = ["updateJsonPayload"]
                        }
                    },

                    new FunctionDeclaration
                    {
                        Name = "UpdateRanks",
                        Description = "Updates one or more existing Ranks. Call GetRankSchema FIRST to learn the valid fields and their meanings. Pass a stringified JSON ARRAY of objects, each with 'id' (required) and ONLY the fields you want to change.",
                        Parameters = new Schema
                        {
                            Type = Type.Object,
                            Properties = new Dictionary<string, Schema>
                            {
                                ["updateJsonPayload"] = new Schema { Type = Type.String, Description = "Stringified JSON ARRAY of objects, each with 'id' (required) and only the fields to update. Call GetRankSchema first to see valid fields." }
                            },
                            Required = ["updateJsonPayload"]
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