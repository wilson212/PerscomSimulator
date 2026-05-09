using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database;

/// <summary>
/// Represents the weight configuration for attributes associated with a promotion
/// board in the database.
/// </summary>
/// <remarks>
/// This class is used to define how much a particular attribute contributes
/// (via its Weight property) to the overall scoring for a promotion board.
/// Each instance is uniquely identified by the combination of
/// <see cref="PromotionBoardId"/> and <see cref="Attribute"/>.
/// The <see cref="PromotionBoard"/> navigation property allows access to the
/// related promotion board entity.
/// </remarks>
[Table(WithoutRowID = true)]
public class PromotionBoardWeight : EntityBase
{
    /// <summary>
    /// The unique identifier for the related promotion board.
    /// </summary>
    /// <remarks>
    /// This property establishes a relationship between the current promotion board weight entry and a specific
    /// promotion board in the database. It is used as a foreign key to reference the <see cref="PromotionBoard.Id"/>
    /// property, ensuring data consistency and enabling navigation between related entities. It is required
    /// for each entry in the <see cref="PromotionBoardWeight"/> table.
    /// </remarks>
    [Column, Required, PrimaryKey]
    public virtual int PromotionBoardId { get; set; }

    /// <summary>
    /// The soldier attribute this weight applies to (e.g., Leadership, Discipline)
    /// </summary>
    [Column, Required, PrimaryKey]
    public virtual AttributeType Attribute { get; set; }

    /// <summary>
    /// The weight multiplier applied to this attribute's value when scoring candidates.
    /// Higher values mean this attribute matters more for this board.
    /// </summary>
    [Column, Required, Default(1)]
    public virtual int Weight { get; set; }
    
    #region Foreign Key Navigation Properties

    /// <summary>
    /// Represents the promotion board associated with the PromotionBoardWeight entity.
    /// </summary>
    [ForeignKey(nameof(PromotionBoardId))]
    [References(nameof(Database.PromotionBoard.Id),
        OnDelete = ReferentialAction.Cascade,
        OnUpdate = ReferentialAction.Cascade)]
    public virtual PromotionBoard PromotionBoard { get; set; }
    
    #endregion
}