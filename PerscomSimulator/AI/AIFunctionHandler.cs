using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Perscom.AI.Dtos;
using Perscom.Database;
using Perscom.Simulation;

namespace Perscom.AI
{
    public class AIFunctionHandler
    {
        public string HandleFunctionCall(string functionName, Dictionary<string, JsonElement> args)
        {
            switch (functionName)
            {
                case "GetUnitBlueprintSchema":
                    return GetUnitBlueprintSchema();

                case "BuildMilitaryUnit":
                    return BuildMilitaryUnit(args["blueprintJsonPayload"].GetString());

                case "GetEchelons":
                    return GetEchelons();

                case "GetPosBlueprintSchema":
                    return GetPosBlueprintSchema();

                case "BuildMilitaryPos":
                    return BuildMilitaryPos(args["blueprintJsonPayload"].GetString());

                case "GetRankClassifications":
                    return GetRankClassifications();

                case "GetRanks":
                    return GetRanks();

                case "GetRanksByClassification":
                    int classId = args["classificationId"].GetInt32();
                    return GetRanksByClassification(classId);

                default:
                    return JsonSerializer.Serialize(new { error = $"Unknown function: {functionName}" });
            }
        }

        private string GetEchelons()
        {
            using var db = new AppDatabase();
            var echelons = db.Echelons.ToList();
            return JsonSerializer.Serialize(echelons.Select(e => new
            {
                e.Id,
                e.Name,
                e.HierarchyLevel
            }));
        }

        private string GetRankClassifications()
        {
            using var db = new AppDatabase();
            var items = db.RankClassifications.ToList();
            return JsonSerializer.Serialize(items.Select(r => new
            {
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
            }));
        }

        private string GetRanks()
        {
            using var db = new AppDatabase();
            var ranks = db.Ranks.ToList();
            return JsonSerializer.Serialize(ranks.Select(r => new
            {
                r.Id,
                r.RankClassificationId,
                r.Name,
                r.Abbreviation,
                r.Precedence,
                r.IsPositional,
                r.NextRankId
            }));
        }

        private string GetRanksByClassification(int classificationId)
        {
            using var db = new AppDatabase();
            var ranks = db.Ranks
                .Where(r => r.RankClassificationId == classificationId)
                .ToList();

            return JsonSerializer.Serialize(ranks.Select(r => new
            {
                r.Id,
                r.Name,
                r.Abbreviation,
                r.Precedence,
                r.IsPositional,
                r.NextRankId
            }));
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
                instructions =
                    "Fill in this template. EchelonId and PromotionPoolId must be valid Echelon IDs from the 'validEchelons' list. PromotionPoolId is the echelon level from which soldiers are drawn to fill positions in this unit.",
                validEchelons = echelons,
                template = new
                {
                    name = "(string, required) e.g. 'Rifle Squad'",
                    echelonId = "(int, required) FK to Echelon.Id",
                    unitNameFormat = "(string) e.g. '%n Rifle Squad' where %n=index number, %c=alpha code",
                    unitCodeFormat = "(string, required) e.g. '%n RflSqd'",
                    promotionPoolId = "(int, required) FK to Echelon.Id — the echelon level soldiers are pulled from"
                }
            };

            return JsonSerializer.Serialize(schema);
        }

        /// <summary>
        /// Returns the JSON schema template and rules for PositionBlueprint creation.
        /// </summary>
        private string GetPosBlueprintSchema()
        {
            using var db = new AppDatabase();
            var echelons = db.Echelons.Select(e => new { e.Id, e.Name }).ToList();
            var ranks = db.Ranks.Select(r => new { r.Id, r.Name, r.Abbreviation, r.RankClassificationId }).ToList();
            var occupations = db.Occupations.Select(o => new { o.Id, o.Code, o.Name }).ToList();
            var categories = db.PositionCatagories.Select(c => new { c.Id, c.Name }).ToList();
            var unitBlueprints = db.UnitBlueprints.Select(u => new { u.Id, u.Name }).ToList();

            var schema = new
            {
                instructions =
                    "Fill in this template. All ID fields must reference valid IDs from the lookup lists below.",
                validRanks = ranks,
                validEchelons = echelons,
                validOccupations = occupations,
                validCategories = categories,
                validUnitBlueprints = unitBlueprints,
                validFlags = new[] { "NormalAssignment", "SpecialAssignment", "CommandPosition", "StaffPosition" },
                validSelectionMethods = new[]
                    { "PromotionOrLateral", "PromotionOnly", "LateralOnly", "CreateNewSoldier", "EvaluationBoard" },
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
        private string BuildMilitaryUnit(string jsonPayload)
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
        private string BuildMilitaryPos(string jsonPayload)
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
    }
}