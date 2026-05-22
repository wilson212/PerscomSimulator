using System;
using System.Collections.Generic;
using Perscom.AI.Dtos;
using Perscom.Database;

namespace Perscom.Services;

public class UnitBlueprintService
{
    /// <summary>
    /// Creates one or more <see cref="PositionBlueprint"/> rows inside a single transaction.
    /// All inserts succeed or all are rolled back.
    /// </summary>
    /// <param name="dtos">
    /// The collection of <see cref="PositionBlueprintDto"/> objects. Each DTO must reference
    /// a valid <c>UnitBlueprintId</c>, <c>TargetRankId</c>, and <c>PromotionEchelonId</c>.
    /// String enum fields (<c>Flag</c>, <c>SelectionMethod</c>) are parsed via <see cref="Enum.Parse{T}"/>.
    /// </param>
    /// <returns>
    /// A <see cref="ServiceResult{T}"/> whose <c>Data</c> is a <see cref="BatchCreateResult"/>
    /// containing the count and Id/Name pairs of every inserted blueprint.
    /// </returns>
    public static ServiceResult<List<UnitBlueprint>> Create(
        List<UnitBlueprintDto> dtos)
    {
        try
        {
            var result = new List<UnitBlueprint>(dtos.Count);

            using var db = new AppDatabase();
            using var ts = db.BeginTransaction();

            foreach (var dto in dtos)
            {
                var pos = dto.ToEntity(db);
                db.UnitBlueprints.Add(pos);
                result.Add(pos);
            }

            ts.Commit();
            return ServiceResult<List<UnitBlueprint>>.Ok(result,
                $"{result.Count} PositionBlueprint(s) created.");
        }
        catch (Exception ex)
        {
            return ServiceResult<List<UnitBlueprint>>.Fail(ex.Message);
        }
    }
    
    /// <summary>
    /// Applies a partial update to an existing <see cref="UnitBlueprint"/>.
    /// Only non-null parameters are written; all others are left unchanged.
    /// Validates that the blueprint belongs to the specified faction before updating.
    /// </summary>
    /// <param name="factionId">
    /// The caller's active faction. The update is rejected if the blueprint belongs
    /// to a different faction (prevents cross-faction edits).
    /// </param>
    /// <returns>
    /// A <see cref="ServiceResult{T}"/> containing the updated <see cref="UnitBlueprint"/>
    /// on success, or an error message on failure.
    /// </returns>
    public static ServiceResult<UnitBlueprint> Update(int factionId, int blueprintId, UpdateUnitBlueprintDto dto)
    {
        try
        {
            using var db = new AppDatabase();
            var blueprint = db.UnitBlueprints.Find(blueprintId);
            if (blueprint == null)
                return ServiceResult<UnitBlueprint>.Fail($"UnitBlueprint with Id {blueprintId} not found.");

            if (blueprint.FactionId != factionId)
                return ServiceResult<UnitBlueprint>.Fail($"UnitBlueprint {blueprintId} belongs to a different faction.");

            dto.ApplyTo(blueprint);

            db.UnitBlueprints.Update(blueprint);
            return ServiceResult<UnitBlueprint>.Ok(blueprint, $"UnitBlueprint '{blueprint.Name}' (Id={blueprint.Id}) updated.");
        }
        catch (Exception ex)
        {
            return ServiceResult<UnitBlueprint>.Fail(ex.Message);
        }
    }
}