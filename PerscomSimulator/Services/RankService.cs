using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Perscom.AI.Dtos;
using Perscom.Database;
using Perscom.Simulation;

namespace Perscom.Services;

/// <summary>
/// Provides shared CRUD operations for <see cref="RankClassification"/> and <see cref="Rank"/>
/// entities. Enforces faction-scoping, duplicate detection, and the two-pass NextRankId wiring
/// needed for split-lane rank structures.
/// </summary>
public class RankService
{
    /// <summary>
    /// Creates one or more <see cref="RankClassification"/> rows inside a single transaction.
    /// Validates that each Type+PayGrade combination is unique within the faction.
    /// </summary>
    /// <param name="factionId">
    /// The faction these classifications belong to. Used for duplicate detection.
    /// </param>
    /// <param name="dtos">
    /// The collection of <see cref="RankClassificationDto"/> objects. String enum fields
    /// (<c>Type</c>, <c>Selection</c>) are parsed via <see cref="Enum.Parse{T}"/>.
    /// </param>
    /// <returns>
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown (and caught internally) if a Type+PayGrade combination already exists,
    /// causing the entire transaction to roll back.
    /// </exception>
    public static ServiceResult<List<RankClassification>> CreateClassifications(
        int factionId, List<RankClassificationDto> dtos)
    {
        try
        {
            var created = new List<RankClassification>();

            using var db = new AppDatabase();
            using var ts = db.BeginTransaction();

            foreach (var dto in dtos)
            {
                var rankType = Enum.Parse<RankType>(dto.Type);
                var exists = db.RankClassifications
                    .Any(rc => rc.FactionId == factionId && rc.Type == rankType && rc.PayGrade == dto.PayGrade);

                if (exists)
                    throw new InvalidOperationException(
                        $"{dto.Type}-{dto.PayGrade} already exists in this faction. " +
                        "Remove it from the payload or change its PayGrade.");

                var entity = dto.ToEntity(factionId, db);
                db.RankClassifications.Add(entity);
                created.Add(entity);
            }

            ts.Commit();
            return ServiceResult<List<RankClassification>>.Ok(created, $"{created.Count} RankClassification(s) created.");
        }
        catch (Exception ex)
        {
            return ServiceResult<List<RankClassification>>.Fail(ex.Message);
        }
    }

    /// <summary>
    /// Creates one or more <see cref="Rank"/> rows inside a single transaction using a
    /// two-pass strategy:
    /// <list type="number">
    ///   <item><description>
    ///     <b>Pass 1 — Insert:</b> All ranks are inserted with <c>NextRankId = null</c>.
    ///     This guarantees every rank has a valid Id before cross-references are resolved.
    ///   </description></item>
    ///   <item><description>
    ///     <b>Pass 2 — Wire:</b> For each DTO with a <c>NextRankAbbreviation</c>,
    ///     the target rank is looked up — first in the current payload, then in the
    ///     existing database — and <c>NextRankId</c> is set accordingly.
    ///   </description></item>
    /// </list>
    /// </summary>
    /// <param name="factionId">
    /// The caller's active faction. Each DTO's <c>RankClassificationId</c> is validated
    /// to ensure it belongs to this faction.
    /// </param>
    /// <param name="dtos">The collection of <see cref="RankDto"/> objects.</param>
    /// <returns>
    /// A <see cref="ServiceResult{T}"/> with the list of created <see cref="Rank"/> entities.
    /// </returns>
    public static ServiceResult<List<Rank>> CreateRanks(int factionId, List<RankDto> dtos)
    {
        try
        {
            var created = new List<Rank>();

            using var db = new AppDatabase();
            using var ts = db.BeginTransaction();

            // Pass 1: Insert all ranks with NextRankId deferred
            var abbreviationToEntity = new Dictionary<string, Rank>(StringComparer.OrdinalIgnoreCase);

            foreach (var dto in dtos)
            {
                var classification = db.RankClassifications.Find(dto.RankClassificationId);

                if (classification == null)
                    throw new InvalidOperationException(
                        $"RankClassificationId {dto.RankClassificationId} does not exist. " +
                        $"Cannot create rank '{dto.Name}'.");

                if (classification.FactionId != factionId)
                    throw new InvalidOperationException(
                        $"RankClassificationId {dto.RankClassificationId} belongs to a different faction. " +
                        $"Cannot create rank '{dto.Name}'.");

                var entity = dto.ToEntity(db);
                db.Ranks.Add(entity);
                abbreviationToEntity[dto.Abbreviation] = entity;
                created.Add(entity);
            }

            // Pass 2: Wire up NextRankId using abbreviation lookups
            foreach (var dto in dtos.Where(d => !string.IsNullOrEmpty(d.NextRankAbbreviation)))
            {
                if (!abbreviationToEntity.TryGetValue(dto.Abbreviation, out var sourceRank))
                    continue;

                // GUARD: Only set NextRankId when the classification has split rank lanes
                var classification = db.RankClassifications.Find(sourceRank.RankClassificationId);
                if (classification == null || !classification.HasSplitRankLanes)
                    continue;

                if (abbreviationToEntity.TryGetValue(dto.NextRankAbbreviation, out var targetRank))
                {
                    sourceRank.NextRankId = targetRank.Id;
                }
                else
                {
                    var existingTarget = db.Ranks
                        .FirstOrDefault(r => r.Abbreviation == dto.NextRankAbbreviation);

                    if (existingTarget == null)
                    {
                        string abbr = dto.NextRankAbbreviation;
                        string rankName = dto.Name;
                        throw new InvalidOperationException(
                            $"NextRankAbbreviation '{abbr}' does not match any rank " +
                            $"in the payload or database. Cannot wire rank '{rankName}'.");
                    }

                    sourceRank.NextRankId = existingTarget.Id;
                }

                db.Ranks.Update(sourceRank);
            }

            ts.Commit();
            return ServiceResult<List<Rank>>.Ok(created, $"{created.Count} Rank(s) created.");
        }
        catch (Exception ex)
        {
            return ServiceResult<List<Rank>>.Fail(ex.Message);
        }
    }
    
    public static ServiceResult<RankClassification> UpdateClassification(
        int factionId, UpdateRankClassificationDto dto)
    {
        try
        {
            using var db = new AppDatabase();
            var entity = db.RankClassifications.Find(dto.Id);
            if (entity == null)
                return ServiceResult<RankClassification>.Fail($"RankClassification with Id {dto.Id} not found.");

            if (entity.FactionId != factionId)
                return ServiceResult<RankClassification>.Fail($"RankClassification {dto.Id} belongs to a different faction.");

            dto.ApplyTo(entity);

            db.RankClassifications.Update(entity);
            return ServiceResult<RankClassification>.Ok(entity, $"RankClassification (Id={entity.Id}) updated.");
        }
        catch (Exception ex)
        {
            return ServiceResult<RankClassification>.Fail(ex.Message);
        }
    }

    public static ServiceResult<Rank> UpdateRank(int factionId, UpdateRankDto dto)
    {
        try
        {
            using var db = new AppDatabase();
            var entity = db.Ranks.Find(dto.Id);
            if (entity == null)
                return ServiceResult<Rank>.Fail($"Rank with Id {dto.Id} not found.");

            var classification = db.RankClassifications.Find(entity.RankClassificationId);
            if (classification == null || classification.FactionId != factionId)
                return ServiceResult<Rank>.Fail($"Rank {dto.Id} belongs to a different faction.");

            // If changing classification, validate faction ownership
            if (dto.RankClassificationId.HasValue)
            {
                var newClassification = db.RankClassifications.Find(dto.RankClassificationId.Value);
                if (newClassification == null)
                    return ServiceResult<Rank>.Fail($"RankClassificationId {dto.RankClassificationId.Value} does not exist.");
                if (newClassification.FactionId != factionId)
                    return ServiceResult<Rank>.Fail($"RankClassificationId {dto.RankClassificationId.Value} belongs to a different faction.");
            }
            
            // Don't set next rank if classification doesn't have split lanes
            if (dto.NextRankId.HasValue && !classification.HasSplitRankLanes)
            {
                dto.NextRankId = null;
            }

            dto.ApplyTo(entity);

            db.Ranks.Update(entity);
            return ServiceResult<Rank>.Ok(entity, $"Rank '{entity.Name}' (Id={entity.Id}) updated.");
        }
        catch (Exception ex)
        {
            return ServiceResult<Rank>.Fail(ex.Message);
        }
    }

    /// <summary>
    /// Updates one or more <see cref="Rank"/> records within a single transaction.
    /// Ensures all validations and updates are performed atomically.
    /// </summary>
    /// <param name="factionId">
    /// The identifier of the faction to which the ranks belong. Used for contextual validation.
    /// </param>
    /// <param name="dtos">
    /// A list of <see cref="UpdateRankDto"/> instances representing the data for updating existing ranks.
    /// Each DTO contains fields for identifying and modifying rank records.
    /// </param>
    /// <returns>
    /// A <see cref="ServiceResult{T}"/> containing the list of updated <see cref="Rank"/> objects
    /// if successful, or an error message if the operation fails.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown (and caught internally) if a validation error occurs or
    /// if any of the updates fail, resulting in a transaction rollback.
    /// </exception>
    public static ServiceResult<List<Rank>> UpdateRanks(int factionId, List<UpdateRankDto> dtos)
    {
        try
        {
            using var db = new AppDatabase();
            using var ts = db.BeginTransaction();
            var updated = new List<Rank>();
            
            Debug.Write("Updating Ranks:");
            Debug.WriteLine(string.Join(", ", dtos.Select(d => d.Id)));

            foreach (var dto in dtos)
            {
                // Id is required for update
                if (dto.Id == 0)
                    throw new InvalidOperationException("Cannot update a rank with Id=0.");
                
                // Find the entity to update
                var entity = db.Ranks.Find(dto.Id);
                if (entity == null)
                    throw new InvalidOperationException($"Rank with Id {dto.Id} not found.");

                // Query the classification directly instead of lazy-loading
                var classification = db.RankClassifications.Find(entity.RankClassificationId);
                if (classification == null || classification.FactionId != factionId)
                    throw new InvalidOperationException($"Rank {dto.Id} belongs to a different faction.");

                if (dto.RankClassificationId.HasValue)
                {
                    var newClassification = db.RankClassifications.Find(dto.RankClassificationId.Value);
                    if (newClassification == null)
                        throw new InvalidOperationException($"RankClassificationId {dto.RankClassificationId.Value} does not exist.");
                    
                    if (newClassification.FactionId != factionId)
                        throw new InvalidOperationException($"RankClassificationId {dto.RankClassificationId.Value} belongs to a different faction.");
                }
                
                // Don't set next rank if classification doesn't have split lanes
                if (dto.NextRankId.HasValue && !classification.HasSplitRankLanes)
                {
                    // Silently ignore
                    dto.NextRankId = null;
                }
                
                dto.ApplyTo(entity);
                db.Ranks.Update(entity);
                updated.Add(entity);
                
                Debug.WriteLine(">" + dto.Id + " Updated");
            }

            Debug.WriteLine("Commiting Transaction");
            ts.Commit();
            return ServiceResult<List<Rank>>.Ok(updated, $"{updated.Count} Rank(s) updated.");
        }
        catch (Exception ex)
        {
            return ServiceResult<List<Rank>>.Fail(ex.Message);
        }
    }
}