using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    /// <summary>
    /// This entity representsa (1:1 or 1:0) relationship between a 
    /// <see cref="Perscom.Database.Soldier"/> and a <see cref="Unit"/>
    /// </summary>
    /// <remarks>
    /// Soldiers that are considered Retired will not be attached to a Unit,
    /// hence being a 1:1 or 1:0 relationship!
    /// </remarks>
    [Table]
    [CompositeUnique(nameof(SoldierId), nameof(UnitId))]
    public class SoldierUnitAttachment
    {
        #region Column Properties

        /// <summary>
        /// The Unique Soldier ID
        /// </summary>
        [Column, PrimaryKey]
        public int Id { get; protected set; }

        /// <summary>
        /// Gets or sets the parent <see cref="Soldier.Id"/>
        /// </summary>
        [Column, Required]
        public int SoldierId { get; set; }

        /// <summary>
        /// Gets or sets the parent <see cref="Unit.Id"/>
        /// </summary>
        [Column, Required]
        public int UnitId { get; set; }

        #endregion

        #region Virtual Foreign Keys

        [InverseKey("Id")]
        [ForeignKey("SoldierId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<Soldier> FK_Soldier { get; set; }

        [InverseKey("Id")]
        [ForeignKey("UnitId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<Unit> FK_Unit { get; set; }

        #endregion
    }
}