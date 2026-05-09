using CrossLite;
using CrossLite.CodeFirst;
using Perscom.Collections;

namespace Perscom.Database
{
    /// <summary>
    /// Represents a mapping between a Soldier and a PersonalityTrait in the database.
    /// This entity establishes a many-to-many relationship, linking a soldier
    /// to their associated traits.
    /// </summary>
    public class SoldierTraitAttachment : EntityBase, IKeyed<int>
    {
        #region Columns

        /// <summary>
        /// The IsUnique SpawnRate ID (Row ID)
        /// </summary>
        [Column, PrimaryKey]
        public virtual int SoldierId { get; set; }

        /// <summary>
        /// The IsUnique Row ID
        /// </summary>
        [Column, PrimaryKey]
        public virtual int TraitId { get; set; }

        /// <summary>
        /// The date associated with the assignment of a trait to a soldier.
        /// </summary>
        [Column, Required]
        public virtual int AssignedOnIterationId { get; set; }

        /// <summary>
        /// Represents the duration of time, in simulation months, for which this personality trait
        /// is assigned to a soldier in the database. This value determines how
        /// long the relationship between the soldier and the trait is active. A negative value
        /// indicates that the trait is permanent.
        /// </summary>
        [Column, Required]
        public virtual int Duration { get; set; }

        #endregion

        #region Foreign Key Navigation Properties

        /// <summary>
        /// Gets the <see cref="Soldier"/> entity that this entity references.
        /// </summary>
        [ForeignKey(nameof(SoldierId))]
        [References(nameof(Database.Soldier.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual Soldier Soldier { get; set; }

        /// <summary>
        /// Gets the <see cref="Database.PersonalityTrait"/> entity that this entity references.
        /// </summary>
        [ForeignKey(nameof(TraitId))]
        [References(nameof(Database.PersonalityTrait.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual PersonalityTrait Trait { get; set; }
        
        /// <summary>
        /// Gets the <see cref="Database.IterationDate"/> entity that this entity references.
        /// </summary>
        [ForeignKey(nameof(AssignedOnIterationId))]
        [References(nameof(Database.IterationDate.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual IterationDate AssignedOn { get; set; }

        #endregion

        /// <summary>
        /// The unique identifier key for a <see cref="Perscom.Collections.IdentityList{TKey, T}"/>
        /// </summary>
        public int Key => TraitId;
    }
}