using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using CrossLite;
using Perscom.AI.Dtos;
using Perscom.Database;
using Perscom.Simulation;

namespace Perscom.AI
{
    /// <summary>
    /// Handles the AI's function calls and returns JSON responses.'
    /// </summary>
    public class AIFunctionHandler
    {
        private Func<int> GetFactionId { get; }
        
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
                case "GetSelectedFactionId":
                    return JsonSerializer.Serialize(new { factionId = GetFactionId() });
                
                case "GetFactionById":
                    return GetFactionById(factionId);
                
                case "GetUnitBlueprintSchema":
                    return GetUnitBlueprintSchema();

                case "BuildMilitaryUnit":
                    return CreateUnitBlueprint(args["blueprintJsonPayload"].GetString());

                case "GetEchelons":
                    return GetEchelons();

                case "GetPosBlueprintSchema":
                    return GetPosBlueprintSchema(factionId);

                case "BuildMilitaryPos":
                    return CreatePositionBlueprint(args["blueprintJsonPayload"].GetString());

                case "GetRankClassifications":
                    return GetRankClassifications(factionId);

                case "GetRanks":
                    return GetRanks(factionId);

                case "GetRanksByClassificationId":
                    int classId = args["classificationId"].GetInt32();
                    return GetRanksByClassification(classId);
                
                case "GetRankClassificationSchema":
                    return GetRankClassificationSchema(factionId);

                case "BuildRankClassification":
                    return CreateRankClassification(args["blueprintJsonPayload"].GetString());

                case "GetRankSchema":
                    return GetRankSchema(factionId);

                case "BuildRank":
                    return CreateRank(args["blueprintJsonPayload"].GetString());
                
                case "UpdateFaction":
                    return UpdateFaction(factionId, args["updateJsonPayload"].GetString());

                case "UpdateUnitBlueprint":
                    return UpdateUnitBlueprint(factionId, args["updateJsonPayload"].GetString());

                case "UpdatePositionBlueprint":
                    return UpdatePositionBlueprint(factionId, args["updateJsonPayload"].GetString());
                
                case "SearchUnitBlueprints":
                    return SearchUnitBlueprints(factionId, args["query"].GetString());

                case "SearchRanks":
                    return SearchRanks(factionId, args["query"].GetString());

                case "SearchPositionBlueprints":
                    return SearchPositionBlueprints(factionId, args["query"].GetString());

                default:
                    return JsonSerializer.Serialize(new { error = $"Unknown function: {functionName}" });
            }
        }

        private string GetFactionById(int factionId)
        {
            using var db = new AppDatabase();
            var faction = db.Factions.Where(f => f.Id == factionId)
                .Select(f => new { f.Id, f.Name })
                .FirstOrDefault();

            return JsonSerializer.Serialize(new
            {
                description = $"Faction lookup for Id {factionId}",
                found = faction != null,
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
            var echelons = db.Echelons.Select(e => new { e.Id, e.Name }).ToList();
            var ranks = GetFactionRanks(db, factionId);
            var occupations = db.Occupations.Where(o => o.FactionId == factionId).Select(o => new { o.Id, o.Code, o.Name }).ToList();
            var categories = db.PositionCatagories.Select(c => new { c.Id, c.Name }).ToList();
            var unitBlueprints = db.UnitBlueprints.Where(b => b.FactionId == factionId).Select(u => new { u.Id, u.Name }).ToList();

            var schema = new
            {
                instructions = "Fill in this template. All ID fields must reference valid IDs from the lookup lists below.",
                validRanks = ranks,
                validEchelons = echelons,
                validOccupations = occupations,
                validCategories = categories,
                validUnitBlueprints = unitBlueprints,
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
        private string CreateUnitBlueprint(string jsonPayload)
        {
            try
            {
                var dto = JsonSerializer.Deserialize<UnitBlueprintDto>(jsonPayload,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                using var db = new AppDatabase();
                var blueprint = new UnitBlueprint
                {
                    Name = dto.Name,
                    EchelonId = dto.EchelonId,
                    FactionId = dto.FactionId,
                    UnitNameFormat = dto.UnitNameFormat ?? "",
                    UnitCodeFormat = dto.UnitCodeFormat,
                    PromotionPoolId = dto.PromotionPoolId
                };

                db.UnitBlueprints.Add(blueprint);

                return JsonSerializer.Serialize(new
                {
                    success = true, id = blueprint.Id,
                    message = $"UnitBlueprint '{blueprint.Name}' created."
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Deserializes the AI's JSON payload and inserts a PositionBlueprint into the database.
        /// </summary>
        private string CreatePositionBlueprint(string jsonPayload)
        {
            try
            {
                var dto = JsonSerializer.Deserialize<PositionBlueprintDto>(jsonPayload,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                using var db = new AppDatabase();
                var pos = new PositionBlueprint
                {
                    Name = dto.Name,
                    UnitBlueprintId = dto.UnitBlueprintId,
                    CatagoryId = dto.CatagoryId,
                    TargetRankId = dto.TargetRankId,
                    PositionalRankId = dto.PositionalRankId,
                    Flag = Enum.Parse<PositionFlag>(dto.Flag),
                    PromotionEchelonId = dto.PromotionEchelonId,
                    OccupationId = dto.OccupationId,
                    Stature = dto.Stature,
                    Prestige = dto.Prestige,
                    MinTourLength = dto.MinTourLength,
                    MaxTourLength = dto.MaxTourLength,
                    CanRetireEarly = dto.CanRetireEarly,
                    CanBePromotedEarly = dto.CanBePromotedEarly,
                    CanLateralEarly = dto.CanLateralEarly,
                    Waiverable = dto.Waiverable,
                    SelectionMethod = Enum.Parse<SelectionProcedure>(dto.SelectionMethod),
                    DemoteOverRanked = dto.DemoteOverRanked,
                    AutoPromoteInRankRange = dto.AutoPromoteInRankRange,
                    SupervisorPositionBlueprintId = dto.SupervisorPositionBlueprintId,
                    ZIndex = dto.ZIndex
                };

                db.PositionBlueprints.Add(pos);

                return JsonSerializer.Serialize(new
                {
                    success = true, id = pos.Id,
                    message = $"PositionBlueprint '{pos.Name}' created."
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { success = false, error = ex.Message });
            }
        }
        
        private List<Rank> GetFactionRanks(AppDatabase db, int factionId)
        {
            var ranks = db.Ranks.ToList()
                .Where(r => r.Classification.FactionId == factionId)
                .ToList();
            
            return ranks;
        }
        
        /// <summary>
        /// Returns the JSON schema template and rules for RankClassification creation.
        /// </summary>
        private string GetRankClassificationSchema(int factionId)
        {
            using var db = new AppDatabase();

            // Show existing classifications so the AI avoids duplicates
            var existing = db.RankClassifications
                .Where(rc => rc.FactionId == factionId)
                .Select(rc => new
                {
                    rc.Id,
                    Type = rc.Type.ToString(),
                    rc.PayGrade,
                    Selection = rc.Selection.ToString(),
                    rc.HasSplitRankLanes
                }).ToList();

            var schema = new
            {
                instructions = "Fill in this template to create a RankClassification (pay grade group). " +
                               "The combination of Type + PayGrade must be unique within the faction. " +
                               "Create classifications in order from lowest pay grade to highest. " +
                               "The first enlisted grade (E-1) should use Selection='EntryLevel'. " +
                               "Lower enlisted grades typically use Selection='Automatic'. " +
                               "Senior grades use Selection='PromotionBoard'.",
                factionId,
                existingClassifications = existing,
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
        /// Deserializes the AI's JSON payload and inserts a RankClassification into the database.
        /// </summary>
        private string CreateRankClassification(string jsonPayload)
        {
            try
            {
                var dto = JsonSerializer.Deserialize<RankClassificationDto>(jsonPayload,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var rankType = Enum.Parse<RankType>(dto.Type);
                var selection = Enum.Parse<PayGradeSelection>(dto.Selection);
                int factionId = GetFactionId();

                using var db = new AppDatabase();

                // Check for duplicate
                var exists = db.RankClassifications
                    .Any(rc => rc.FactionId == factionId
                             && rc.Type == rankType
                             && rc.PayGrade == dto.PayGrade);

                if (exists)
                {
                    return JsonSerializer.Serialize(new
                    {
                        success = false,
                        error = $"A RankClassification for {dto.Type} grade {dto.PayGrade} already exists in this faction."
                    });
                }

                var entity = new RankClassification
                {
                    FactionId = factionId,
                    Type = rankType,
                    PayGrade = dto.PayGrade,
                    Selection = selection,
                    LockInTime = dto.LockInTime,
                    MinTimeInGrade = dto.MinTimeInGrade,
                    MaxTimeInGrade = dto.MaxTimeInGrade,
                    PreviousTimeInGradeRequirement = dto.PreviousTimeInGradeRequirement,
                    PromotableLength = dto.PromotableLength,
                    Stipend = dto.Stipend,
                    HasSplitRankLanes = dto.HasSplitRankLanes
                };

                db.RankClassifications.Add(entity);

                return JsonSerializer.Serialize(new
                {
                    success = true,
                    id = entity.Id,
                    message = $"RankClassification '{dto.Type}-{dto.PayGrade}' created with Id={entity.Id}."
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Returns the JSON schema template and rules for Rank creation.
        /// </summary>
        private string GetRankSchema(int factionId)
        {
            using var db = new AppDatabase();

            // Show valid classifications the rank can belong to
            var classifications = db.RankClassifications
                .Where(rc => rc.FactionId == factionId)
                .Select(rc => new
                {
                    rc.Id,
                    Type = rc.Type.ToString(),
                    rc.PayGrade,
                    rc.HasSplitRankLanes
                }).ToList();

            // Show existing ranks so the AI can reference them for NextRankId and avoid duplicates
            var factionClassIds = classifications.Select(c => c.Id).ToList();
            var existingRanks = db.Ranks
                .Where(r => r.RankClassificationId.In(factionClassIds))
                .Select(r => new
                {
                    r.Id,
                    r.RankClassificationId,
                    r.Name,
                    r.Abbreviation,
                    r.Precedence,
                    r.IsPositional,
                    r.NextRankId
                }).ToList();

            var schema = new
            {
                instructions = "Fill in this template to create a Rank. The RankClassificationId must reference " +
                               "a valid classification from 'validClassifications'. " +
                               "NextRankId is ONLY used when the parent classification has HasSplitRankLanes=true — " +
                               "it points to the specific rank this rank promotes into (e.g., 1stSgt → SgtMaj). " +
                               "Abbreviation must be unique across the entire database. " +
                               "Precedence controls priority within the same classification: " +
                               "entry-level rank = 0, positional/special ranks get higher values.",
                factionId,
                validClassifications = classifications,
                existingRanks = existingRanks,
                template = new
                {
                    rankClassificationId = "(int, required) FK to RankClassification.Id from validClassifications",
                    name = "(string, required) full rank name — e.g. 'Private First Class'",
                    abbreviation = "(string, required, unique) short form — e.g. 'PFC'",
                    precedence = "(int, default 0) priority within the same classification, 0 = base entry rank",
                    isPositional = "(bool, default false) true if this rank can only be achieved via special assignment",
                    nextRankId = "(int?, optional) FK to an existing Rank.Id — only set when HasSplitRankLanes is true on the NEXT classification",
                    image = "(string, default '') optional image filename"
                }
            };

            return JsonSerializer.Serialize(schema);
        }

        /// <summary>
        /// Deserializes the AI's JSON payload and inserts a Rank into the database.
        /// </summary>
        private string CreateRank(string jsonPayload)
        {
            try
            {
                var dto = JsonSerializer.Deserialize<RankDto>(jsonPayload,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                using var db = new AppDatabase();

                // Validate the classification exists and belongs to the active faction
                var classification = db.RankClassifications
                    .FirstOrDefault(rc => rc.Id == dto.RankClassificationId);

                if (classification == null)
                {
                    return JsonSerializer.Serialize(new
                    {
                        success = false,
                        error = $"RankClassificationId {dto.RankClassificationId} does not exist."
                    });
                }

                if (classification.FactionId != GetFactionId())
                {
                    return JsonSerializer.Serialize(new
                    {
                        success = false,
                        error = $"RankClassificationId {dto.RankClassificationId} belongs to a different faction."
                    });
                }

                // Validate NextRankId if provided
                if (dto.NextRankId.HasValue)
                {
                    var nextRank = db.Ranks.FirstOrDefault(r => r.Id == dto.NextRankId.Value);
                    if (nextRank == null)
                    {
                        return JsonSerializer.Serialize(new
                        {
                            success = false,
                            error = $"NextRankId {dto.NextRankId.Value} does not exist."
                        });
                    }
                }

                var entity = new Rank
                {
                    RankClassificationId = dto.RankClassificationId,
                    Name = dto.Name,
                    Abbreviation = dto.Abbreviation,
                    Precedence = dto.Precedence,
                    IsPositional = dto.IsPositional,
                    NextRankId = dto.NextRankId,
                    Image = dto.Image ?? ""
                };

                db.Ranks.Add(entity);

                return JsonSerializer.Serialize(new
                {
                    success = true,
                    id = entity.Id,
                    message = $"Rank '{entity.Name}' ({entity.Abbreviation}) created with Id={entity.Id}, " +
                              $"under classification Id={entity.RankClassificationId}."
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { success = false, error = ex.Message });
            }
        }
        
        /// <summary>
        /// Updates an existing Faction entity with partial data from the AI's JSON payload.
        /// </summary>
        private string UpdateFaction(int factionId, string jsonPayload)
        {
            try
            {
                var data = JsonSerializer.Deserialize<JsonElement>(jsonPayload);

                using var db = new AppDatabase();
                var faction = db.Factions.FirstOrDefault(f => f.Id == factionId);
                if (faction == null)
                    return JsonSerializer.Serialize(new { success = false, error = $"Faction with Id {factionId} not found." });

                if (data.TryGetProperty("name", out var name))
                    faction.Name = name.GetString();

                if (data.TryGetProperty("shortTag", out var tag))
                    faction.ShortTag = tag.GetString();

                if (data.TryGetProperty("description", out var desc))
                    faction.Description = desc.GetString();

                if (data.TryGetProperty("themeColorCode", out var color))
                    faction.ThemeColorCode = color.GetString();

                db.Factions.Update(faction);

                return JsonSerializer.Serialize(new
                {
                    success = true,
                    message = $"Faction '{faction.Name}' (Id={faction.Id}) updated successfully.",
                    faction = new { faction.Id, faction.Name, faction.ShortTag, faction.Description, faction.ThemeColorCode }
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
        private string UpdateUnitBlueprint(int unitBlueprintId, string jsonPayload)
        {
            try
            {
                var data = JsonSerializer.Deserialize<JsonElement>(jsonPayload);

                using var db = new AppDatabase();
                var blueprint = db.UnitBlueprints.FirstOrDefault(u => u.Id == unitBlueprintId);
                if (blueprint == null)
                    return JsonSerializer.Serialize(new { success = false, error = $"UnitBlueprint with Id {unitBlueprintId} not found." });

                // Verify faction ownership
                if (blueprint.FactionId != GetFactionId())
                    return JsonSerializer.Serialize(new { success = false, error = $"UnitBlueprint {unitBlueprintId} belongs to a different faction." });

                if (data.TryGetProperty("name", out var name))
                    blueprint.Name = name.GetString();

                if (data.TryGetProperty("unitNameFormat", out var unf))
                    blueprint.UnitNameFormat = unf.GetString();

                if (data.TryGetProperty("unitCodeFormat", out var ucf))
                    blueprint.UnitCodeFormat = ucf.GetString();

                if (data.TryGetProperty("echelonId", out var eid))
                    blueprint.EchelonId = eid.GetInt32();

                if (data.TryGetProperty("promotionPoolId", out var ppid))
                    blueprint.PromotionPoolId = ppid.GetInt32();

                db.UnitBlueprints.Update(blueprint);

                return JsonSerializer.Serialize(new
                {
                    success = true,
                    message = $"UnitBlueprint '{blueprint.Name}' (Id={blueprint.Id}) updated successfully.",
                    unitBlueprint = new
                    {
                        blueprint.Id, blueprint.Name, blueprint.EchelonId,
                        blueprint.UnitNameFormat, blueprint.UnitCodeFormat, blueprint.PromotionPoolId
                    }
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing PositionBlueprint entity with partial data from the AI's JSON payload.
        /// </summary>
        private string UpdatePositionBlueprint(int positionBlueprintId, string jsonPayload)
        {
            try
            {
                var data = JsonSerializer.Deserialize<JsonElement>(jsonPayload);

                using var db = new AppDatabase();
                var pos = db.PositionBlueprints.FirstOrDefault(p => p.Id == positionBlueprintId);
                if (pos == null)
                    return JsonSerializer.Serialize(new { success = false, error = $"PositionBlueprint with Id {positionBlueprintId} not found." });

                if (data.TryGetProperty("name", out var name))
                    pos.Name = name.GetString();

                if (data.TryGetProperty("unitBlueprintId", out var ubid))
                    pos.UnitBlueprintId = ubid.GetInt32();

                if (data.TryGetProperty("catagoryId", out var cid))
                    pos.CatagoryId = cid.GetInt32();

                if (data.TryGetProperty("targetRankId", out var trid))
                    pos.TargetRankId = trid.GetInt32();

                if (data.TryGetProperty("positionalRankId", out var prid))
                    pos.PositionalRankId = prid.ValueKind == JsonValueKind.Null ? null : prid.GetInt32();

                if (data.TryGetProperty("flag", out var flag))
                    pos.Flag = Enum.Parse<PositionFlag>(flag.GetString());

                if (data.TryGetProperty("promotionEchelonId", out var peid))
                    pos.PromotionEchelonId = peid.GetInt32();

                if (data.TryGetProperty("occupationId", out var oid))
                    pos.OccupationId = oid.GetInt32();

                if (data.TryGetProperty("stature", out var st))
                    pos.Stature = st.GetInt32();

                if (data.TryGetProperty("prestige", out var pr))
                    pos.Prestige = pr.GetInt32();

                if (data.TryGetProperty("minTourLength", out var minT))
                    pos.MinTourLength = minT.GetInt32();

                if (data.TryGetProperty("maxTourLength", out var maxT))
                    pos.MaxTourLength = maxT.GetInt32();

                if (data.TryGetProperty("canRetireEarly", out var cre))
                    pos.CanRetireEarly = cre.GetBoolean();

                if (data.TryGetProperty("canBePromotedEarly", out var cbpe))
                    pos.CanBePromotedEarly = cbpe.GetBoolean();

                if (data.TryGetProperty("canLateralEarly", out var cle))
                    pos.CanLateralEarly = cle.GetBoolean();

                if (data.TryGetProperty("waiverable", out var wav))
                    pos.Waiverable = wav.GetBoolean();

                if (data.TryGetProperty("selectionMethod", out var sm))
                    pos.SelectionMethod = Enum.Parse<SelectionProcedure>(sm.GetString());

                if (data.TryGetProperty("demoteOverRanked", out var dor))
                    pos.DemoteOverRanked = dor.GetBoolean();

                if (data.TryGetProperty("autoPromoteInRankRange", out var apr))
                    pos.AutoPromoteInRankRange = apr.GetBoolean();

                if (data.TryGetProperty("supervisorPositionBlueprintId", out var spid))
                    pos.SupervisorPositionBlueprintId = spid.ValueKind == JsonValueKind.Null ? null : spid.GetInt32();

                if (data.TryGetProperty("zIndex", out var zi))
                    pos.ZIndex = zi.GetInt32();

                db.PositionBlueprints.Update(pos);

                return JsonSerializer.Serialize(new
                {
                    success = true,
                    message = $"PositionBlueprint '{pos.Name}' (Id={pos.Id}) updated successfully.",
                    positionBlueprint = new
                    {
                        pos.Id, pos.Name, pos.UnitBlueprintId, pos.CatagoryId,
                        pos.TargetRankId, pos.PositionalRankId,
                        Flag = pos.Flag.ToString(), pos.PromotionEchelonId,
                        pos.OccupationId, pos.Stature, pos.Prestige,
                        pos.MinTourLength, pos.MaxTourLength,
                        pos.CanRetireEarly, pos.CanBePromotedEarly, pos.CanLateralEarly,
                        pos.Waiverable, SelectionMethod = pos.SelectionMethod.ToString(),
                        pos.DemoteOverRanked, pos.AutoPromoteInRankRange,
                        pos.SupervisorPositionBlueprintId, pos.ZIndex
                    }
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