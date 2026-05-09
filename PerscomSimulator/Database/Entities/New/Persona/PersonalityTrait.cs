using CrossLite;
using CrossLite.CodeFirst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perscom.Database
{
    /// <summary>
    /// Represents a personality trait entity within the database.
    /// </summary>
    [Table]
    public class PersonalityTrait : EntityBase
    {
        #region Columns

        /// <summary>
        /// The IsUnique Row ID
        /// </summary>
        [Column, PrimaryKey]
        public virtual int Id { get; set; }

        /// <summary>
        /// The name of the personality trait.
        /// </summary>
        [Column, Required]
        public virtual string Name { get; set; }
        
        /// <summary>
        /// Traits sharing the same non-null ExclusionGroup cannot coexist on the same soldier.
        /// Null means the trait has no conflicts.
        /// </summary>
        [Column]
        public virtual int? ExclusionGroup { get; set; }

        #endregion
        
        #region Child Navigation Properties
        
        /// <summary>
        /// Gets a list of <see cref="TraitEffect"/> entities that reference this <see cref="PersonalityTrait"/>
        /// </summary>
        public virtual EntitySet<TraitEffect> TraitEffects { get; set; }
        
        /// <summary>
        /// Gets a list of <see cref="SoldierTraits"/> entities that reference this <see cref="PersonalityTrait"/>
        /// </summary>
        public virtual EntitySet<SoldierTraitAttachment> SoldierTraits { get; set; }
        
        #endregion
    }
}
