using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    /// <summary>
    /// Represents the effect associated with a specific personality trait.
    /// This class defines how a personality trait modifies a target attribute within the system.
    /// </summary>
    [Table]
    public class TraitEffect : EntityBase
    {
        /// <summary>
        /// Serves as the primary key in the database table associated with this entity.
        /// </summary>
        [Column, PrimaryKey]
        public virtual int Id { get; set; }

        /// <summary>
        /// Represents the foreign key association to the PersonalityTrait entity,
        /// linking this entity to a specific personality trait.
        /// </summary>
        [Column, Required]
        public virtual int PersonalityTraitId { get; set; }

        /// <summary>
        /// The attribute this trait modifies (e.g., "Leadership", "Ambition")
        /// </summary>
        [Column, Required]
        public virtual AttributeType TargetAttribute { get; set; }

        /// <summary>
        /// The modifier value (positive = buff, negative = debuff)
        /// </summary>
        [Column, Required]
        public virtual int Modifier { get; set; }
        
        #region Foreign Key Navigation Properties

        [ForeignKey(nameof(PersonalityTraitId))]
        [References(nameof(Database.PersonalityTrait.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade)]
        public virtual PersonalityTrait PersonalityTrait { get; set; }
        
        #endregion
    }
}