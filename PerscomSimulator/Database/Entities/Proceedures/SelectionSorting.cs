using CrossLite;
using CrossLite.CodeFirst;
using System;
using CrossLite.QueryBuilder;

namespace Perscom.Database
{
    /// <summary>
    /// Represents a per-billet sorting rule that orders candidate soldiers
    /// within their group during the selection procedure pipeline.
    /// </summary>
    [Table(WithoutRowID = true)]
    public class SelectionSorting : EntityBase, IEquatable<SelectionSorting>
    {
        #region Columns

        /// <summary>
        /// Gets or Sets the <see cref="PositionBlueprint.Id"/> that this entity references
        /// </summary>
        [Column, PrimaryKey]
        public virtual int SelectionProcedureId { get; set; }
        
        /// <summary>
        /// Indicates the order or priority this sorting is applied
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
        public virtual int SelectorId { get; set; }

        /// <summary>
        /// The sorting direction
        /// </summary>
        [Column, Required, Default(0)]
        public virtual Sorting Direction { get; set; }

        #endregion

        public bool IsDuplicateOf(SelectionSorting other)
        {
            return (Selector == other.Selector
                && SelectorId == other.SelectorId
                && Direction == other.Direction
            );
        }

        public bool Equals(SelectionSorting other)
        {
            if (other == null) return false;
            return (this.IsDuplicateOf(other));
        }
    }
}
