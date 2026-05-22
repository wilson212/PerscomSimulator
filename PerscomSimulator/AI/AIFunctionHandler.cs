using CrossLite;
using Perscom.AI.Dtos;
using Perscom.Database;
using Perscom.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Perscom.AI
{
    /// <summary>
    /// Handles the AI's function calls and returns JSON responses.'
    /// </summary>
    public class AIFunctionHandler
    {
        private Func<int> GetFactionId { get; }

        /// <summary>
        /// Represents a static instance of <see cref="JsonSerializerOptions"/> used within the <see cref="AIFunctionHandler"/> class.
        /// The options are configured to allow case-insensitive property name matching during JSON deserialization.
        /// </summary>
        private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };
        
        public AIFunctionHandler(Func<int> getFactionId)
        {
            GetFactionId = getFactionId;
        }

        /// <summary>
        /// Executes a specified function based on its name and provided arguments, and returns the result.
        /// </summary>
        /// <param name="functionName">The name of the function to execute.</param>
        /// <param name="args">A dictionary containing argument names and their corresponding values required for the function call.</param>
        /// <returns>A string containing the result of the executed function or an error message if the function is unknown.</returns>
        public string HandleFunctionCall(string functionName, Dictionary<string, JsonElement> args)
        {
            if (args == null) args = new Dictionary<string, JsonElement>();
            
            // Get faction id if not provided
            var factionId = args.ContainsKey("factionId") ? args["factionId"].GetInt32() : GetFactionId();
            
            switch (functionName)
            {
                case "GetSelectedFaction":
                    return GetSelectedFaction();
                
                case "GetUnitBlueprintSchema":
                    return GetUnitBlueprintSchema();

                case "CreateUnitBlueprints":
                    return CreateUnitBlueprints(args["blueprintJsonPayload"].GetString());

                case "GetEchelons":
                    return GetEchelons();

                case "GetPosBlueprintSchema":
                    return GetPosBlueprintSchema(factionId);

                case "CreatePositionBlueprints":
                    return CreatePositionBlueprints(args["blueprintJsonPayload"].GetString());

                case "GetRankClassifications":
                    return GetRankClassifications(factionId);

                case "GetRanks":
                    return GetRanks(factionId);

                case "GetRanksByClassificationId":
                    int classId = args["classificationId"].GetInt32();
                    return GetRanksByClassification(classId);
                
                case "GetRankClassificationSchema":
                    return GetRankClassificationSchema(factionId);

                case "CreateRankClassifications":
                    return CreateRankClassifications(args["blueprintJsonPayload"].GetString());

                case "GetRankSchema":
                    return GetRankSchema(factionId);

                case "CreateRanks":
                    return CreateRanks(args["blueprintJsonPayload"].GetString());
                
                case "UpdateFaction":
                    return UpdateFaction(factionId, args["updateJsonPayload"].GetString());

                case "UpdateUnitBlueprint":
                    return UpdateUnitBlueprint(factionId, args["unitBlueprintId"].GetInt32(), args["updateJsonPayload"].GetString());

                case "UpdatePositionBlueprint":
                    return UpdatePositionBlueprint(args["updateJsonPayload"].GetString());

                case "UpdateRankClassification":
                    return UpdateRankClassification(factionId, args["updateJsonPayload"].GetString());

                case "UpdateRanks":
                    return UpdateRanks(factionId, args["updateJsonPayload"].GetString());
                
                case "SearchUnitBlueprints":
                    return SearchUnitBlueprints(factionId, args["query"].GetString());

                case "SearchRanks":
                    return SearchRanks(factionId, args["query"].GetString());

                case "SearchPositionBlueprints":
                    return SearchPositionBlueprints(factionId, args["query"].GetString());
                
                case "GetRankImages":
                    return GetRankImages();

                default:
                    return JsonSerializer.Serialize(new { error = $"Unknown function: {functionName}" });
            }
        }

        private string GetSelectedFaction()
        {
            int factionId = GetFactionId();
            if (factionId == 0)
            {
                return JsonSerializer.Serialize(new
                {
                    description = "No faction is currently selected.",
                    found = false,
                    factionId = 0,
                    faction = (object)null
                });
            }

            using var db = new AppDatabase();
            var faction = db.Factions.Where(f => f.Id == factionId)
                .Select(f => new { f.Id, f.Name, f.ShortTag, f.Description, f.ThemeColorCode })
                .FirstOrDefault();

            return JsonSerializer.Serialize(new
            {
                description = "Currently selected faction",
                found = faction != null,
                factionId,
                faction
            });
        }

        /// <summary>
        /// Returns a list of all echelons in the database.
        /// </summary>
        /// <returns></returns>
        private string GetEchelons()
        {
            using var db = new AppDatabase();
            var echelons = db.Echelons.ToList();
            return JsonSerializer.Serialize(new
            {
                description = "All available Echelon levels in the database",
                count = echelons.Count,
                echelons = echelons.Select(e => new
                {
                    e.Id,
                    e.Name,
                    e.HierarchyLevel
                })
            });
        }

        /// <summary>
        /// Returns a list of all rank classifications in the database.
        /// </summary>
        /// <returns></returns>
        private string GetRankClassifications(int factionId)
        {
            using var db = new AppDatabase();
            var items = db.RankClassifications
                .Where(r => r.FactionId == factionId)
                .Select(r => new {
                    r.Id,
                    r.FactionId,
                    Type = r.Type.ToString(),
                    r.PayGrade,
                    Selection = r.Selection.ToString(),
                    r.LockInTime,
                    r.MinTimeInGrade,
                    r.MaxTimeInGrade,
                    r.PreviousTimeInGradeRequirement,
                    r.PromotableLength,
                    r.HasSplitRankLanes
                }).ToList();

            return JsonSerializer.Serialize(new
            {
                description = $"RankClassifications for Faction {factionId}",
                count = items.Count,
                rankClassifications = items
            });
        }

        /// <summary>
        /// Returns a list of all ranks in the database.
        /// </summary>
        /// <returns></returns>
        private string GetRanks(int factionId)
        {
            using var db = new AppDatabase();
            var ranks = GetFactionRanks(db, factionId);

            return JsonSerializer.Serialize(new
            {
                description = $"All Ranks for Faction {factionId}",
                count = ranks.Count,
                ranks = ranks.Select(r => new
                {
                    r.Id,
                    r.RankClassificationId,
                    r.Name,
                    r.Abbreviation,
                    r.Precedence,
                    r.IsPositional,
                    r.NextRankId
                })
            });
        }

        /// <summary>
        /// Returns a list of all ranks in the database that match the specified classification ID.
        /// </summary>
        /// <param name="classificationId"></param>
        /// <returns></returns>
        private string GetRanksByClassification(int classificationId)
        {
            using var db = new AppDatabase();
            var ranks = db.Ranks
                .Where(r => r.RankClassificationId == classificationId)
                .ToList();

            return JsonSerializer.Serialize(new
            {
                description = $"Ranks for RankClassificationId {classificationId}",
                classificationId,
                count = ranks.Count,
                ranks = ranks.Select(r => new
                {
                    r.Id,
                    r.Name,
                    r.Abbreviation,
                    r.Precedence,
                    r.IsPositional,
                    r.NextRankId
                })
            });
        }
        
        /// <summary>
        /// Scans the Images/Ranks directory recursively and returns a list of all
        /// available rank image relative paths, grouped by subfolder.
        /// The AI can use these paths to set the <c>image</c> field when creating ranks.
        /// </summary>
        private string GetRankImages()
        {
            var ranksDir = Path.Combine(Program.RootPath, "Images", "ranks");

            if (!Directory.Exists(ranksDir))
            {
                return JsonSerializer.Serialize(new
                {
                    description = "No Images/Ranks directory found.",
                    count = 0,
                    images = Array.Empty<object>()
                });
            }

            var imagesDir = Path.Combine(Program.RootPath, "Images");
            var files = Directory.GetFiles(ranksDir, "*.*", SearchOption.AllDirectories)
                .Select(fullPath =>
                {
                    // Build relative path from the Images folder: "ranks/US Army/E5.svg"
                    var relative = Path.GetRelativePath(imagesDir, fullPath).Replace('\\', '/');
                    var folder = Path.GetFileName(Path.GetDirectoryName(fullPath));
                    return new { path = relative, folder, fileName = Path.GetFileName(fullPath) };
                })
                .OrderBy(f => f.folder)
                .ThenBy(f => f.fileName)
                .ToList();

            return JsonSerializer.Serialize(new
            {
                description = "Available rank images in Images/ranks (use the 'path' value for the Rank's image field — do NOT include the 'Images/' prefix).",
                count = files.Count,
                images = files
            });
        }

        /// <summary>
        /// Returns the JSON schema template and rules for UnitBlueprint creation.
        /// Gemini calls this FIRST to learn the exact format before building.
        /// </summary>
        private string GetUnitBlueprintSchema()
        {
            // Fetch live echelon data so the AI knows valid IDs
            using var db = new AppDatabase();
            var echelons = db.Echelons.Select(e => new { e.Id, e.Name, e.HierarchyLevel }).ToList();

            var schema = new
            {
                instructions = "Fill in this template. EchelonId and PromotionPoolId must be valid Echelon IDs from the 'validEchelons' list. PromotionPoolId is the echelon level from which soldiers are drawn to fill positions in this unit.",
                validEchelons = echelons,
                template = new
                {
                    name = "(string, required) e.g. 'Rifle Squad'",
                    echelonId = "(int, required) FK to Echelon.Id",
                    factionId = "(int, required) FK to Faction.Id",
                    unitNameFormat = "(string) e.g. '%n Rifle Squad' where %n=index number, %c=alpha code",
                    unitCodeFormat = "(string, required) e.g. '%n RflSqd'",
                    promotionPoolId = "(int, required) FK to Echelon.Id — the echelon level soldiers are pulled from. If you are not sure, ASK THE USER"
                }
            };

            return JsonSerializer.Serialize(schema);
        }

        /// <summary>
        /// Returns the JSON schema template and rules for PositionBlueprint creation.
        /// </summary>
        private string GetPosBlueprintSchema(int factionId)
        {
            using var db = new AppDatabase();
            var occupations = db.Occupations.Where(o => o.FactionId == factionId).Select(o => new { o.Id, o.Code, o.Name }).ToList();
            var categories = db.PositionCatagories.Select(c => new { c.Id, c.Name }).ToList();

            var schema = new
            {
                instructions = "Fill in this template. All ID fields must reference valid IDs from the lookup lists below.",
                validOccupations = occupations,
                validCategories = categories,
                validFlags = new[] { "NormalAssignment", "SpecialAssignment", "CommandPosition", "StaffPosition" },
                validSelectionMethods = new[] { "PromotionOrLateral", "PromotionOnly", "LateralOnly", "CreateNewSoldier", "EvaluationBoard" },
                template = new
                {
                    name = "(string, required)",
                    unitBlueprintId = "(int, required) FK to UnitBlueprint.Id",
                    catagoryId = "(int, required) FK to PositionCatagory.Id",
                    targetRankId = "(int, required) FK to Rank.Id — the rank a soldier should hold",
                    positionalRankId = "(int?, optional) FK to Rank.Id — rank granted while holding this position",
                    flag = "(string, required) one of validFlags",
                    promotionEchelonId = "(int, required) FK to Echelon.Id",
                    occupationId = "(int, required) FK to Occupation.Id",
                    stature = "(int, default 0) higher = filled by more experienced soldiers",
                    prestige = "(int, default 50) competitiveness/cool factor 0-100",
                    minTourLength = "(int, default 0) months minimum in position",
                    maxTourLength = "(int, default 0) months max, 0 = unlimited",
                    canRetireEarly = "(bool, default true)",
                    canBePromotedEarly = "(bool, default true)",
                    canLateralEarly = "(bool, default false)",
                    waiverable = "(bool, default true)",
                    selectionMethod = "(string, default 'PromotionOrLateral') one of validSelectionMethods",
                    demoteOverRanked = "(bool, default false)",
                    autoPromoteInRankRange = "(bool, default false)",
                    supervisorPositionBlueprintId = "(int?, optional) FK to another PositionBlueprint.Id",
                    zIndex = "(int, default 0) display order"
                }
            };

            return JsonSerializer.Serialize(schema);
        }

        /// <summary>
        /// Deserializes the AI's JSON payload and inserts a UnitBlueprint into the database.
        /// </summary>
        private string CreateUnitBlueprints(string jsonPayload)
        {
            try
            {
                var dtos = JsonSerializer.Deserialize<List<UnitBlueprintDto>>(jsonPayload, JsonOpts );
                var result = UnitBlueprintService.Create(dtos);

                if (!result.Success)
                    return JsonSerializer.Serialize(new
                    {
                        success = false,
                        error = result.Error,
                        action = "Fix the error in the payload and resubmit the ENTIRE corrected array.",
                        originalPayload = jsonPayload
                    });

                // Convert full entities to lightweight refs for the AI
                var refs = result.Data.Select(e => e.ToRef()).ToList();

                return JsonSerializer.Serialize(new
                {
                    success = true,
                    count = refs.Count,
                    message = result.Message,
                    created = refs
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    error = ex.Message,
                    action = "Fix the error in the payload and resubmit the ENTIRE corrected array.",
                    originalPayload = jsonPayload
                });
            }
        }

        /// <summary>
        /// Deserializes the AI's JSON payload and inserts a PositionBlueprint into the database.
        /// </summary>
        private string CreatePositionBlueprints(string jsonPayload)
        {
            try
            {
                var dtos = JsonSerializer.Deserialize<List<PositionBlueprintDto>>(jsonPayload, JsonOpts);
                var result = PositionBlueprintService.Create(dtos);

                if (!result.Success)
                    return JsonSerializer.Serialize(new
                    {
                        success = false,
                        error = result.Error,
                        action = "Fix the error in the payload and resubmit the ENTIRE corrected array.",
                        originalPayload = jsonPayload
                    });

                // Convert full entities to lightweight refs for the AI
                var refs = result.Data.Select(e => e.ToRef()).ToList();

                return JsonSerializer.Serialize(new
                {
                    success = true,
                    count = refs.Count,
                    message = result.Message,
                    created = refs
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    error = ex.Message,
                    action = "Fix the error in the payload and resubmit the ENTIRE corrected array.",
                    originalPayload = jsonPayload
                });
            }
        }

        /// <summary>
        /// Retrieves a list of ranks associated with the specified faction from the database.
        /// </summary>
        /// <param name="db">An instance of the <c>AppDatabase</c> used to query rank information.</param>
        /// <param name="factionId">The unique identifier of the faction for which ranks are being retrieved.</param>
        /// <returns>A list of <c>Rank</c> objects representing the ranks assigned to the specified faction.</returns>
        private List<Rank> GetFactionRanks(AppDatabase db, int factionId)
        {
            var classIds = db.RankClassifications
                .Where(rc => rc.FactionId == factionId)
                .Select(rc => rc.Id)
                .ToList();

            return db.Ranks
                .Where(r => r.RankClassificationId.In(classIds))
                .ToList();
        }
        
        /// <summary>
        /// Returns the JSON schema template and rules for RankClassification creation.
        /// </summary>
        private string GetRankClassificationSchema(int factionId)
        {
            var schema = new
            {
                instructions = "Fill in this template to create a RankClassification (pay grade group). " +
                               "The combination of Type + PayGrade must be unique within the faction. " +
                               "Create classifications in order from lowest pay grade to highest. " +
                               "The first enlisted grade (E-1) should use Selection='EntryLevel'. " +
                               "Lower enlisted grades typically use Selection='Automatic'. " +
                               "Senior grades use Selection='PromotionBoard'.",
                factionId,
                validTypes = new[] { "Enlisted", "Officer", "Warrant" },
                validSelections = new[] { "EntryLevel", "Automatic", "PromotionBoard", "SelectionProcedure" },
                template = new
                {
                    type = "(string, required) one of validTypes — e.g. 'Enlisted', 'Officer', 'Warrant'",
                    payGrade = "(int, required) the numeric pay grade — e.g. 1 for E-1, 5 for O-5",
                    selection = "(string, default 'Automatic') one of validSelections — how soldiers reach this grade",
                    lockInTime = "(int, default 0) months a soldier must serve at this grade before being allowed to retire",
                    minTimeInGrade = "(int, default 0) minimum months at this grade to retire at this grade (otherwise demoted on retirement)",
                    maxTimeInGrade = "(int, default 0) maximum months at this grade before forced retirement, 0 = unlimited",
                    previousTimeInGradeRequirement = "(int, default 12) months required at the previous grade before being eligible for promotion to this grade",
                    promotableLength = "(int, default 12) months a soldier retains 'Promotable' status after passing the board",
                    stipend = "(double, default 0) monthly stipend amount for this pay grade",
                    hasSplitRankLanes = "(bool, default false) true if this grade has branching rank tracks (e.g. USMC E-8: 1stSgt vs MSgt)"
                }
            };

            return JsonSerializer.Serialize(schema);
        }

        /// <summary>
        /// Deserializes the AI's JSON payload and inserts RankClassifications into the database via <see cref="RankService"/>.
        /// </summary>
        private string CreateRankClassifications(string jsonPayload)
        {
            try
            {
                var dtos = JsonSerializer.Deserialize<List<RankClassificationDto>>(jsonPayload, JsonOpts);
                int factionId = GetFactionId();
                var result = RankService.CreateClassifications(factionId, dtos);

                if (!result.Success)
                    return JsonSerializer.Serialize(new
                    {
                        success = false,
                        error = result.Error,
                        action = "Fix the error in the payload and resubmit the ENTIRE corrected array.",
                        originalPayload = jsonPayload
                    });

                var refs = result.Data.Select(e => e.ToRef()).ToList();

                return JsonSerializer.Serialize(new
                {
                    success = true,
                    count = refs.Count,
                    message = result.Message,
                    created = refs
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    error = ex.Message,
                    action = "Fix the error in the payload and resubmit the ENTIRE corrected array.",
                    originalPayload = jsonPayload
                });
            }
        }

        /// <summary>
        /// Returns the JSON schema template and rules for Rank creation.
        /// </summary>
        private string GetRankSchema(int factionId)
        {
            var schema = new
            {
                instructions = "Fill in this template to create a Rank. The RankClassificationId must reference " +
                               "a valid classification from 'validClassifications'. " +
                               "NextRankAbbreviation is ONLY used when the parent classification has HasSplitRankLanes=true — " +
                               "it points to the specific rank this rank promotes into (e.g., 1stSgt → SgtMaj). " +
                               "Abbreviation must be unique across the entire database. " +
                               "Precedence controls priority within the same classification: " +
                               "entry-level rank = 0, positional/special ranks get higher values.",
                factionId,
                template = new
                {
                    rankClassificationId = "(int, required) FK to RankClassification.Id from validClassifications",
                    name = "(string, required) full rank name — e.g. 'Private First Class'",
                    abbreviation = "(string, required, unique) short form — e.g. 'PFC'",
                    precedence = "(int, default 0) priority within the same classification, 0 = base entry rank",
                    isPositional = "(bool, default false) true if this rank can only be achieved via special assignment",
                    nextRankAbbreviation = "(string?, optional) The ABBREVIATION of the rank this rank promotes into — ONLY SET when the RankClassification HasSplitRankLanes is true. Can reference a rank in the same payload or an existing rank.",
                    image = "(string, default '') relative image path WITHOUT the 'Images/' prefix (e.g. 'ranks/US Marines/E2.svg'). Use GetRankImages to get available paths."
                }
            };

            return JsonSerializer.Serialize(schema);
        }

        /// <summary>
        /// Deserializes the AI's JSON payload and inserts Ranks into the database via <see cref="RankService"/>.
        /// </summary>
        private string CreateRanks(string jsonPayload)
        {
            try
            {
                var dtos = JsonSerializer.Deserialize<List<RankDto>>(jsonPayload, JsonOpts);
                int factionId = GetFactionId();
                var result = RankService.CreateRanks(factionId, dtos);

                if (!result.Success)
                    return JsonSerializer.Serialize(new
                    {
                        success = false,
                        error = result.Error,
                        action = "Fix the error in the payload and resubmit the ENTIRE corrected array.",
                        originalPayload = jsonPayload
                    });

                var refs = result.Data.Select(e => e.ToRef()).ToList();

                return JsonSerializer.Serialize(new
                {
                    success = true,
                    count = refs.Count,
                    message = result.Message,
                    created = refs
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    error = ex.Message,
                    action = "Fix the error in the payload and resubmit the ENTIRE corrected array.",
                    originalPayload = jsonPayload
                });
            }
        }
        
        /// <summary>
        /// Updates an existing Faction entity with partial data from the AI's JSON payload via <see cref="FactionService"/>.
        /// </summary>
        private string UpdateFaction(int factionId, string jsonPayload)
        {
            try
            {
                var dto = JsonSerializer.Deserialize<UpdateFactionDto>(jsonPayload, JsonOpts);
                var result = FactionService.Update(factionId, dto);

                if (!result.Success)
                    return JsonSerializer.Serialize(new { success = false, error = result.Error });

                return JsonSerializer.Serialize(new
                {
                    success = true,
                    message = result.Message,
                    entity = result.Data.ToRef()
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing UnitBlueprint entity with partial data from the AI's JSON payload.
        /// </summary>
        private string UpdateUnitBlueprint(int factionId, int unitBlueprintId, string jsonPayload)
        {
            try
            {
                var dto = JsonSerializer.Deserialize<UpdateUnitBlueprintDto>(jsonPayload, JsonOpts);
                var result = UnitBlueprintService.Update(factionId, unitBlueprintId, dto);

                if (!result.Success)
                    return JsonSerializer.Serialize(new { success = false, error = result.Error });

                return JsonSerializer.Serialize(new
                {
                    success = true,
                    message = result.Message,
                    entity = result.Data.ToRef()
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing PositionBlueprint entity with partial data from the AI's JSON payload via <see cref="PositionBlueprintService"/>.
        /// </summary>
        private string UpdatePositionBlueprint(string jsonPayload)
        {
            try
            {
                var dto = JsonSerializer.Deserialize<UpdatePositionBlueprintDto>(jsonPayload, JsonOpts);
                var result = PositionBlueprintService.Update(dto);

                if (!result.Success)
                    return JsonSerializer.Serialize(new { success = false, error = result.Error });

                return JsonSerializer.Serialize(new
                {
                    success = true,
                    message = result.Message,
                    entity = result.Data.ToRef()
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing RankClassification entity with partial data from the AI's JSON payload via <see cref="RankService"/>.
        /// </summary>
        private string UpdateRankClassification(int factionId, string jsonPayload)
        {
            try
            {
                var dto = JsonSerializer.Deserialize<UpdateRankClassificationDto>(jsonPayload, JsonOpts);
                var result = RankService.UpdateClassification(factionId, dto);

                if (!result.Success)
                    return JsonSerializer.Serialize(new { success = false, error = result.Error });

                return JsonSerializer.Serialize(new
                {
                    success = true,
                    message = result.Message,
                    entity = result.Data.ToRef()
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Updates ranks for a specific faction using the provided JSON payload.
        /// </summary>
        /// <param name="factionId">The unique identifier of the faction whose ranks are to be updated.</param>
        /// <param name="jsonPayload">A JSON string containing a list of rank update data transfer objects (DTOs).</param>
        /// <returns>A JSON string indicating the success or failure of the update operation, along with relevant details.</returns>
        private string UpdateRanks(int factionId, string jsonPayload)
        {
            try
            {
                var dtos = JsonSerializer.Deserialize<List<UpdateRankDto>>(jsonPayload, JsonOpts);
                var result = RankService.UpdateRanks(factionId, dtos);

                if (!result.Success)
                    return JsonSerializer.Serialize(new
                    {
                        success = false,
                        error = result.Error,
                        action = "Fix the error in the payload and resubmit the ENTIRE corrected array.",
                        originalPayload = jsonPayload
                    });

                var refs = result.Data.Select(r => new { id = r.Id, name = r.Name }).ToList();

                return JsonSerializer.Serialize(new
                {
                    success = true,
                    count = refs.Count,
                    message = result.Message,
                    updated = refs
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { success = false, error = ex.Message });
            }
        }
        
        /// <summary>
        /// Searches UnitBlueprints by name using partial + fuzzy matching.
        /// </summary>
        private string SearchUnitBlueprints(int factionId, string query)
        {
            using var db = new AppDatabase();
            var all = db.UnitBlueprints
                .Where(u => u.FactionId == factionId)
                .ToList();

            var results = FuzzyMatch(all, query,
                u => new[] { u.Name, u.UnitCodeFormat },
                u => new { u.Id, u.Name, u.EchelonId, u.UnitNameFormat, u.UnitCodeFormat, u.PromotionPoolId });

            return JsonSerializer.Serialize(new
            {
                description = $"UnitBlueprint search results for '{query}' in Faction {factionId}",
                query,
                count = results.Count,
                results
            });
        }

        /// <summary>
        /// Searches Ranks by name or abbreviation using partial + fuzzy matching.
        /// </summary>
        private string SearchRanks(int factionId, string query)
        {
            using var db = new AppDatabase();
            var factionRanks = GetFactionRanks(db, factionId);

            var results = FuzzyMatch(factionRanks, query,
                r => new[] { r.Name, r.Abbreviation },
                r => new { r.Id, r.RankClassificationId, r.Name, r.Abbreviation, r.Precedence, r.IsPositional, r.NextRankId });

            return JsonSerializer.Serialize(new
            {
                description = $"Rank search results for '{query}' in Faction {factionId}",
                query,
                count = results.Count,
                results
            });
        }

        /// <summary>
        /// Searches PositionBlueprints by name using partial + fuzzy matching.
        /// </summary>
        private string SearchPositionBlueprints(int factionId, string query)
        {
            using var db = new AppDatabase();

            // Get faction-scoped unit blueprint IDs to filter positions
            var factionUnitIds = db.UnitBlueprints
                .Where(u => u.FactionId == factionId)
                .Select(u => u.Id)
                .ToList();

            var all = db.PositionBlueprints
                .Where(p => factionUnitIds.Contains(p.UnitBlueprintId))
                .ToList();

            var results = FuzzyMatch(all, query,
                p => new[] { p.Name },
                p => new
                {
                    p.Id, p.Name, p.UnitBlueprintId, p.CatagoryId,
                    p.TargetRankId, p.PositionalRankId,
                    Flag = p.Flag.ToString(), p.PromotionEchelonId,
                    p.OccupationId, p.Stature, p.Prestige
                });

            return JsonSerializer.Serialize(new
            {
                description = $"PositionBlueprint search results for '{query}' in Faction {factionId}",
                query,
                count = results.Count,
                results
            });
        }

        /// <summary>
        /// Generic fuzzy matching helper. Scores entities by substring containment first,
        /// then falls back to Levenshtein distance for misspelling tolerance.
        /// Returns top 10 results sorted by relevance.
        /// </summary>
        private List<object> FuzzyMatch<T>(
            List<T> entities,
            string query,
            Func<T, string[]> fieldSelector,
            Func<T, object> projection)
        {
            string queryLower = query.ToLowerInvariant();

            var scored = entities.Select(e =>
            {
                string[] fields = fieldSelector(e)
                    .Where(f => f != null)
                    .Select(f => f.ToLowerInvariant())
                    .ToArray();

                int bestScore = int.MaxValue;

                foreach (var field in fields)
                {
                    // Exact match = best possible score
                    if (field == queryLower)
                    {
                        bestScore = 0;
                        break;
                    }

                    // Substring containment = very good score
                    if (field.Contains(queryLower) || queryLower.Contains(field))
                    {
                        int score = Math.Abs(field.Length - queryLower.Length);
                        bestScore = Math.Min(bestScore, score + 1);
                        continue;
                    }

                    // Levenshtein distance for fuzzy/misspelling tolerance
                    int distance = LevenshteinDistance(field, queryLower);
                    // Normalize: allow up to ~40% character errors
                    if (distance <= Math.Max(queryLower.Length, field.Length) * 0.4)
                    {
                        bestScore = Math.Min(bestScore, distance + 100); // offset so substring matches rank higher
                    }
                }

                return new { Entity = e, Score = bestScore };
            })
            .Where(x => x.Score < int.MaxValue)
            .OrderBy(x => x.Score)
            .Take(10)
            .Select(x => projection(x.Entity))
            .ToList();

            return scored;
        }

        /// <summary>
        /// Standard Levenshtein distance calculation for fuzzy string matching.
        /// </summary>
        private static int LevenshteinDistance(string a, string b)
        {
            if (string.IsNullOrEmpty(a)) return b?.Length ?? 0;
            if (string.IsNullOrEmpty(b)) return a.Length;

            int[,] d = new int[a.Length + 1, b.Length + 1];

            for (int i = 0; i <= a.Length; i++) d[i, 0] = i;
            for (int j = 0; j <= b.Length; j++) d[0, j] = j;

            for (int i = 1; i <= a.Length; i++)
            {
                for (int j = 1; j <= b.Length; j++)
                {
                    int cost = a[i - 1] == b[j - 1] ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }

            return d[a.Length, b.Length];
        }
    }
}