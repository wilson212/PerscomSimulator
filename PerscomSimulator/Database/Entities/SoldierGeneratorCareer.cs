using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    /// <summary>
    /// Represents a (1:1 or 1:0) relationship between a <see cref="Database.SoldierGenerator"/>
    /// and a <see cref="Database.CareerGenerator"/>
    /// </summary>
    /// <remarks>
    /// Used if there is a probability of new <see cref="Database.Soldier"/> creation in the parent 
    /// <see cref="Database.SoldierGenerator"/>, as we will need to generate a career length for
    /// that new soldier!
    /// </remarks>
    [Table]
    public class SoldierGeneratorCareer
    {
        #region Columns

        /// <summary>
        /// The Unique Soldier Generator ID
        /// </summary>
        [Column, PrimaryKey]
        public int SoldierGeneratorId { get; protected set; }

        /// <summary>
        /// The Unique Career Generator ID
        /// </summary>
        [Column]
        public int CareerGeneratorId { get; protected set; }

        #endregion

        #region Virtual Foreign Keys

        /// <summary>
        /// Gets the <see cref="SoldierGenerator"/> entity that this entity references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("SoldierGeneratorId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<SoldierGenerator> FK_Soldier { get; set; }

        /// <summary>
        /// Gets the <see cref="CareerGenerator"/> entity that this entity references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("CareerGeneratorId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<CareerGenerator> FK_Career { get; set; }

        #endregion

        #region Foreign Key Properties

        /// <summary>
        /// Gets or sets the <see cref="Database.SoldierGenerator"/> that 
        /// this entity references.
        /// </summary>
        public SoldierGenerator SoldierGenerator
        {
            get
            {
                return FK_Soldier?.Fetch();
            }
            set
            {
                SoldierGeneratorId = value.Id;
                FK_Soldier?.Refresh();
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
                return FK_Career?.Fetch();
            }
            set
            {
                CareerGeneratorId = value.Id;
                FK_Career?.Refresh();
            }
        }

        #endregion
    }
}
