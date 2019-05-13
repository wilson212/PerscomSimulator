using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    /// <summary>
    /// Represents a (1:1 or 1:0) relationship between a <see cref="Database.RandomizedPool"/>
    /// and a <see cref="Database.CareerGenerator"/>
    /// </summary>
    /// <remarks>
    /// Used if there is a career length adjustment with the parent <see cref="Database.RandomizedPool"/>
    /// </remarks>
    [Table]
    public class RandomizedPoolCareer
    {
        #region Columns

        /// <summary>
        /// The Unique Soldier Generator ID
        /// </summary>
        [Column, PrimaryKey]
        public int RandomizedPoolId { get; protected set; }

        /// <summary>
        /// The Unique Career Generator ID
        /// </summary>
        [Column]
        public int CareerGeneratorId { get; protected set; }

        #endregion

        #region Virtual Foreign Keys

        /// <summary>
        /// Gets the <see cref="Database.RandomizedPool"/> entity that this entity references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("RandomizedPoolId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<RandomizedPool> FK_Pool { get; set; }

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
        /// Gets or sets the <see cref="Database.RandomizedPool"/> that 
        /// this entity references.
        /// </summary>
        public RandomizedPool RandomizedPool
        {
            get
            {
                return FK_Pool?.Fetch();
            }
            set
            {
                RandomizedPoolId = value.Id;
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
