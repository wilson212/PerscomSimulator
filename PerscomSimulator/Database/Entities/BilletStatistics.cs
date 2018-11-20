using CrossLite;
using CrossLite.CodeFirst;
using System;

namespace Perscom.Database
{
    [Table]
    public class BilletStatistics
    {
        /// <summary>
        /// 
        /// </summary>
        [Column, Required, PrimaryKey]
        public int BilletId { get; set; }

        /// <summary>
        /// Gets or Sets the total number of soldiers who held this Billet
        /// </summary>
        [Column, Required]
        public int TotalSoldiersIncoming { get; set; } = 0;

        /// <summary>
        /// Gets or Sets the total number of soldiers who held this Billet, and were
        /// either promoted or retired out, or were transfered OUT of this Billet
        /// </summary>
        [Column, Required]
        public int TotalSoldiersOutgoing { get; set; } = 0;

        /// <summary>
        /// Gets or Sets the total accumulative months for all
        /// soldiers who held this Billet
        /// </summary>
        [Column, Required]
        public int TotalMonthsInBillet { get; set; } = 0;

        /// <summary>
        /// The total number of months that a billet was filled by a stand in soldier.
        /// </summary>
        [Column, Required]
        public int StandInDeficit { get; set; }

        /// <summary>
        /// The total number of months that a billet was empty.
        /// </summary>
        [Column, Required]
        public int EmptyDeficit { get; set; }

        /// <summary>
        /// Gets the average time in grade (months) for this grade.
        /// </summary>
        public decimal AverageTimeInBillet
        {
            get
            {
                return (TotalSoldiersOutgoing == 0) ? 0 : Math.Round(TotalMonthsInBillet / (decimal)TotalSoldiersOutgoing, 2);
            }
        }
    }
}
