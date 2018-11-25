using CrossLite;
using CrossLite.CodeFirst;
using System;
using System.Collections.Generic;

namespace Perscom.Database
{
    [Table]
    public class Experience : IEquatable<Experience>
    {
        [Column, PrimaryKey]
        public int Id { get; set; }

        [Column, Unique, Collation(Collation.NoCase)]
        public string Name { get; set; }

        #region Child Database Sets

        /// <summary>
        /// Gets a list of <see cref="Database.SoldierExperience"/> entities that reference this 
        /// <see cref="Experience"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual IEnumerable<SoldierExperience> SoldierExperience { get; set; }

        /// <summary>
        /// Gets a list of <see cref="Database.SoldierExperience"/> entities that reference this 
        /// <see cref="Experience"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual IEnumerable<BilletExperience> BilletExperience { get; set; }

        /// <summary>
        /// Gets a list of <see cref="BilletSelectionSorting"/> entities that reference this 
        /// <see cref="Experience"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual IEnumerable<BilletSelectionSorting> BilletSorts { get; set; }

        /// <summary>
        /// Gets a list of <see cref="BilletSelectionFilter"/> entities that reference this 
        /// <see cref="Experience"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual IEnumerable<BilletSelectionFilter> BilletFilters { get; set; }

        #endregion

        public bool IsDuplicateOf(Experience other)
        {
            return (other.Id == Id || other.Name.Equals(Name, StringComparison.InvariantCultureIgnoreCase));
        }

        public bool Equals(Experience other)
        {
            if (other == null) return false;
            return (Id == other.Id);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Experience);
        }

        public override int GetHashCode() => Id;

        public override string ToString() => Name;
    }

    public class ExperienceComparer : IEqualityComparer<Experience>
    {

        public bool Equals(Experience x, Experience y)
        {
            //Check whether the objects are the same object. 
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether the products' properties are equal. 
            StringComparison c = StringComparison.InvariantCultureIgnoreCase;
            return x != null && y != null && x.Id.Equals(y.Id) && x.Name.Equals(y.Name, c);
        }

        public int GetHashCode(Experience obj)
        {
            // Get hash code for the Name field if it is not null. 
            int hashName = obj.Name == null ? 0 : obj.Name.GetHashCode();

            // Get hash code for the Id field. 
            int hashId = obj.Id.GetHashCode();

            //Calculate the hash code for the product. 
            return hashName ^ hashId;
        }
    }
}
