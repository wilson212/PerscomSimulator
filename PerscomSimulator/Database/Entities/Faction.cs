using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database;

/// <summary>
/// Represents a Faction entity in the application's database.
/// A faction is an organizational group with a unique identifier and distinct properties such as name,
/// theme color, short tag, description, and optional image representation.
/// </summary>
/// <remarks>
/// This class forms the basis for representing factions in the application database and
/// is annotated with attributes for database mapping and constraints. It inherits
/// from <see cref="EntityBase"/>, enabling integration with CrossLite and its CodeFirst features.
/// </remarks>
[Table]
public class Faction : EntityBase
{
    #region Columns

    /// <summary>
    /// The IsUnique PromotionBoard ID
    /// </summary>
    [Column, PrimaryKey]
    public virtual int Id { get; set; }

    /// <summary>
    /// The name of the faction, which is a required property and cannot be null.
    /// </summary>
    [Column, Required]
    public virtual string Name { get; set; }
    
    /// <summary>
    /// The color of the faction.
    /// </summary>
    [Column, Required]
    public virtual string ThemeColorCode { get; set; }
    
    /// <summary>
    /// The short tag of the faction.
    /// </summary>
    [Column, Required]
    public virtual string ShortTag { get; set; }

    /// <summary>
    /// The description of the faction.
    /// </summary>
    [Column, Required]
    public virtual string Description { get; set; }

    /// <summary>
    /// The relative image file path of the faction's flag, which can be null.
    /// </summary>
    [Column, Default("")]
    public virtual string Image { get; set; } = "";

    #endregion
}