using System.Collections.Generic;
using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    /// <summary>
    /// Represents an abstract set of rules that child <see cref="Position"/>'s will inherit
    /// </summary>
    [Table]
    public class Billet
    {
        #region Columns

        /// <summary>
        /// The Unique Billet ID
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
        /// Gets or Sets the <see cref="Rank.Id"/> the <see cref="Soldier"/>
        /// holding this billet should be.
        /// </summary>
        [Column, Required]
        public int RankId { get; set; }

        /// <summary>
        /// Gets or Sets the string name of this Unit
        /// </summary>
        [Column, Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the prominence level of the position. A higher value will cause
        /// this <see cref="Billet"/> to be filled with more experienced (time in grade) 
        /// soldiers, while lower values will be filled with more inexperienced soldiers.
        /// </summary>
        /// <remarks>
        /// Entry level billits should be set to a value of zero, while positions that require
        /// seniority should have a higher value.
        /// </remarks>
        [Column, Default(0)]
        public int Stature { get; set; } = 0;

        /// <summary>
        /// Gets or sets the minimum time (months) a soldier must hold this <see cref="Billet"/> 
        /// before being allowed to leave. If the minimum amount is less than the remaining time 
        /// to live for the soldier, their retirement date will be adjusted accordingly.
        /// </summary>
        [Column, Default(0)]
        public int MinTourLength { get; set; } = 0;

        /// <summary>
        /// Gets or sets the maximum time (months) a soldier can hold this <see cref="Billet"/>
        /// before being forcefully pushed out.
        /// </summary>
        [Column, Default(0)]
        public int MaxTourLength { get; set; } = 0;

        /// <summary>
        /// Indicates whether the soldier holding this <see cref="Billet"/> can retire before
        /// meeting the <see cref="MinTourLength"/>.
        /// </summary>
        [Column, Required, Default(1)]
        public bool CanRetireEarly { get; set; } = true;

        #endregion

        #region Virtual Foreign Keys

        [InverseKey("Id")]
        [ForeignKey("UnitTypeId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<UnitType> FK_Parent { get; set; }

        /// <summary>
        /// Gets the <see cref="Rank"/> entity that this entity references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("RankId",
            OnDelete = ReferentialIntegrity.Restrict,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<Rank> FK_Rank { get; set; }

        #endregion

        #region Foreign Key Properties

        /// <summary>
        /// Gets or Sets the <see cref="Perscom.Database.UnitType"/> that 
        /// this Billit is attached to.
        /// </summary>
        public UnitType UnitType
        {
            get
            {
                return FK_Parent?.Fetch();
            }
            set
            {
                UnitTypeId = value.Id;
                FK_Parent?.Refresh();
            }
        }

        /// <summary>
        /// Gets or Sets the <see cref="Perscom.Database.Rank"/> that 
        /// this position will hold.
        /// </summary>
        public Rank Rank
        {
            get
            {
                return FK_Rank?.Fetch();
            }
            set
            {
                RankId = value.Id;
                FK_Rank?.Refresh();
            }
        }

        #endregion

        #region Child Database Sets

        /// <summary>
        /// Gets a list of <see cref="Position"/> entities that reference this 
        /// <see cref="Billet"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration that fetches all Torque Ratios
        /// that are bound by the foreign key and this Engine.Id.
        /// </remarks>
        public virtual IEnumerable<Position> Positions { get; set; }

        #endregion
    }
}
