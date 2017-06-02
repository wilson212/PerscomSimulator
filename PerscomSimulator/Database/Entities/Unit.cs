using System.Collections.Generic;
using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    /// <summary>
    /// Represents a structure that contains child <see cref="Unit"/>s and <see cref="Billet"/>s.
    /// </summary>
    [Table]
    public class Unit
    {
        #region Columns

        /// <summary>
        /// The Unique Unit ID
        /// </summary>
        [Column, PrimaryKey]
        public int Id { get; protected set; }

        /// <summary>
        /// Gets or Sets the <see cref="UnitType"/> object
        /// ID that this entity references
        /// </summary>
        [Column, Required]
        public int UnitTypeId { get; set; }

        /// <summary>
        /// Gets or Sets the string name of this Unit
        /// </summary>
        [Column, Required]
        public string Name { get; set; }

        #endregion

        #region Foreign Keys

        /// <summary>
        /// Gets the <see cref="UnitType"/> entity that this entity references.
        /// </summary>
        /// <remarks>Eager loaded because it should never be changed!</remarks>
        [InverseKey("Id")]
        [ForeignKey("UnitTypeId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        public virtual UnitType Type { get; private set; }

        #endregion

        #region Child Database Sets

        /// <summary>
        /// Gets a list of <see cref="UnitAttachment"/> entities that reference this 
        /// <see cref="Unit"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration that fetches all Torque Ratios
        /// that are bound by the foreign key and this Unit.Id.
        /// </remarks>
        public virtual IEnumerable<UnitAttachment> Attachments { get; set; }

        #endregion
    }
}
