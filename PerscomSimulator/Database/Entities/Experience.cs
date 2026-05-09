using CrossLite;
using CrossLite.CodeFirst;
using System;
using System.Collections.Generic;

namespace Perscom.Database
{
    [Table]
    public class Experience : EntityBase, IEquatable<Experience>
    {
        [Column, PrimaryKey]
        public virtual int Id { get; protected set; }

        [Column, Unique, Collation(Collation.NoCase)]
        public virtual string Name { get; set; }

        #region Child Database Sets

        /// <summary>
        /// Gets a list of <see cref="Database.SoldierExperience"/> entities that reference this 
        /// <see cref="Experience"/>
        /// </summary>
        public virtual EntitySet<SoldierExperience> SoldierExperience { get; set; }

        /// <summary>
        /// Gets a list of <see cref="PositionBlueprintExperience"/> entities that reference this 
        /// <see cref="Experience"/>
        /// </summary>
        public virtual EntitySet<PositionBlueprintExperience> BilletExperience { get; set; }

        /// <summary>
        /// Gets a list of <see cref="SelectionSorting"/> entities that reference this 
        /// <see cref="Experience"/>
        /// </summary>
        public virtual EntitySet<SelectionSorting> BilletSorts { get; set; }

        /// <summary>
        /// Gets a list of <see cref="SelectionFilter"/> entities that reference this 
        /// <see cref="Experience"/>
        /// </summary>
        public virtual EntitySet<SelectionFilter> BilletFilters { get; set; }

        #endregion

        public bool IsDuplicateOf(Experience other)
        {
            if (other == null) return false;
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
            // Check whether the objects are the same object. 
            if (Object.ReferenceEquals(x, y)) return true;

            // Check whether the products' properties are equal. 
            StringComparison c = StringComparison.InvariantCultureIgnoreCase;
            return x != null && y != null && x.Id.Equals(y.Id) && x.Name.Equals(y.Name, c);
        }

        public int GetHashCode(Experience obj)
        {
            // Get hash code for the ColumnName field if it is not null. 
            int hashName = obj.Name == null ? 0 : obj.Name.GetHashCode();

            // Get hash code for the Id field. 
            int hashId = obj.Id.GetHashCode();

            // Calculate the hash code for the product. 
            return hashName ^ hashId;
        }
    }
}
