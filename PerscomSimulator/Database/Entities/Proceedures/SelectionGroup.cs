using CrossLite;
using CrossLite.CodeFirst;
using System;

namespace Perscom.Database
{
    /// <summary>
    /// Represents a per-billet grouping rule that buckets candidate soldiers
    /// into priority tiers during the selection procedure pipeline.
    /// </summary>
    [Table(WithoutRowID = true)]
    public class SelectionGroup : EntityBase, IEquatable<SelectionGroup>
    {
        #region Columns

        /// <summary>
        /// Gets or Sets the <see cref="PositionBlueprint.Id"/> that this entity references
        /// </summary>
        [Column, PrimaryKey]
        public virtual int EvaluationBoardId { get; set; }
        
        /// <summary>
        /// Indicates the order or priority this condition is applied
        /// </summary>
        [Column, PrimaryKey]
        public virtual int Precedence { get; set; }

        /// <summary>
        /// Gets or sets the selector method for the LEFT value
        /// </summary>
        [Column, Required]
        public virtual ClauseLeftSelector Selector { get; set; }

        /// <summary>
        /// Gets or sets the Identifier value of the <see cref="ClauseLeftSelector"/>
        /// to get the LEFT value from.
        /// </summary>
        [Column, Required]
        public virtual  int SelectorId { get; set; }

        /// <summary>
        /// Gets or sets the comparison operator
        /// </summary>
        [Column, Required]
        public virtual ComparisonOperator Operator { get; set; }

        /// <summary>
        /// The condition value
        /// </summary>
        [Column, Required, Default(0)]
        public virtual int RightValue { get; set; }

        #endregion
        
        #region Foreign Key Navigation Properties
        
        [ForeignKey(nameof(EvaluationBoardId))]
        [References(nameof(EvaluationBoard.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade)]
        public virtual EvaluationBoard EvaluationBoard { get; set; }
        
        #endregion

        public bool IsDuplicateOf(SelectionGroup other)
        {
            return (Selector == other.Selector
                && SelectorId == other.SelectorId
                && Operator == other.Operator
                && RightValue == other.RightValue
            );
        }

        public bool Equals(SelectionGroup other)
        {
            if (other == null) return false;
            return (this.IsDuplicateOf(other));
        }
    }
}
