using System;
using Perscom.AI.Dtos;
using Perscom.Database;

namespace Perscom.Services;

/// <summary>
/// Provides shared CRUD operations for <see cref="Faction"/> entities.
/// Both the WinForms UI and <c>AIFunctionHandler</c> should call these methods
/// instead of duplicating database logic.
/// </summary>
public static class FactionService
{
    /// <summary>
    /// Applies a partial update to an existing <see cref="Faction"/>.
    /// Only non-null parameters are written; all others are left unchanged.
    /// </summary>
    /// <param name="factionId">The primary key of the faction to update.</param>
    /// <returns>
    /// A <see cref="ServiceResult{T}"/> containing the updated <see cref="Faction"/> on success,
    /// or an error message if the faction was not found.
    /// </returns>
    public static ServiceResult<Faction> Update(int factionId, UpdateFactionDto dto)
    {
        try
        {
            using var db = new AppDatabase();
            var faction = db.Factions.Find(factionId);
            if (faction == null)
                return ServiceResult<Faction>.Fail($"Faction with Id {factionId} not found.");

            dto.ApplyTo(faction);

            db.Factions.Update(faction);
            return ServiceResult<Faction>.Ok(faction, $"Faction '{faction.Name}' (Id={faction.Id}) updated.");
        }
        catch (Exception ex)
        {
            return ServiceResult<Faction>.Fail(ex.Message);
        }
    }

    /// <summary>
    /// Creates a new <see cref="Faction"/> entity with the specified attributes and adds it to the database.
    /// </summary>
    /// <param name="name">The name of the faction. Can be null to use a default value.</param>
    /// <param name="shortTag">The short tag or abbreviation for the faction. Can be null to use a default value.</param>
    /// <param name="description">A description of the faction. Can be null to use a default value.</param>
    /// <param name="themeColorCode">The hex color code representing the theme of the faction. Can be null to use a default value.</param>
    /// <returns>
    /// A <see cref="ServiceResult{T}"/> containing the newly created <see cref="Faction"/> on success,
    /// or an error message if the creation process fails.
    /// </returns>
    public static ServiceResult<Faction> Create(
        string name = null,
        string shortTag = null,
        string description = null,
        string themeColorCode = null)
    {
        try
        {
            using var db = new AppDatabase();
            var faction = db.Factions.Create();

            if (name != null) faction.Name = name;
            if (shortTag != null) faction.ShortTag = shortTag;
            if (description != null) faction.Description = description;
            if (themeColorCode != null) faction.ThemeColorCode = themeColorCode;

            db.Factions.Add(faction);
            return ServiceResult<Faction>.Ok(faction, $"Faction '{faction.Name}' added with (Id={faction.Id}).");
        }
        catch (Exception ex)
        {
            return ServiceResult<Faction>.Fail(ex.Message);
        }
    }

    /// <summary>
    /// Deletes an existing <see cref="Faction"/> from the database.
    /// If the faction does not exist, an error message is returned.
    /// </summary>
    /// <param name="factionId">The primary key of the faction to delete.</param>
    /// <returns>
    /// A <see cref="ServiceResult{T}"/> containing the deleted <see cref="Faction"/> on success,
    /// or an error message if the faction was not found or another issue occurred.
    /// </returns>
    public static ServiceResult<Faction> Delete(int factionId)
    {
        try
        {
            using var db = new AppDatabase();
            var faction = db.Factions.Find(factionId);
            if (faction == null)
                return ServiceResult<Faction>.Fail($"Faction with Id {factionId} not found.");

            db.Factions.Remove(faction);
            return ServiceResult<Faction>.Ok(faction, $"Faction '{faction.Name}' has been deleted.");
        }
        catch (Exception ex)
        {
            return ServiceResult<Faction>.Fail(ex.Message);
        }
    }
}