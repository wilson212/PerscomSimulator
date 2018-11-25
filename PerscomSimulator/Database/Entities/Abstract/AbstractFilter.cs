using CrossLite;
using CrossLite.CodeFirst;
using System;

namespace Perscom.Database
{
    public abstract class AbstractFilter : IEquatable<AbstractFilter>
    {
        /// <summary>
        /// Indicates the order or priority this condition is applied
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
        /// Gets or sets the comparison operator
        /// </summary>
        [Column, Required]
        public ComparisonOperator Operator { get; set; }

        /// <summary>
        /// The condition value
        /// </summary>
        [Column, Required, Default(0)]
        public int RightValue { get; set; }

        /// <summary>
        /// Compares a <see cref="AbstractFilter"/> with this one, and returns whether
        /// the identifying properties match
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsDuplicateOf(AbstractFilter other)
        {
            return (
                Selector == other.Selector
                && SelectorId == other.SelectorId
                && Operator == other.Operator
                && RightValue == other.RightValue
            );
        }

        public bool Equals(AbstractFilter other)
        {
            if (other == null) return false;
            return (this.IsDuplicateOf(other));
        }
    }
}
