using CrossLite;
using CrossLite.CodeFirst;
using Perscom.Simulation;

namespace Perscom.Database
{
    [Table]
    public class BilletOrderedProcedure : BilletCustomProcedure
    {
        /// <summary>
        /// The Unique Soldier Generator ID
        /// </summary>
        [Column, Required]
        public int OrderedProcedureId { get; set; }

        #region Virtual Foreign Keys

        /// <summary>
        /// Gets the <see cref="Database.OrderedProcedure"/> entity that this entity references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("OrderedProcedureId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<OrderedProcedure> FK_Generator { get; set; }

        #endregion

        #region Foreign Key Properties

        /// <summary>
        /// Gets or Sets the <see cref="Perscom.Database.OrderedProcedure"/> that 
        /// this Billit uses.
        /// </summary>
        public OrderedProcedure Procedure
        {
            get
            {
                return FK_Generator?.Fetch();
            }
            set
            {
                OrderedProcedureId = value.Id;
                FK_Generator?.Refresh();
            }
        }

        public override SelectionProcedure ProcedureType => SelectionProcedure.OrderedProcedure;

        public override int GetProcedureId()
        {
            return OrderedProcedureId;
        }

        #endregion
    }
}
