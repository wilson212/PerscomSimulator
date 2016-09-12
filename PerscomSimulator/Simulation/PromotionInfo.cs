using System;

namespace Perscom.Simulation
{
    public class PromotionInfo
    {
        /// <summary>
        /// Gets or Sets the total accumulative months time in grade for all
        /// soldiers in this grade
        /// </summary>
        public int TotalMonthsInGrade { get; set; } = 0;

        /// <summary>
        /// Gets or Sets the total accumulative months time in service for all
        /// soldiers in this grade
        /// </summary>
        public int TotalMonthsInService { get; set; } = 0;

        /// <summary>
        /// Gets or Sets the total number of people who held this rank
        /// </summary>
        public int TotalPersonel { get; set; } = 0;

        /// <summary>
        /// Gets or Sets the total number of retired personel whom where
        /// promotable to the next rank
        /// </summary>
        public int TotalPromotable { get; set; } = 0;

        /// <summary>
        /// Gets the average time in grade (months) for this grade
        /// </summary>
        public decimal AverageTimeInGrade
        {
            get
            {
                return Math.Round(TotalMonthsInGrade / (decimal)TotalPersonel, 2);
            }
        }

        /// <summary>
        /// Gets the average time in service (months) for this grade
        /// </summary>
        public decimal AverageTimeInService
        {
            get
            {
                return Math.Round(TotalMonthsInService / (decimal)TotalPersonel, 2);
            }
        }

        /// <summary>
        /// Gets the average time in grade (years) for this grade
        /// </summary>
        public decimal AverageYearsInService
        {
            get
            {
                return Math.Round((TotalMonthsInService / 12) / (decimal)TotalPersonel, 2);
            }
        }
    }
}
