using CrossLite;
using CrossLite.CodeFirst;
using CrossLite.QueryBuilder;
using System;

namespace Perscom.Database
{
    public abstract class AbstractSort : IEquatable<AbstractSort>
    {
        /// <summary>
        /// Indicates the order or priority this sorting is applied
        /// </summary>
        [Column, PrimaryKey]
        public int Precedence { get; set; }

        /// <summary>
        /// Gets or sets the selector method for the LEFT value
        /// </summary>
        [Column, Required]
        public ClauseLeftSelector Selector { get; set; }

        /// <summary>
        /// Gets or sets the Identifier value of the <see cref="ClauseLeftSelector"/>
        /// to get the LEFT value from.
        /// </summary>
        [Column, Required]
        public int SelectorId { get; set; }

        /// <summary>
        /// The sorting direction
        /// </summary>
        [Column, Required, Default(0)]
        public Sorting Direction { get; set; }

        /// <summary>
        /// Compares a <see cref="AbstractSort"/> with this one, and returns whether
        /// the identifying properties match
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsDuplicateOf(AbstractSort other)
        {
            return (
                Selector == other.Selector
                && SelectorId == other.SelectorId
                && Direction == other.Direction
            );
        }

        public bool Equals(AbstractSort other)
        {
            if (other == null) return false;
            return (this.IsDuplicateOf(other));
        }
    }
}
