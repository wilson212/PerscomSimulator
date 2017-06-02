using System.Collections.Generic;
using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    /// <summary>
    /// Represents a position within a <see cref="Unit"/> that a 
    /// <see cref="Database.Soldier"/> will occupy.
    /// </summary>
    [Table]
    public class Position
    {
        #region Columns

        /// <summary>
        /// The Unique Position ID
        /// </summary>
        [Column, PrimaryKey]
        public int Id { get; protected set; }

        /// <summary>
        /// Gets or Sets the <see cref="Database.Billet"/> object
        /// ID that this entity references
        /// </summary>
        [Column, Required]
        public int BilletId { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="Database.Unit"/> this position
        /// is attached to
        /// </summary>
        [Column, Required]
        public int UnitId { get; set; }

        /// <summary>
        /// Gets or Sets the string name of this Unit
        /// </summary>
        [Column, Required]
        public string Name { get; set; }

        #endregion

        #region Foreign Keys

        /// <summary>
        /// Gets the <see cref="Database.Billet"/> entity that this position references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("BilletId",
             OnDelete = ReferentialIntegrity.Cascade,
             OnUpdate = ReferentialIntegrity.Cascade
         )]
        protected virtual ForeignKey<Billet> FK_Billet { get; set; }

        /// <summary>
        /// Gets the <see cref="Database.Unit"/> entity that this position is attached to.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("UnitId",
             OnDelete = ReferentialIntegrity.Cascade,
             OnUpdate = ReferentialIntegrity.Cascade
         )]
        protected virtual ForeignKey<Unit> FK_Unit { get; set; }

        #endregion


        #region Foreign Key Properties

        /// <summary>
        /// Gets the <see cref="Database.Billet"/> entity that this position references.
        /// </summary>
        public Billet Billet
        {
            get
            {
                return FK_Billet?.Fetch();
            }
            set
            {
                BilletId = value.Id;
                FK_Billet?.Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Database.Unit"/> entity that this position is attached to.
        /// </summary>
        public Unit Unit
        {
            get
            {
                return FK_Unit?.Fetch();
            }
            set
            {
                UnitId = value.Id;
                FK_Unit?.Refresh();
            }
        }

        #endregion

        #region Child Database Sets

        /// <summary>
        /// Gets a list of <see cref="Assignment"/> entities that reference this 
        /// <see cref="Position"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration that fetches all Torque Ratios
        /// that are bound by the foreign key and this Engine.Id.
        /// </remarks>
        public virtual IEnumerable<Assignment> Assignments { get; set; }

        /// <summary>
        /// Gets a list of <see cref="PastAssignment"/> entities that reference this 
        /// <see cref="Position"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration that fetches all Torque Ratios
        /// that are bound by the foreign key and this Engine.Id.
        /// </remarks>
        public virtual IEnumerable<PastAssignment> PastAssignments { get; set; }

        #endregion
    }
}
