using System;
using Perscom.Database;
using Perscom.Simulation;

namespace Perscom.AI.Dtos;

public static class DtoMapper
{
    public static RankDto ToDto(this Rank entity, string nextRankAbbreviation = null) => new()
    {
        RankClassificationId = entity.RankClassificationId,
        Name = entity.Name,
        Abbreviation = entity.Abbreviation,
        Precedence = entity.Precedence,
        IsPositional = entity.IsPositional,
        NextRankAbbreviation = nextRankAbbreviation,  // Caller resolves this
        Image = entity.Image
    };

    public static Rank ToEntity(this RankDto dto) => new()
    {
        RankClassificationId = dto.RankClassificationId,
        Name = dto.Name,
        Abbreviation = dto.Abbreviation,
        Precedence = dto.Precedence,
        IsPositional = dto.IsPositional,
        NextRankId = null,  // Caller wires this up in pass 2
        Image = dto.Image ?? ""
    };

    // ── RankClassification ────────────────────────────────
    public static RankClassificationDto ToDto(this RankClassification entity) => new()
    {
        Type = entity.Type.ToString(),
        PayGrade = entity.PayGrade,
        Selection = entity.Selection.ToString(),
        LockInTime = entity.LockInTime,
        MinTimeInGrade = entity.MinTimeInGrade,
        MaxTimeInGrade = entity.MaxTimeInGrade,
        PreviousTimeInGradeRequirement = entity.PreviousTimeInGradeRequirement,
        PromotableLength = entity.PromotableLength,
        Stipend = entity.Stipend,
        HasSplitRankLanes = entity.HasSplitRankLanes
    };

    public static RankClassification ToEntity(this RankClassificationDto dto, int factionId) => new()
    {
        FactionId = factionId,
        Type = Enum.Parse<RankType>(dto.Type),
        PayGrade = dto.PayGrade,
        Selection = Enum.Parse<PayGradeSelection>(dto.Selection),
        LockInTime = dto.LockInTime,
        MinTimeInGrade = dto.MinTimeInGrade,
        MaxTimeInGrade = dto.MaxTimeInGrade,
        PreviousTimeInGradeRequirement = dto.PreviousTimeInGradeRequirement,
        PromotableLength = dto.PromotableLength,
        Stipend = dto.Stipend,
        HasSplitRankLanes = dto.HasSplitRankLanes
    };

    // ── UnitBlueprint ─────────────────────────────────────
    public static UnitBlueprintDto ToDto(this UnitBlueprint entity) => new()
    {
        Name = entity.Name,
        EchelonId = entity.EchelonId,
        FactionId = entity.FactionId,
        UnitNameFormat = entity.UnitNameFormat,
        UnitCodeFormat = entity.UnitCodeFormat,
        PromotionPoolId = entity.PromotionPoolId
    };

    public static UnitBlueprint ToEntity(this UnitBlueprintDto dto) => new()
    {
        Name = dto.Name,
        EchelonId = dto.EchelonId,
        FactionId = dto.FactionId,
        UnitNameFormat = dto.UnitNameFormat ?? "",
        UnitCodeFormat = dto.UnitCodeFormat,
        PromotionPoolId = dto.PromotionPoolId
    };

    // ── PositionBlueprint ─────────────────────────────────
    public static PositionBlueprintDto ToDto(this PositionBlueprint entity) => new()
    {
        Name = entity.Name,
        UnitBlueprintId = entity.UnitBlueprintId,
        CatagoryId = entity.CatagoryId,
        TargetRankId = entity.TargetRankId,
        PositionalRankId = entity.PositionalRankId,
        Flag = entity.Flag.ToString(),
        PromotionEchelonId = entity.PromotionEchelonId,
        OccupationId = entity.OccupationId,
        Stature = entity.Stature,
        Prestige = entity.Prestige,
        MinTourLength = entity.MinTourLength,
        MaxTourLength = entity.MaxTourLength,
        CanRetireEarly = entity.CanRetireEarly,
        CanBePromotedEarly = entity.CanBePromotedEarly,
        CanLateralEarly = entity.CanLateralEarly,
        Waiverable = entity.Waiverable,
        SelectionMethod = entity.SelectionMethod.ToString(),
        DemoteOverRanked = entity.DemoteOverRanked,
        AutoPromoteInRankRange = entity.AutoPromoteInRankRange,
        SupervisorPositionBlueprintId = entity.SupervisorPositionBlueprintId,
        ZIndex = entity.ZIndex
    };

    public static PositionBlueprint ToEntity(this PositionBlueprintDto dto) => new()
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
}