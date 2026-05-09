using CrossLite;
using CrossLite.CodeFirst;
using Perscom.Simulation;

namespace Perscom.Database
{
    /// <summary>
    /// Represents a mapping between a Persona and a Trait in the database. This class
    /// establishes a many-to-many relationship between the Persona and PersonalityTrait
    /// entities and includes a probability weight for the relationship.
    /// </summary>
    [Table]
    public class PersonaTrait : EntityBase, IProbable
    {
        #region Columns

        /// <summary>
        /// The IsUnique SpawnRate ID (Row ID)
        /// </summary>
        [Column, PrimaryKey]
        public virtual int PersonaId { get; protected set; }

        /// <summary>
        /// The IsUnique Row ID
        /// </summary>
        [Column, PrimaryKey]
        public virtual int TraitId { get; set; }
        
        /// <summary>
        /// The Probability of this trait being applied to a soldier
        /// </summary>
        [Column, Required]
        public virtual int Probability { get; set; }
        
        /// <summary>
        /// Represents the duration of time, in simulation months, for which a personality trait
        /// is assigned to a soldier in the database. This value determines how
        /// long the relationship between the soldier and the trait is active. A negative value
        /// indicates that the trait is permanent.
        /// </summary>
        [Column, Required, Default(-1)]
        public virtual int Duration { get; set; } = -1;

        #endregion

        #region Foreign Key Navigation Properties

        /// <summary>
        /// Gets the <see cref="Persona"/> entity that this entity references.
        /// </summary>
        [ForeignKey(nameof(PersonaId))]
        [References(nameof(Database.Persona.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual Persona Persona { get; set; }

        /// <summary>
        /// Gets the <see cref="Database.PersonalityTrait"/> entity that this entity references.
        /// </summary>
        [ForeignKey(nameof(TraitId))]
        [References(nameof(Database.PersonalityTrait.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual PersonalityTrait Trait { get; set; }

        #endregion
    }
}
