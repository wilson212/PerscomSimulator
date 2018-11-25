using CrossLite;
using CrossLite.CodeFirst;
using System;

namespace Perscom.Database
{
    public abstract class AbstractBilletStatistics
    {
        /// <summary>
        /// Gets or Sets the total number of soldiers who held this Position
        /// </summary>
        [Column, Required]
        public int TotalSoldiersIncoming { get; set; } = 0;

        /// <summary>
        /// Gets or Sets the total number of soldiers who held this Position, and were
        /// either promoted or retired out, or were transfered OUT of this Billet
        /// </summary>
        [Column, Required]
        public int TotalSoldiersOutgoing { get; set; } = 0;

        /// <summary>
        /// Gets or sets the total number of soldiers who were promoted into this position
        /// </summary>
        [Column, Required]
        public int TotalSoldiersPromotedIn { get; set; } = 0;

        /// <summary>
        /// Gets or sets the total number of soldiers who were promoted out of this position
        /// </summary>
        [Column, Required]
        public int TotalSoldiersPromotedOut { get; set; } = 0;

        /// <summary>
        /// Gets or sets the total number of soldiers who were laterally promoted into this position
        /// </summary>
        [Column, Required]
        public int TotalSoldiersLateralIn { get; set; } = 0;

        /// <summary>
        /// Gets or sets the total number of soldiers who were laterally promoted out of this position
        /// </summary>
        [Column, Required]
        public int TotalSoldiersLateralOut { get; set; } = 0;

        /// <summary>
        /// Gets or sets the total number of soldiers who retired from position
        /// </summary>
        [Column, Required]
        public int TotalSoldiersRetireOut { get; set; } = 0;

        /// <summary>
        /// Gets or Sets the total accumulative months for all
        /// soldiers who held this Billet
        /// </summary>
        [Column, Required]
        public int TotalMonthsInPosition { get; set; } = 0;

        /// <summary>
        /// Gets or Sets the total accumulative months time in grade for all
        /// soldiers who came INTO this position
        /// </summary>
        [Column, Required]
        public int TotalMonthsInGradeIncoming { get; set; } = 0;

        /// <summary>
        /// Gets or Sets the total accumulative months time in service for all
        /// soldiers who came INTO this position
        /// </summary>
        [Column, Required]
        public int TotalMonthsInServiceIncoming { get; set; } = 0;

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
        public decimal AverageTimeInPosition
        {
            get
            {
                return (TotalSoldiersOutgoing == 0) ? 0 : Math.Round(TotalMonthsInPosition / (decimal)TotalSoldiersOutgoing, 2);
            }
        }

        /// <summary>
        /// Gets the average time in grade (months) for this grade.
        /// </summary>
        public decimal AverageTimeInGradeIncoming
        {
            get
            {
                return (TotalSoldiersIncoming == 0) ? 0 : Math.Round(TotalMonthsInGradeIncoming / (decimal)TotalSoldiersIncoming, 2);
            }
        }

        /// <summary>
        /// Gets the average time in service (months) for this grade.
        /// </summary>
        public decimal AverageTimeInServiceIncoming
        {
            get
            {
                return (TotalSoldiersIncoming == 0) ? 0 : Math.Round(TotalMonthsInServiceIncoming / (decimal)TotalSoldiersIncoming, 2);
            }
        }
    }
}
