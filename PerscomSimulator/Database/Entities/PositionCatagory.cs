using System;
using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    [Table]
    public class PositionCatagory : EntityBase, IEquatable<PositionCatagory>
    {
        #region Columns

        /// <summary>
        /// The IsUnique PositionCatagory ID
        /// </summary>
        [Column, PrimaryKey]
        public virtual int Id { get; protected set; }

        /// <summary>
        /// Gets or Sets the string name of this catagory
        /// </summary>
        [Column, Required]
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the ordering of this catagory. Higher value
        /// means higher up the ladder.
        /// </summary>
        [Column, Required]
        public virtual int ZIndex { get; set; }

        #endregion

        public bool Equals(PositionCatagory other)
        {
            if (other == null) return false;
            return (Id == other.Id);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as PositionCatagory);
        }

        public override int GetHashCode() => Id;

        public override string ToString() => Name;
    }
}
