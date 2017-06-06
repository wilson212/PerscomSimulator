using System;
using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    [Table]
    public class BilletCatagory : IEquatable<BilletCatagory>
    {
        #region Columns

        /// <summary>
        /// The Unique BilletCatagory ID
        /// </summary>
        [Column, PrimaryKey]
        public int Id { get; protected set; }

        /// <summary>
        /// Gets or Sets the string name of this catagory
        /// </summary>
        [Column, Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the ordering of this catagory. Higher value
        /// means higher up the ladder.
        /// </summary>
        [Column, Required]
        public int ZIndex { get; set; }

        #endregion

        public bool Equals(BilletCatagory other)
        {
            if (other == null) return false;
            return (Id == other.Id);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as BilletCatagory);
        }

        public override int GetHashCode() => Id;

        public override string ToString() => Name;
    }
}
