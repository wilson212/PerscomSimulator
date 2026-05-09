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
    
    #endregion
}