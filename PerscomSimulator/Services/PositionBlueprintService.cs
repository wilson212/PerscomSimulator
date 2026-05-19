using System;
using System.Collections.Generic;
using Perscom.AI.Dtos;
using Perscom.Database;

namespace Perscom.Services;

public class PositionBlueprintService
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
    public static ServiceResult<List<PositionBlueprint>> Create(
        List<PositionBlueprintDto> dtos)
    {
        try
        {
            var result = new List<PositionBlueprint>(dtos.Count);

            using var db = new AppDatabase();
            using var ts = db.BeginTransaction();

            foreach (var dto in dtos)
            {
                var pos = dto.ToEntity(db);
                db.PositionBlueprints.Add(pos);
                result.Add(pos);
            }

            ts.Commit();
            return ServiceResult<List<PositionBlueprint>>.Ok(result,
                $"{result.Count} PositionBlueprint(s) created.");
        }
        catch (Exception ex)
        {
            return ServiceResult<List<PositionBlueprint>>.Fail(ex.Message);
        }
    }

    /// <summary>
    /// Updates an existing <see cref="PositionBlueprint"/> record with the provided data.
    /// </summary>
    /// <param name="dto">
    /// The <see cref="UpdatePositionBlueprintDto"/> containing the new values to apply.
    /// The DTO must specify a valid <c>Id</c> corresponding to an existing PositionBlueprint record.
    /// </param>
    /// <returns>
    /// A <see cref="ServiceResult{T}"/> whose <c>Data</c> is the updated <see cref="PositionBlueprint"/>.
    /// If the record is not found or an error occurs, the result will indicate failure with a relevant message.
    /// </returns>
    public static ServiceResult<PositionBlueprint> Update(UpdatePositionBlueprintDto dto)
    {
        try
        {
            using var db = new AppDatabase();
            var pos = db.PositionBlueprints.Find(dto.Id);
            if (pos == null)
                return ServiceResult<PositionBlueprint>.Fail(
                    $"PositionBlueprint with Id {dto.Id} not found.");

            dto.ApplyTo(pos);

            db.PositionBlueprints.Update(pos);
            return ServiceResult<PositionBlueprint>.Ok(pos,
                $"PositionBlueprint '{pos.Name}' (Id={pos.Id}) updated.");
        }
        catch (Exception ex)
        {
            return ServiceResult<PositionBlueprint>.Fail(ex.Message);
        }
    }
}