using System;
using System.Collections.Generic;
using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    [Table]
    public class Echelon : IEquatable<Echelon>
    {
        #region Columns

        /// <summary>
        /// The Unique Echelon ID
        /// </summary>
        [Column, PrimaryKey]
        public int Id { get; protected set; }

        /// <summary>
        /// Gets or Sets the string name of this Echelon
        /// </summary>
        [Column, Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the hierarchy level of this Echelon. Higher value
        /// means higher up the ladder.
        /// </summary>
        [Column, Required]
        public int HierarchyLevel { get; set; }

        #endregion

        #region Child Database Sets

        /// <summary>
        /// Gets a list of <see cref="UnitTemplate"/> entities that reference this 
        /// <see cref="Echelon"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration that fetches all Torque Ratios
        /// that are bound by the foreign key and this Engine.Id.
        /// </remarks>
        public virtual IEnumerable<UnitTemplate> UnitTypes { get; set; }

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
