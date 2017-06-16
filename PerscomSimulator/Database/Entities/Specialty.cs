using System;
using System.Collections.Generic;
using CrossLite;
using CrossLite.CodeFirst;
using Perscom.Simulation;

namespace Perscom.Database
{
    /// <summary>
    /// Represents a series of attributes to identify a specific job
    /// </summary>
    [Table]
    public class Specialty : IEquatable<Specialty>
    {
        #region Columns

        /// <summary>
        /// The Unique Specialty ID (Row ID)
        /// </summary>
        [Column, PrimaryKey]
        public int Id { get; protected set; }

        /// <summary>
        /// Gets or sets the code of this Occupational Specialty
        /// </summary>
        [Column, Required, Unique, Collation(Collation.NoCase)]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the string name of this Occupational Specialty
        /// </summary>
        [Column, Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the grade of this rank
        /// </summary>
        [Column, Required]
        public RankType Type { get; set; }

        #endregion

        #region Child Database Sets

        /// <summary>
        /// Gets the <see cref="BilletSpecialty"/> entity that reference this 
        /// <see cref="Specialty"/>, if any
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual IEnumerable<BilletSpecialty> Billets { get; set; }

        #endregion

        public bool Equals(Specialty other)
        {
            if (other == null) return false;
            return (Id == other.Id || Code == other.Code);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Specialty);
        }

        public override int GetHashCode() => Id;

        public override string ToString() => $"{Name} ({Code})";
    }
}
