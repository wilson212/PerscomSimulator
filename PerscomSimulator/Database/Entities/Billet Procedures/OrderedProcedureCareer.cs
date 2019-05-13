using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    /// <summary>
    /// Represents a (1:1 or 1:0) relationship between a <see cref="Database.OrderedProcedure"/>
    /// and a <see cref="Database.CareerGenerator"/>
    /// </summary>
    /// <remarks>
    /// Used if there is a probability of new <see cref="Database.Soldier"/> creation in the parent 
    /// <see cref="Database.OrderedProcedure"/>, as we will need to generate a career length for
    /// that new soldier!
    /// </remarks>
    [Table]
    public class OrderedProcedureCareer
    {
        #region Columns

        /// <summary>
        /// The Unique Soldier Generator ID
        /// </summary>
        [Column, PrimaryKey]
        public int OrderedProcedureId { get; protected set; }

        /// <summary>
        /// The Unique Career Generator ID
        /// </summary>
        [Column, Required]
        public int CareerGeneratorId { get; protected set; }

        #endregion

        #region Virtual Foreign Keys

        /// <summary>
        /// Gets the <see cref="Database.OrderedProcedure"/> entity that this entity references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("OrderedProcedureId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<OrderedProcedure> FK_Procedure { get; set; }

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
        /// Gets or sets the <see cref="Database.OrderedProcedure"/> that 
        /// this entity references.
        /// </summary>
        public OrderedProcedure OrderedProcedure
        {
            get
            {
                return FK_Procedure?.Fetch();
            }
            set
            {
                OrderedProcedureId = value.Id;
                FK_Procedure?.Refresh();
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
