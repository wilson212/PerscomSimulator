using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    [Table]
    public class SpecialtyGradeStatistics : RankGradeStatistics
    {
        /// <summary>
        /// The Unique Billet ID
        /// </summary>
        [Column, PrimaryKey]
        public int SpecialtyId { get; set; }

        #region Virtual Foreign Keys

        /// <summary>
        /// Gets the <see cref="Specialty"/> entity that this entity references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("SpecialtyId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<Specialty> FK_Specialty { get; set; }

        #endregion
    }
}
