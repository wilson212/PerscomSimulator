using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    [Table]
    public class SpecialtyGradeStatistics : RankGradeStatistics
    {
        #region Columns

        /// <summary>
        /// Gets or sets the <see cref="Occupation.Id"/>
        /// </summary>
        [Column, PrimaryKey]
        public virtual int SpecialtyId { get; set; }

        #endregion

        #region Foreign Keys

        /// <summary>
        /// Gets or sets the <see cref="Occupation"/> entity that this entity references.
        /// </summary>
        [ForeignKey(nameof(SpecialtyId))]
        [References(nameof(Database.Occupation.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual Occupation Occupation { get; set; }

        #endregion
    }
}
