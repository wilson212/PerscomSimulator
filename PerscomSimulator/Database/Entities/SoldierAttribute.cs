using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database;

[Table]
public class SoldierAttribute : EntityBase
{
    /// <summary>
    /// The IsUnique Soldier ID (Row ID)
    /// </summary>
    [Column, Required, PrimaryKey]
    public virtual int SoldierId { get; set; }

    /// <summary>
    /// The IsUnique Attribute ID
    /// </summary>
    [Column, Required, PrimaryKey]
    public virtual AttributeType Attribute { get; set; }

    /// <summary>
    /// Represents the value of a soldier's attribute. This property is required
    /// and defaults to 0 if no specific value is provided.
    /// </summary>
    [Column, Required, Default(0)]
    public virtual int Value { get; set; }

    /// <summary>
    /// Represents the total accumulated experience points for a soldier's attribute.
    /// This value determines the soldier's progression within the attribute and is
    /// typically adjusted during skill updates and performance evaluations.
    /// </summary>
    [Column, Required, Default(0)]
    public virtual int TotalExperience { get; set; }

    #region Foreign Key Navigation Properties

    /// <summary>
    /// Gets the <see cref="Database.Soldier"/> entity that this entity references.
    /// </summary>
    [ForeignKey(nameof(SoldierId))]
    [References(nameof(Database.Soldier.Id),
        OnDelete = ReferentialAction.Cascade,
        OnUpdate = ReferentialAction.Cascade)
    ]
    public virtual Soldier Soldier { get; set; }

    #endregion
}