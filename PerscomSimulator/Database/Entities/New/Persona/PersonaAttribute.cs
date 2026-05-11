using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    /// <summary>
    /// Represents the relationship between a Persona and its associated attributes.
    /// This class defines the minimum and maximum spawn values for a specific attribute
    /// associated with a Persona.
    /// </summary>
    [Table]
    public class PersonaAttribute : EntityBase
    {
        #region Columns

        /// <summary>
        /// The IsUnique SpawnRate ID (Row ID)
        /// </summary>
        [Column, PrimaryKey]
        public virtual int PersonaId { get; set; }

        /// <summary>
        /// The IsUnique Row ID
        /// </summary>
        [Column, PrimaryKey]
        public virtual AttributeType AttributeId { get; set; }

        /// <summary>
        /// Gets or sets the minimum spawn value a soldier will recieve using this
        /// <see cref="Database.Persona"/> for the provided Stat ID
        /// </summary>
        [Column, Required]
        public virtual int MinSpawnValue { get; set; } = 0;

        /// <summary>
        /// Gets or sets the maximim spawn value a soldier will recieve using this
        /// <see cref="Database.Persona"/> for the provided Stat ID
        /// </summary>
        [Column, Required]
        public virtual int MaxSpawnValue { get; set; } = 0;

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

        #endregion
    }
}
