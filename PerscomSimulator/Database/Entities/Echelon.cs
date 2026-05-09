using CrossLite;
using CrossLite.CodeFirst;
using System;

namespace Perscom.Database
{
    /// <summary>
    /// This entity represents an organizational level of <see cref="Soldier"/>s
    /// </summary>
    [Table]
    public class Echelon : EntityBase, IEquatable<Echelon>
    {
        #region Columns

        /// <summary>
        /// The IsUnique Echelon ID
        /// </summary>
        [Column, PrimaryKey]
        public virtual int Id { get; protected set; }

        /// <summary>
        /// Gets or Sets the string name of this Echelon
        /// </summary>
        [Column, Required]
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the hierarchy level of this Echelon. Higher value
        /// means higher up the organization chart
        /// </summary>
        [Column, Required]
        public virtual int HierarchyLevel { get; set; }

        #endregion

        #region Child Database Sets

        /// <summary>
        /// Gets a list of <see cref="UnitBlueprint"/> entities that reference this 
        /// <see cref="Echelon"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual EntitySet<UnitBlueprint> UnitBlueprints { get; set; }

        #endregion

        public bool Equals(Echelon other)
        {
            if (other == null) return false;
            return (Id == other.Id);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Echelon);
        }

        public override int GetHashCode() => Id;

        public override string ToString() => Name;
    }
}
