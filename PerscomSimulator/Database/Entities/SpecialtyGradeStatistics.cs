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
        [Column, Required, PrimaryKey]
        public int SpecialtyId { get; set; }
    }
}
