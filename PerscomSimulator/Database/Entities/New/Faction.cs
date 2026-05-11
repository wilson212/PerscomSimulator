using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database;

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