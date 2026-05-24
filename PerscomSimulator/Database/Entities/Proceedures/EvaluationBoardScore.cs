using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    [Table]
    public class EvaluationBoardScore : EntityBase
    {
        /// <summary>
        /// The unique identifier for this promotion board weight entry.
        /// </summary>
        [Column, Required, PrimaryKey]
        public virtual int Id { get; set; }
        
        /// <summary>
        /// The unique identifier for the related Evaluation board.
        /// </summary>
        /// <remarks>
        [Column, Required, Index]
        public virtual int EvaluationBoardId { get; set; }

        /// <summary>
        /// Specifies the selection criterion or attribute used for comparing a soldier's traits,
        /// position, or experience level during the promotion board evaluation process.
        /// </summary>
        [Column, Required]
        public virtual ClauseLeftSelector Selector { get; set; }

        /// <summary>
        /// Represents a specific function or criterion that is evaluated for a soldier during
        /// the promotion board process. This is used to determine whether a soldier meets
        /// a defined condition based on attributes such as time in service, rank, or position.
        /// </summary>
        [Column, Required]
        public virtual int SelectorId { get; set; }

        /// <summary>
        /// Represents the comparison operator used to evaluate a condition
        /// within a promotion board scoring process. Supports equality
        /// and relational checks such as "Equals" and "GreaterThan".
        /// </summary>
        [Column, Required]
        public virtual ComparisonOperator Operator { get; set; }
        
        /// <summary>
        /// The weight multiplier applied to this attribute's value when scoring candidates.
        /// Higher values mean this attribute matters more for this board.
        /// </summary>
        [Column, Required, Default(1)]
        public virtual int ExpectedLevel { get; set; } = 1;

        /// <summary>
        /// The amount of points this attribute contributes to the overall score.
        /// </summary>
        [Column, Required, Default(1)]
        public virtual int Points { get; set; } = 1;
        
        #region Foreign Key Navigation Properties

        /// <summary>
        /// Represents the promotion board associated with the PromotionBoardWeight entity.
        /// </summary>
        [ForeignKey(nameof(EvaluationBoardId))]
        [References(nameof(Database.EvaluationBoard.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade)]
        public virtual EvaluationBoard Board { get; set; }
    
        #endregion
    }
}