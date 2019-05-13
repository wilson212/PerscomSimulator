using CrossLite;
using CrossLite.CodeFirst;
using Perscom.Simulation;

namespace Perscom.Database
{
    [Table]
    public class BilletRandomizedProcedure : BilletCustomProcedure
    {
        /// <summary>
        /// The Unique Soldier Generator ID
        /// </summary>
        [Column, Required]
        public int RandomizedProcedureId { get; set; }

        #region Virtual Foreign Keys

        /// <summary>
        /// Gets the <see cref="Database.RandomizedProcedure"/> entity that this entity references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("RandomizedProcedureId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<RandomizedProcedure> FK_Generator { get; set; }

        #endregion

        #region Foreign Key Properties

        /// <summary>
        /// Gets or Sets the <see cref="Perscom.Database.RandomizedProcedure"/> that 
        /// this Billit uses.
        /// </summary>
        public RandomizedProcedure Procedure
        {
            get
            {
                return FK_Generator?.Fetch();
            }
            set
            {
                RandomizedProcedureId = value.Id;
                FK_Generator?.Refresh();
            }
        }

        public override SelectionProcedure ProcedureType => SelectionProcedure.RandomizedProcedure;

        #endregion

        public override int GetProcedureId()
        {
            return RandomizedProcedureId;
        }
    }
}
