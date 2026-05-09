using System.Collections.Generic;
using System.Linq;
using Google.GenAI;
using Google.GenAI.Types;
using System.Text.Json;
using System.Threading.Tasks;

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
    private const string Context =
        @"You are a military organization design assistant for a MILSIM Airsoft league simulator.
You help users create rank systems, unit blueprints, and position blueprints.
Always use the provided functions to query existing database state before making suggestions.
When the user confirms a design, output structured JSON for entity creation.";
    
    /// <summary>
    /// The Gemini model ID used for generating responses.
    /// </summary>
    private const string ModelId = "gemini-3.1-pro";
    
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
    public Advisor(string apiKey)
    {
        GemeniClient = new Client(apiKey: apiKey);
        History = new List<Content>();
        FunctionHandler = new AIFunctionHandler();
        
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
    public async Task<string> SendMessageAsync(string userMessage, List<string> attachedEntitiesJson = null)
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
        var response = await GemeniClient.Models.GenerateContentAsync(ModelId, History, Config);
        
        // Function call loop — keep going until Gemini returns plain text
        while (response.Candidates?[0].Content?.Parts?.Any(p => p.FunctionCall != null) == true)
        {
            var modelContent = response.Candidates[0].Content;
            History.Add(modelContent); // Add the model's function call to history

            // Find the function call in the model's response
            var functionCall = modelContent.Parts?.FirstOrDefault(p => p.FunctionCall != null)?.FunctionCall;
            if (functionCall == null || functionCall.Name == null) continue;
            
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
                        Parameters = new Schema { Type = Type.Integer, Properties = new Dictionary<string, Schema>() }
                    },
                    
                    //
                    // == Position Building Tools ==
                    //
                    
                    // Tool 1: The Discovery Tool
                    new FunctionDeclaration
                    {
                        Name = "GetPosBlueprintSchema",
                        Description = "Call this FIRST to get the strictly formatted JSON template and database rules before building any military unit position.",
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
                        Description = "Returns all RankClassification records (pay grade groups like E-1 through E-9, O-1 through O-10) from the database.",
                    },
                    
                    new FunctionDeclaration
                    {
                        Name = "GetRanks",
                        Description = "Returns all Rank records (such as Private, Sergeant etc) from the database.",
                    },
                    
                    new FunctionDeclaration
                    {
                        Name = "GetRanksByClassification",
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
                }
            }
        };
    }
}