using System;
using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    /// <summary>
    /// Represents a career length entity stored in the database. The data here will be used in a
    /// standard Gaussian (Normal) Distribution curge to generate a random career length for a 
    /// <see cref="Database.Soldier"/>
    /// </summary>
    [Table]
    public class CareerLength : EntityBase, IEquatable<CareerLength>
    {
        #region Columns

        /// <summary>
        /// The IsUnique Career ID (Row ID)
        /// </summary>
        [Column, PrimaryKey]
        public virtual int Id { get; protected set; }

        /// <summary>
        /// Gets or sets the string name of this generator
        /// </summary>
        [Column, Required]
        public virtual string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the average months of this career length.
        /// </summary>
        [Column, Required]
        public virtual int AverageMonths { get; set; }
        
        /// <summary>
        /// Gets or sets the minimum months of this career length.
        /// </summary>
        [Column, Required]
        public virtual int MinimumMonths { get; set; }
        
        /// <summary>
        /// Gets or sets the maximum months of this career length.
        /// </summary>
        [Column, Required]
        public virtual int MaximumMonths { get; set; }
        
        /// <summary>
        /// Gets or sets the skew right of this career length.
        /// </summary>
        [Column, Required]
        public virtual double SkewRight { get; set; }
        
        /// <summary>
        /// Gets or sets the skew left of this career length.
        /// </summary>
        [Column, Required]
        public virtual double SkewLeft { get; set; }

        #endregion

        public override bool Equals(object obj)
        {
            return Equals(obj as CareerLength);
        }

        public bool Equals(CareerLength other)
        {
            if (other == null) return false;
            return (Id == other.Id);
        }

        public override int GetHashCode() => Id;

        public override string ToString() => Name;
    }
}
