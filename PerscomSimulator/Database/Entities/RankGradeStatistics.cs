﻿using System;
using CrossLite;
using CrossLite.CodeFirst;
using Perscom.Simulation;

namespace Perscom.Database
{
    /// <summary>
    /// Contains a list of 
    /// </summary>
    [Table]
    public class RankGradeStatistics
    {
        /// <summary>
        /// 
        /// </summary>
        [Column, Required, PrimaryKey]
        public RankType RankType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column, Required, PrimaryKey]
        public int RankGrade { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column, Required, PrimaryKey]
        public int UnitTemplateId { get; set; }

        #region Total Statistics 

        /// <summary>
        /// Gets or Sets the total number of soldiers who held this grade, and were
        /// either promoted or retired out.
        /// </summary>
        [Column, Required]
        public int TotalSoldiers { get; set; } = 0;

        /// <summary>
        /// Gets or Sets the total accumulative months time in grade for all
        /// soldiers who held this grade and were either promoted or retired out.
        /// </summary>
        [Column, Required]
        public int TotalMonthsInGrade { get; set; } = 0;

        /// <summary>
        /// Gets or Sets the total accumulative months time in service for all
        /// soldiers who held this grade this grade and were either promoted or retired out.
        /// </summary>
        [Column, Required]
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
        [Column, Required]
        public int PromotionsToNextGrade { get; set; }

        /// <summary>
        /// Gets or Sets the total accumulative months time in grade for all
        /// soldiers who held this grade, just before being promoted
        /// </summary>
        [Column, Required]
        public int PromotedTotalMonthsInGrade { get; set; }

        /// <summary>
        /// Gets or Sets the total accumulative months time in service for all
        /// soldiers who held this grade, just before being promoted
        /// </summary>
        [Column, Required]
        public int PromotedTotalMonthsInService { get; set; }

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
        [Column, Required]
        public int TotalRetirements { get; set; }

        /// <summary>
        /// Gets or Sets the total number of retired personel who held this grade and
        /// met the requirements to be promotable to the next rank
        /// </summary>
        [Column, Required]
        public int TotalPromotableRetirees { get; set; }

        /// <summary>
        /// Gets or Sets the total accumulative months time in grade for all
        /// soldiers who held this grade, just before retiring
        /// </summary>
        [Column, Required]
        public int RetiredTotalMonthsInGrade { get; set; }

        /// <summary>
        /// Gets or Sets the total accumulative months time in service for all
        /// soldiers who held this grade, just before retiring
        /// </summary>
        [Column, Required]
        public int RetiredTotalMonthsInService { get; set; }

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
        /// The total number of months that a position was filled by a stand in soldier or empty.
        /// </summary>
        [Column, Required]
        public int Deficit { get; set; }

        /// <summary>
        /// Adds a soldier's statistical data as a retirement, holding this rank/grade
        /// </summary>
        /// <param name="soldier">The soldier being retired</param>
        /// <param name="currentDate">The current simulation date</param>
        public void TrackRetiree(SoldierWrapper soldier, IterationDate currentDate)
        {
            // Get time in service and grade in months
            int tis = soldier.EntryServiceDate.Date.MonthDifference(currentDate.Date);
            int tig = soldier.LastPromotionDate.Date.MonthDifference(currentDate.Date);

            // Increment total values
            TotalSoldiers += 1;
            TotalMonthsInGrade += tig;
            TotalMonthsInService += tis;

            // Increment retirement values
            TotalRetirements += 1;
            RetiredTotalMonthsInGrade += tig;
            RetiredTotalMonthsInService += tis;

            // Promotable?
            PromotableStatus type;
            if (soldier.IsPromotable(currentDate, out type))
                TotalPromotableRetirees += 1;
        }

        /// <summary>
        /// Adds a soldier's statistical data as a promotion to the next grade
        /// </summary>
        /// <param name="soldier">The soldier being promoted from this grade to the next</param>
        /// <param name="currentDate">The current simulation date</param>
        public void TrackPromotionToNextGrade(SoldierWrapper soldier, DateTime currentDate)
        {
            // Get time in service and grade in months
            int tis = soldier.EntryServiceDate.Date.MonthDifference(currentDate);
            int tig = soldier.LastPromotionDate.Date.MonthDifference(currentDate);

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
