using System;
using System.Collections.Generic;
using System.Linq;
using Perscom.Database;
using Perscom.Simulation;

namespace Perscom.AI.Dtos;

public static class DtoMapper
{
    // ── Rank ──────────────────────────────────────────────

    /// <summary>
    /// Converts a <see cref="Rank"/> database entity into a serializable <see cref="RankDto"/>.
    /// </summary>
    /// <param name="entity">The source <see cref="Rank"/> entity.</param>
    /// <param name="nextRankAbbreviation">
    /// The abbreviation of the next rank in the promotion chain, resolved by the caller.
    /// </param>
    /// <returns>A new <see cref="RankDto"/> populated from the entity.</returns>
    public static RankDto ToDto(this Rank entity, string nextRankAbbreviation = null) => new()
    {
        RankClassificationId = entity.RankClassificationId,
        Name = entity.Name,
        Abbreviation = entity.Abbreviation,
        Precedence = entity.Precedence,
        IsPositional = entity.IsPositional,
        NextRankAbbreviation = nextRankAbbreviation,
        Image = entity.Image
    };

    /// <summary>
    /// Converts a <see cref="RankDto"/> into a CrossLite-tracked <see cref="Rank"/> entity.
    /// The <see cref="Rank.NextRankId"/> is left null and must be wired up by the caller in a second pass.
    /// </summary>
    /// <param name="dto">The source DTO.</param>
    /// <param name="db">The active database context used to create a tracked entity via <c>DbSet.Create()</c>.</param>
    /// <returns>A tracked <see cref="Rank"/> entity populated from the DTO.</returns>
    public static Rank ToEntity(this RankDto dto, BaseDatabase db)
    {
        var entity = db.Ranks.Create();
        entity.RankClassificationId = dto.RankClassificationId;
        entity.Name = dto.Name;
        entity.Abbreviation = dto.Abbreviation;
        entity.Precedence = dto.Precedence;
        entity.IsPositional = dto.IsPositional;
        entity.NextRankId = null;
        entity.Image = dto.Image ?? "";
        return entity;
    }

    // ── RankClassification ────────────────────────────────

    /// <summary>
    /// Converts a <see cref="RankClassification"/> database entity into a serializable
    /// <see cref="RankClassificationDto"/>.
    /// </summary>
    /// <param name="entity">The source <see cref="RankClassification"/> entity.</param>
    /// <returns>A new <see cref="RankClassificationDto"/> populated from the entity.</returns>
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

    /// <summary>
    /// Converts a <see cref="RankClassificationDto"/> into a CrossLite-tracked
    /// <see cref="RankClassification"/> entity.
    /// </summary>
    /// <param name="dto">The source DTO.</param>
    /// <param name="factionId">The faction ID to assign to the new entity.</param>
    /// <param name="db">The active database context used to create a tracked entity via <c>DbSet.Create()</c>.</param>
    /// <returns>A tracked <see cref="RankClassification"/> entity populated from the DTO.</returns>
    public static RankClassification ToEntity(this RankClassificationDto dto, int factionId, BaseDatabase db)
    {
        var entity = db.RankClassifications.Create();
        entity.FactionId = factionId;
        entity.Type = Enum.Parse<RankType>(dto.Type);
        entity.PayGrade = dto.PayGrade;
        entity.Selection = Enum.Parse<PayGradeSelection>(dto.Selection);
        entity.LockInTime = dto.LockInTime;
        entity.MinTimeInGrade = dto.MinTimeInGrade;
        entity.MaxTimeInGrade = dto.MaxTimeInGrade;
        entity.PreviousTimeInGradeRequirement = dto.PreviousTimeInGradeRequirement;
        entity.PromotableLength = dto.PromotableLength;
        entity.Stipend = dto.Stipend;
        entity.HasSplitRankLanes = dto.HasSplitRankLanes;
        return entity;
    }

    // ── UnitBlueprint ─────────────────────────────────────

    /// <summary>
    /// Converts a <see cref="UnitBlueprint"/> database entity into a serializable
    /// <see cref="UnitBlueprintDto"/>.
    /// </summary>
    /// <param name="entity">The source <see cref="UnitBlueprint"/> entity.</param>
    /// <returns>A new <see cref="UnitBlueprintDto"/> populated from the entity.</returns>
    public static UnitBlueprintDto ToDto(this UnitBlueprint entity) => new()
    {
        Name = entity.Name,
        EchelonId = entity.EchelonId,
        FactionId = entity.FactionId,
        UnitNameFormat = entity.UnitNameFormat,
        UnitCodeFormat = entity.UnitCodeFormat,
        PromotionPoolId = entity.PromotionPoolId
    };

    /// <summary>
    /// Converts a <see cref="UnitBlueprintDto"/> into a CrossLite-tracked
    /// <see cref="UnitBlueprint"/> entity.
    /// </summary>
    /// <param name="dto">The source DTO.</param>
    /// <param name="db">The active database context used to create a tracked entity via <c>DbSet.Create()</c>.</param>
    /// <returns>A tracked <see cref="UnitBlueprint"/> entity populated from the DTO.</returns>
    public static UnitBlueprint ToEntity(this UnitBlueprintDto dto, BaseDatabase db)
    {
        var entity = db.UnitBlueprints.Create();
        entity.Name = dto.Name;
        entity.EchelonId = dto.EchelonId;
        entity.FactionId = dto.FactionId;
        entity.UnitNameFormat = dto.UnitNameFormat ?? "";
        entity.UnitCodeFormat = dto.UnitCodeFormat;
        entity.PromotionPoolId = dto.PromotionPoolId;
        return entity;
    }

    // ── PositionBlueprint ─────────────────────────────────

    /// <summary>
    /// Converts a <see cref="PositionBlueprint"/> database entity into a serializable
    /// <see cref="PositionBlueprintDto"/>.
    /// </summary>
    /// <param name="entity">The source <see cref="PositionBlueprint"/> entity.</param>
    /// <returns>A new <see cref="PositionBlueprintDto"/> populated from the entity.</returns>
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

    /// <summary>
    /// Converts a <see cref="PositionBlueprintDto"/> into a CrossLite-tracked
    /// <see cref="PositionBlueprint"/> entity.
    /// </summary>
    /// <param name="dto">The source DTO.</param>
    /// <param name="db">The active database context used to create a tracked entity via <c>DbSet.Create()</c>.</param>
    /// <returns>A tracked <see cref="PositionBlueprint"/> entity populated from the DTO.</returns>
    public static PositionBlueprint ToEntity(this PositionBlueprintDto dto, BaseDatabase db)
    {
        var entity = db.PositionBlueprints.Create();
        entity.Name = dto.Name;
        entity.UnitBlueprintId = dto.UnitBlueprintId;
        entity.CatagoryId = dto.CatagoryId;
        entity.TargetRankId = dto.TargetRankId;
        entity.PositionalRankId = dto.PositionalRankId;
        entity.Flag = Enum.Parse<PositionFlag>(dto.Flag);
        entity.PromotionEchelonId = dto.PromotionEchelonId;
        entity.OccupationId = dto.OccupationId;
        entity.Stature = dto.Stature;
        entity.Prestige = dto.Prestige;
        entity.MinTourLength = dto.MinTourLength;
        entity.MaxTourLength = dto.MaxTourLength;
        entity.CanRetireEarly = dto.CanRetireEarly;
        entity.CanBePromotedEarly = dto.CanBePromotedEarly;
        entity.CanLateralEarly = dto.CanLateralEarly;
        entity.Waiverable = dto.Waiverable;
        entity.SelectionMethod = Enum.Parse<SelectionProcedure>(dto.SelectionMethod);
        entity.DemoteOverRanked = dto.DemoteOverRanked;
        entity.AutoPromoteInRankRange = dto.AutoPromoteInRankRange;
        entity.SupervisorPositionBlueprintId = dto.SupervisorPositionBlueprintId;
        entity.ZIndex = dto.ZIndex;
        return entity;
    }
    
    /// <summary>
    /// Converts a <see cref="Rank"/> to a lightweight <see cref="EntityRef"/>
    /// using the rank's abbreviation as the Name for quick AI identification.
    /// </summary>
    public static EntityRef ToRef(this Rank entity)
        => new(entity.Id, entity.Abbreviation);

    /// <summary>
    /// Converts a <see cref="RankClassification"/> to a lightweight <see cref="EntityRef"/>
    /// using a "Type-PayGrade" format (e.g., "Enlisted-5") as the Name.
    /// </summary>
    public static EntityRef ToRef(this RankClassification entity)
        => new(entity.Id, $"{entity.Type}-{entity.PayGrade}");

    /// <summary>
    /// Converts a <see cref="UnitBlueprint"/> to a lightweight <see cref="EntityRef"/>.
    /// </summary>
    public static EntityRef ToRef(this UnitBlueprint entity)
        => new(entity.Id, entity.Name);

    /// <summary>
    /// Converts a <see cref="PositionBlueprint"/> to a lightweight <see cref="EntityRef"/>.
    /// </summary>
    public static EntityRef ToRef(this PositionBlueprint entity)
        => new(entity.Id, entity.Name);

    /// <summary>
    /// Converts a <see cref="Faction"/> to a lightweight <see cref="EntityRef"/>.
    /// </summary>
    public static EntityRef ToRef(this Faction entity)
        => new(entity.Id, entity.Name);
    
    /// <summary>
    /// Converts a list of entities to a list of <see cref="EntityRef"/> using the provided selector.
    /// </summary>
    public static List<EntityRef> ToRefs<T>(this IEnumerable<T> entities, Func<T, EntityRef> selector)
        => entities.Select(selector).ToList();
    
    /// <summary>
    /// Applies non-null fields from an <see cref="UpdateFactionDto"/> onto a tracked
    /// <see cref="Faction"/> entity. Only fields explicitly set in the DTO are mutated.
    /// </summary>
    public static void ApplyTo(this UpdateFactionDto dto, Faction entity)
    {
        if (dto.Name != null) entity.Name = dto.Name;
        if (dto.ShortTag != null) entity.ShortTag = dto.ShortTag;
        if (dto.Description != null) entity.Description = dto.Description;
        if (dto.ThemeColorCode != null) entity.ThemeColorCode = dto.ThemeColorCode;
    }

    /// <summary>
    /// Applies non-null fields from an <see cref="UpdateUnitBlueprintDto"/> onto a tracked
    /// <see cref="UnitBlueprint"/> entity.
    /// </summary>
    public static void ApplyTo(this UpdateUnitBlueprintDto dto, UnitBlueprint entity)
    {
        if (dto.Name != null) entity.Name = dto.Name;
        if (dto.UnitNameFormat != null) entity.UnitNameFormat = dto.UnitNameFormat;
        if (dto.UnitCodeFormat != null) entity.UnitCodeFormat = dto.UnitCodeFormat;
        if (dto.EchelonId.HasValue) entity.EchelonId = dto.EchelonId.Value;
        if (dto.PromotionPoolId.HasValue) entity.PromotionPoolId = dto.PromotionPoolId.Value;
    }

    /// <summary>
    /// Applies non-null fields from an <see cref="UpdatePositionBlueprintDto"/> onto a tracked
    /// <see cref="PositionBlueprint"/> entity. Use <c>ClearPositionalRankId</c> and
    /// <c>ClearSupervisorId</c> to explicitly null out nullable FK columns.
    /// </summary>
    public static void ApplyTo(this UpdatePositionBlueprintDto dto, PositionBlueprint entity)
    {
        if (dto.Name != null) entity.Name = dto.Name;
        if (dto.UnitBlueprintId.HasValue) entity.UnitBlueprintId = dto.UnitBlueprintId.Value;
        if (dto.CatagoryId.HasValue) entity.CatagoryId = dto.CatagoryId.Value;
        if (dto.TargetRankId.HasValue) entity.TargetRankId = dto.TargetRankId.Value;
        if (dto.PositionalRankId.HasValue) entity.PositionalRankId = dto.PositionalRankId.Value;
        if (dto.ClearPositionalRankId) entity.PositionalRankId = null;
        if (dto.Flag != null) entity.Flag = Enum.Parse<PositionFlag>(dto.Flag);
        if (dto.PromotionEchelonId.HasValue) entity.PromotionEchelonId = dto.PromotionEchelonId.Value;
        if (dto.OccupationId.HasValue) entity.OccupationId = dto.OccupationId.Value;
        if (dto.Stature.HasValue) entity.Stature = dto.Stature.Value;
        if (dto.Prestige.HasValue) entity.Prestige = dto.Prestige.Value;
        if (dto.MinTourLength.HasValue) entity.MinTourLength = dto.MinTourLength.Value;
        if (dto.MaxTourLength.HasValue) entity.MaxTourLength = dto.MaxTourLength.Value;
        if (dto.CanRetireEarly.HasValue) entity.CanRetireEarly = dto.CanRetireEarly.Value;
        if (dto.CanBePromotedEarly.HasValue) entity.CanBePromotedEarly = dto.CanBePromotedEarly.Value;
        if (dto.CanLateralEarly.HasValue) entity.CanLateralEarly = dto.CanLateralEarly.Value;
        if (dto.Waiverable.HasValue) entity.Waiverable = dto.Waiverable.Value;
        if (dto.SelectionMethod != null) entity.SelectionMethod = Enum.Parse<SelectionProcedure>(dto.SelectionMethod);
        if (dto.DemoteOverRanked.HasValue) entity.DemoteOverRanked = dto.DemoteOverRanked.Value;
        if (dto.AutoPromoteInRankRange.HasValue) entity.AutoPromoteInRankRange = dto.AutoPromoteInRankRange.Value;
        if (dto.SupervisorPositionBlueprintId.HasValue)
            entity.SupervisorPositionBlueprintId = dto.SupervisorPositionBlueprintId.Value;
        if (dto.ClearSupervisorId) entity.SupervisorPositionBlueprintId = null;
        if (dto.ZIndex.HasValue) entity.ZIndex = dto.ZIndex.Value;
    }

    /// <summary>
    /// Applies non-null fields from an <see cref="UpdateRankClassificationDto"/> onto a tracked
    /// <see cref="RankClassification"/> entity. Type and PayGrade are intentionally excluded —
    /// they form the composite unique key and cannot be changed.
    /// </summary>
    public static void ApplyTo(this UpdateRankClassificationDto dto, RankClassification entity)
    {
        if (dto.Selection != null) entity.Selection = Enum.Parse<PayGradeSelection>(dto.Selection);
        if (dto.LockInTime.HasValue) entity.LockInTime = dto.LockInTime.Value;
        if (dto.MinTimeInGrade.HasValue) entity.MinTimeInGrade = dto.MinTimeInGrade.Value;
        if (dto.MaxTimeInGrade.HasValue) entity.MaxTimeInGrade = dto.MaxTimeInGrade.Value;
        if (dto.PreviousTimeInGradeRequirement.HasValue)
            entity.PreviousTimeInGradeRequirement = dto.PreviousTimeInGradeRequirement.Value;
        if (dto.PromotableLength.HasValue) entity.PromotableLength = dto.PromotableLength.Value;
        if (dto.Stipend.HasValue) entity.Stipend = dto.Stipend.Value;
        if (dto.HasSplitRankLanes.HasValue) entity.HasSplitRankLanes = dto.HasSplitRankLanes.Value;
    }

    /// <summary>
    /// Applies non-null fields from an <see cref="UpdateRankDto"/> onto a tracked
    /// <see cref="Rank"/> entity. Use <c>ClearNextRankId</c> to explicitly null out NextRankId.
    /// </summary>
    public static void ApplyTo(this UpdateRankDto dto, Rank entity)
    {
        if (dto.Name != null) entity.Name = dto.Name;
        if (dto.Abbreviation != null) entity.Abbreviation = dto.Abbreviation;
        if (dto.RankClassificationId.HasValue) entity.RankClassificationId = dto.RankClassificationId.Value;
        if (dto.Precedence.HasValue) entity.Precedence = dto.Precedence.Value;
        if (dto.IsPositional.HasValue) entity.IsPositional = dto.IsPositional.Value;
        if (dto.NextRankId.HasValue) entity.NextRankId = dto.NextRankId.Value;
        if (dto.ClearNextRankId) entity.NextRankId = null;
        if (dto.Image != null) entity.Image = dto.Image;
    }
}