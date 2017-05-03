using System;

namespace Perscom.Simulation
{
    public class RankGradeStatistics
    {
        #region Total Statistics 
        /// <summary>
        /// Gets or Sets the total number of soldiers who held this grade, and were
        /// either promoted or retired out.
        /// </summary>
        public int TotalSoldiers { get; set; } = 0;

        /// <summary>
        /// Gets or Sets the total accumulative months time in grade for all
        /// soldiers who held this grade and were either promoted or retired out.
        /// </summary>
        public int TotalMonthsInGrade { get; set; } = 0;

        /// <summary>
        /// Gets or Sets the total accumulative months time in service for all
        /// soldiers who held this grade this grade and were either promoted or retired out.
        /// </summary>
        public int TotalMonthsInService { get; set; } = 0;

        /// <summary>
        /// Gets the average time in grade (months) for this grade
        /// </summary>
        public decimal AverageTimeInGrade => (TotalSoldiers == 0) ? 0 : Math.Round(TotalMonthsInGrade / (decimal)TotalSoldiers, 2);

        /// <summary>
        /// Gets the average time in service (months) for this grade
        /// </summary>
        public decimal AverageTimeInService => (TotalSoldiers == 0) ? 0 : Math.Round(TotalMonthsInService / (decimal)TotalSoldiers, 2);

        /// <summary>
        /// Gets the average time in grade (years) for this grade
        /// </summary>
        public decimal AverageYearsInService => (TotalSoldiers == 0) ? 0 : Math.Round((TotalMonthsInService / 12) / (decimal)TotalSoldiers, 2);

        #endregion Total Statistics

        #region Promoted Statistics

        /// <summary>
        /// Gets or Sets the total number of soldiers who held this grade, and were
        /// then promoted to the next grade.
        /// </summary>
        public int PromotionsToNextGrade = 0;

        /// <summary>
        /// Gets or Sets the total accumulative months time in grade for all
        /// soldiers who held this grade, just before being promoted
        /// </summary>
        public int PromotedTotalMonthsInGrade { get; set; } = 0;

        /// <summary>
        /// Gets or Sets the total accumulative months time in service for all
        /// soldiers who held this grade, just before being promoted
        /// </summary>
        public int PromotedTotalMonthsInService { get; set; } = 0;

        /// <summary>
        /// Gets the average time in grade (months) for all soldiers 
        /// who held this grade, just before being promoted
        /// </summary>
        public decimal PromotedAverageTimeInGrade 
            => (PromotionsToNextGrade == 0) ? 0 : Math.Round(PromotedTotalMonthsInGrade / (decimal)PromotionsToNextGrade, 2);

        /// <summary>
        /// Gets the average time in service (months) for all soldiers 
        /// who held this grade, just before being promoted
        /// </summary>
        public decimal PromotedAverageTimeInService 
            => (PromotionsToNextGrade == 0) ? 0 : Math.Round(PromotedTotalMonthsInService / (decimal)PromotionsToNextGrade, 2);

        /// <summary>
        /// Gets the average time in grade (years) for all soldiers 
        /// who held this grade, just before being promoted
        /// </summary>
        public decimal PromotedAverageYearsInService 
            => (PromotionsToNextGrade == 0) ? 0 : Math.Round((PromotedTotalMonthsInService / 12) / (decimal)PromotionsToNextGrade, 2);

        #endregion Promoted Statistics

        #region Retired Statistics

        /// <summary>
        /// Gets or Sets the total number of soldiers who retired as this rank/grade
        /// </summary>
        public int TotalRetirements = 0;

        /// <summary>
        /// Gets or Sets the total number of retired personel who held this grade and
        /// met the requirements to be promotable to the next rank
        /// </summary>
        public int TotalPromotableRetirees { get; set; } = 0;

        /// <summary>
        /// Gets or Sets the total accumulative months time in grade for all
        /// soldiers who held this grade, just before retiring
        /// </summary>
        public int RetiredTotalMonthsInGrade { get; set; } = 0;

        /// <summary>
        /// Gets or Sets the total accumulative months time in service for all
        /// soldiers who held this grade, just before retiring
        /// </summary>
        public int RetiredTotalMonthsInService { get; set; } = 0;

        /// <summary>
        /// Gets the average time in grade (months) for all soldiers 
        /// who held this grade, just before retiring
        /// </summary>
        public decimal RetiredAverageTimeInGrade
            => (TotalRetirements == 0) ? 0 : Math.Round(RetiredTotalMonthsInGrade / (decimal)TotalRetirements, 2);

        /// <summary>
        /// Gets the average time in service (months) for all soldiers 
        /// who held this grade, just before retiring
        /// </summary>
        public decimal RetiredAverageTimeInService
            => (TotalRetirements == 0) ? 0 : Math.Round(RetiredTotalMonthsInService / (decimal)TotalRetirements, 2);

        /// <summary>
        /// Gets the average time in grade (years) for all soldiers 
        /// who held this grade, just before retiring
        /// </summary>
        public decimal RetiredAverageYearsInService
            => (TotalRetirements == 0) ? 0 : Math.Round((RetiredTotalMonthsInService / 12) / (decimal)TotalRetirements, 2);

        #endregion Retired Statistics

        /// <summary>
        /// Gets the precentage of soldiers who advanced to the next grade total, including
        /// those who were never promotable.
        /// </summary>
        public decimal PromotionRate 
            => (PromotionsToNextGrade == 0) ? 0 : Math.Round(PromotionsToNextGrade / (decimal)TotalSoldiers, 2) * 100;

        /// <summary>
        /// Gets the advancement rate of soldiers who were promotable only.
        /// </summary>
        public decimal SelectionRate
        {
            get
            {
                // If no-one got promoted, then the rate is 0 obviously
                if (PromotionsToNextGrade == 0) return 0;

                // Get the count of promotable only.
                int totalPromotable = PromotionsToNextGrade + TotalPromotableRetirees;
                return Math.Round(PromotionsToNextGrade / (decimal)totalPromotable, 2) * 100;
            }
        }

        /// <summary>
        /// Adds a soldier's statistical data as a retirement, holding this rank/grade
        /// </summary>
        /// <param name="soldier">The soldier being retired</param>
        /// <param name="currentDate">The current simulation date</param>
        public void AddRetiree(Soldier soldier, DateTime currentDate)
        {
            // Get time in service and grade in months
            int tis = soldier.ServiceEntryDate.MonthDifference(currentDate);
            int tig = soldier.LastPromotionDate.MonthDifference(currentDate);

            // Increment total values
            TotalSoldiers += 1;
            TotalMonthsInGrade += tig;
            TotalMonthsInService += tis;

            // Increment retirement values
            TotalRetirements += 1;
            RetiredTotalMonthsInGrade += tig;
            RetiredTotalMonthsInService += tis;

            // Promotable?
            if (soldier.IsPromotable(currentDate))
                TotalPromotableRetirees += 1;
        }

        /// <summary>
        /// Adds a soldier's statistical data as a promotion to the next grade
        /// </summary>
        /// <param name="soldier">The soldier being promoted from this grade to the next</param>
        /// <param name="currentDate">The current simulation date</param>
        public void TrackPromotionToNextGrade(Soldier soldier, DateTime currentDate)
        {
            // Get time in service and grade in months
            int tis = soldier.ServiceEntryDate.MonthDifference(currentDate);
            int tig = soldier.LastPromotionDate.MonthDifference(currentDate);

            // Increment total values
            TotalSoldiers += 1;
            TotalMonthsInGrade += tig;
            TotalMonthsInService += tis;

            // Increment promotion values
            PromotionsToNextGrade += 1;
            PromotedTotalMonthsInGrade += tig;
            PromotedTotalMonthsInService += tis;
        }
    }
}
