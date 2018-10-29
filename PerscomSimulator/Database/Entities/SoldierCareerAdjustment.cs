using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    /// <summary>
    /// Represents a (1:1 or 1:0) relationship between a <see cref="Database.SoldierGeneratorPool"/>
    /// and a <see cref="Database.CareerGenerator"/>
    /// </summary>
    /// <remarks>
    /// Used if there is a career length adjustment with the parent <see cref="Database.SoldierGeneratorPool"/>
    /// </remarks>
    [Table]
    public class SoldierCareerAdjustment
    {
        #region Columns

        /// <summary>
        /// The Unique Soldier Generator ID
        /// </summary>
        [Column, PrimaryKey]
        public int SoldierGeneratorPoolId { get; protected set; }

        /// <summary>
        /// The Unique Career Generator ID
        /// </summary>
        [Column]
        public int CareerGeneratorId { get; protected set; }

        #endregion

        #region Virtual Foreign Keys

        /// <summary>
        /// Gets the <see cref="SoldierGeneratorPool"/> entity that this entity references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("SoldierGeneratorPoolId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<SoldierGeneratorPool> FK_Pool { get; set; }

        /// <summary>
        /// Gets the <see cref="CareerGenerator"/> entity that this entity references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("CareerGeneratorId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<CareerGenerator> FK_Generator { get; set; }

        #endregion

        #region Foreign Key Properties

        /// <summary>
        /// Gets or sets the <see cref="Database.SoldierGeneratorPool"/> that 
        /// this entity references.
        /// </summary>
        public SoldierGeneratorPool SoldierGeneratorPool
        {
            get
            {
                return FK_Pool?.Fetch();
            }
            set
            {
                SoldierGeneratorPoolId = value.Id;
                FK_Pool?.Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Database.CareerGenerator"/> that 
        /// this entity references.
        /// </summary>
        public CareerGenerator CareerGenerator
        {
            get
            {
                return FK_Generator?.Fetch();
            }
            set
            {
                CareerGeneratorId = value.Id;
                FK_Generator?.Refresh();
            }
        }

        #endregion
    }
}
