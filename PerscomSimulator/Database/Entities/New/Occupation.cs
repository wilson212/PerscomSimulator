using CrossLite;
using CrossLite.CodeFirst;
using Perscom.Simulation;
using System;
using System.Collections.Generic;

namespace Perscom.Database
{
    /// <summary>
    /// This entity is used to store details regarding a specific occupation, including its unique identifier,
    /// code, name, and associated rank type.
    /// </summary>
    [Table]
    public class Occupation : EntityBase, IEquatable<Occupation>
    {
        #region Columns

        /// <summary>
        /// The IsUnique Occupation ID (Row ID)
        /// </summary>
        [Column, PrimaryKey]
        public virtual int Id { get; protected set; }
        
        /// <summary>
        /// Gets or sets the <see cref="Faction.Id"/> this entity references
        /// </summary>
        [Column, Required]
        public virtual int FactionId { get; set; }

        /// <summary>
        /// Gets or sets the code of this Occupational Occupation
        /// </summary>
        [Column, Required, Unique, Collation(Collation.NoCase)]
        public virtual string Code { get; set; }

        /// <summary>
        /// Gets or sets the string name of this Occupational Occupation
        /// </summary>
        [Column, Required]
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the grade of this rank
        /// </summary>
        [Column, Required]
        public virtual RankType Type { get; set; }

        #endregion
        
        #region Parent Database Sets
        
        /// <summary>
        /// Represents the faction associated with the occupation.
        /// This property establishes a foreign key relationship to the <see cref="Faction"/> table
        /// to ensure referential integrity within the database.
        /// </summary>
        [ForeignKey(nameof(FactionId))]
        [References(nameof(Database.Faction.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual Faction Faction { get; set; }
        
        #endregion

        #region Child Database Sets

        /// <summary>
        /// Gets the <see cref="PromotionBoard"/> entities that reference this 
        /// <see cref="Occupation"/>, if any
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual IEnumerable<PromotionBoard> PromotionBoards { get; set; }

        #endregion

        public bool Equals(Occupation other)
        {
            if (other == null) return false;
            return (Id == other.Id || Code == other.Code);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Occupation);
        }

        public override int GetHashCode() => Id;

        public override string ToString() => $"{Code} - {Name}";
    }
}
