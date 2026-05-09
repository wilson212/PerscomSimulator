using CrossLite;
using CrossLite.CodeFirst;
namespace Perscom.Database;

/// <summary>
/// Represents the monthly record of a soldier's performance, including attributes
/// such as morale, burnout, and form rating for a specific month and iteration.
/// </summary>
[Table(WithoutRowID: true)]
[CompositeUnique(nameof(SoldierId), nameof(MonthIndex))]
public class SoldierMonthlyRecord : EntityBase
{
    /// <summary>
    /// Represents the unique identifier associated with a specific soldier.
    /// Used to establish relationships between the soldier and their monthly records
    /// in the database.
    /// </summary>
    [Column, Required, PrimaryKey]
    public virtual int SoldierId { get; set; }

    /// <summary>
    /// Ring buffer index (0-11). Calculated as IterationId % 12.
    /// Overwrites itself every 12 months, keeping only the latest year.
    /// </summary>
    [Column, Required, PrimaryKey]
    public virtual int MonthIndex { get; set; }

    /// <summary>
    /// The iteration this record was written on (for save/load context).
    /// </summary>
    [Column, Required]
    public virtual int IterationId { get; set; }

    /// <summary>
    /// Represents the morale level of a soldier for a specific monthly record.
    /// Defaults to a value of 50 if not explicitly set. It is a required property and
    /// is associated with the soldier's performance and well-being during the specified time period.
    /// </summary>
    [Column, Required]
    public virtual int Morale { get; set; }

    /// <summary>
    /// Represents the level of fatigue or stress accumulated by a soldier during a given month.
    /// Defaults to 0. Higher values may indicate increased strain or overwork.
    /// </summary>
    [Column, Required]
    public virtual int Burnout { get; set; }

    /// <summary>
    /// Represents a soldier's performance rating in a given month.
    /// </summary>
    [Column, Required]
    public virtual int FormRating { get; set; } // Stored as int (1.0-9.9 * 10 = 10-99)

    #region Foreign Key Navigation Properties
    
    [ForeignKey(nameof(SoldierId))]
    [References(nameof(Database.Soldier.Id),
        OnDelete = ReferentialAction.Cascade,
        OnUpdate = ReferentialAction.Cascade)]
    public virtual Soldier Soldier { get; set; }
    
    #endregion
}