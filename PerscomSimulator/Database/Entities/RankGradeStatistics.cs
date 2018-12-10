using System;
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
        [Column, PrimaryKey]
        public RankType RankType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column, PrimaryKey]
        public int RankGrade { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column, PrimaryKey]
        public int UnitTemplateId { get; set; }

        #region Virtual Foreign Keys

        /// <summary>
        /// Gets the <see cref="UnitTemplate"/> entity that this entity references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("UnitTemplateId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<UnitTemplate> FK_Parent { get; set; }

        #endregion

        #region Total Statistics 

        /// <summary>
        /// Gets or Sets the total number of soldiers were promnoted into this grade
        /// </summary>
        [Column, Required]
        public int TotalSoldiersIncoming { get; set; } = 0;

        /// <summary>
        /// Gets or Sets the total number of soldiers who held this grade, and were
        /// either promoted or retired out, or were transfered OUT of this Rank into 
        /// a different <see cref="Simulation.RankType"/>
        /// </summary>
        /// <remarks>
        /// This value will always be the sum of Promotions, Retirements, and TransfersFrom
        /// </remarks>
        [Column, Required]
        public int TotalSoldiersOutgoing { get; set; } = 0;

        /// <summary>
        /// Gets or Sets the total accumulative months time in grade for all
        /// soldiers who held this grade and were either promoted or retired out,
        /// or were transfered OUT of this Rank into a different <see cref="Simulation.RankType"/>
        /// </summary>
        [Column, Required]
        public int TotalMonthsInGrade { get; set; } = 0;

        /// <summary>
        /// Gets or Sets the total accumulative months time in service for all
        /// soldiers who held this grade this grade and were either promoted or retired out,
        /// or were transfered OUT of this Rank into a different <see cref="Simulation.RankType"/>
        /// </summary>
        [Column, Required]
        public int TotalMonthsInService { get; set; } = 0;

        /// <summary>
        /// Gets the average time in grade (months) for this grade.
        /// </summary>
        public decimal AverageTimeInGrade => (TotalSoldiersOutgoing == 0) ? 0 : Math.Round(TotalMonthsInGrade / (decimal)TotalSoldiersOutgoing, 2);

        /// <summary>
        /// Gets the average time in service (months) for this grade.
        /// </summary>
        public decimal AverageTimeInService => (TotalSoldiersOutgoing == 0) ? 0 : Math.Round(TotalMonthsInService / (decimal)TotalSoldiersOutgoing, 2);

        /// <summary>
        /// Gets the average time in grade (years) for this grade.
        /// </summary>
        public decimal AverageYearsInService => (TotalSoldiersOutgoing == 0) ? 0 : Math.Round((TotalMonthsInService / 12) / (decimal)TotalSoldiersOutgoing, 2);

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

        #region Transfer From Statistics

        /// <summary>
        /// Gets or Sets the total number of soldiers who held this grade, and were
        /// then promoted to a different Rank Type.
        /// </summary>
        [Column, Required]
        public int TransfersFrom { get; set; }

        /// <summary>
        /// Gets or Sets the total accumulative months time in grade for all
        /// soldiers who held this grade, just before being promoted
        /// </summary>
        [Column, Required]
        public int TransfersFromTotalMonthsInGrade { get; set; }

        /// <summary>
        /// Gets or Sets the total accumulative months time in service for all
        /// soldiers who held this grade, just before being promoted
        /// </summary>
        [Column, Required]
        public int TransfersFromTotalMonthsInService { get; set; }

        /// <summary>
        /// Gets the average time in grade (months) for all soldiers 
        /// who held this grade, just before being promoted
        /// </summary>
        public decimal TransfersFromAverageTimeInGrade
            => (TransfersFrom == 0) ? 0 : Math.Round(TransfersFromTotalMonthsInGrade / (decimal)TransfersFrom, 2);

        /// <summary>
        /// Gets the average time in service (months) for all soldiers 
        /// who held this grade, just before being promoted
        /// </summary>
        public decimal TransfersFromAverageTimeInService
            => (TransfersFrom == 0) ? 0 : Math.Round(TransfersFromTotalMonthsInService / (decimal)TransfersFrom, 2);

        /// <summary>
        /// Gets the average time in grade (years) for all soldiers 
        /// who held this grade, just before being promoted
        /// </summary>
        public decimal TransfersFromAverageYearsInService
            => (TransfersFrom == 0) ? 0 : Math.Round((TransfersFromTotalMonthsInService / 12) / (decimal)TransfersFrom, 2);

        #endregion Transfer From Statistics

        #region Transfer Into Statistics

        /// <summary>
        /// Gets or sets the total number of soldiers who were promoted into this 
        /// rank from a different Rank Type.
        /// </summary>
        [Column, Required]
        public int TransfersInto { get; set; }

        #endregion Transfer Into Statistics

        /// <summary>
        /// Gets the percentage of soldiers who made Promotable Status to the next Rank Grade
        /// </summary>
        public decimal PromotablePercentage
        {
            get
            {
                // If no-one got promoted, then the rate is 0 obviously
                if (TotalSoldiersOutgoing == 0) return 0;

                // Get the count of promotable only.
                int totalPromotable = PromotionsToNextGrade + TotalPromotableRetirees;
                return Math.Round(totalPromotable / (decimal)TotalSoldiersOutgoing, 2) * 100;
            }
        }

        /// <summary>
        /// Gets the precentage of soldiers who advanced to the next grade total, including
        /// those who were never promotable.
        /// </summary>
        public decimal PromotionRate 
            => (PromotionsToNextGrade == 0) ? 0 : Math.Round(PromotionsToNextGrade / (decimal)TotalSoldiersOutgoing, 2) * 100;

        /// <summary>
        /// Gets the advancement rate of soldiers who were promotable only.
        /// This value DOES NOT count soldiers who were transfered OUT of this Rank 
        /// into a different <see cref="Simulation.RankType"/>
        /// </summary>
        public decimal PromotableSelectionRate
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
        /// Gets a percentage rate of soldiers who transfered from this Rank to
        /// a different <see cref="Simulation.RankType"/>
        /// </summary>
        public decimal TransferFromRate
        {
            get
            {
                // If no-one got transfered, then the rate is 0 obviously
                if (TransfersFrom == 0) return 0;

                // Get the count of promotable only.
                return Math.Round(TransfersFrom / (decimal)TotalSoldiersOutgoing, 2) * 100;
            }
        }

        /// <summary>
        /// Gets a percentage rate of soldiers who transfered into this Rank to
        /// a different <see cref="Simulation.RankType"/>
        /// </summary>
        public decimal TransferIntoRate
        {
            get
            {
                // If no-one got transfered, then the rate is 0 obviously
                if (TransfersInto == 0) return 0;

                // Get the count of promotable only.
                return Math.Round(TransfersInto / (decimal)TotalSoldiersIncoming, 2) * 100;
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
            int tig = soldier.LastGradeChangeDate.Date.MonthDifference(currentDate.Date);

            // Increment total values
            TotalSoldiersOutgoing += 1;
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
        public void TrackPromotionToNextGrade(RankChangeEventArgs args, DateTime currentDate)
        {
            // Get time in service and grade in months
            int tis = args.Soldier.EntryServiceDate.Date.MonthDifference(currentDate);
            int tig = args.Promotion.TimeSinceLastGradeChange;

            // Increment total values
            TotalSoldiersOutgoing += 1;
            TotalMonthsInGrade += tig;
            TotalMonthsInService += tis;

            // Increment promotion values
            PromotionsToNextGrade += 1;
            PromotedTotalMonthsInGrade += tig;
            PromotedTotalMonthsInService += tis;
        }

        /// <summary>
        /// Adds a soldier's statistical data as a promotion to the next grade
        /// </summary>
        /// <param name="soldier">The soldier being promoted from this grade to the next</param>
        /// <param name="currentDate">The current simulation date</param>
        public void TrackPromotionToNextGrade(PositionAndRankChangeEventArgs args, DateTime currentDate)
        {
            // Get time in service and grade in months
            int tis = args.Soldier.EntryServiceDate.Date.MonthDifference(currentDate);
            int tig = args.Promotion.TimeSinceLastGradeChange;

            // Increment total values
            TotalSoldiersOutgoing += 1;
            TotalMonthsInGrade += tig;
            TotalMonthsInService += tis;

            // Increment promotion values
            PromotionsToNextGrade += 1;
            PromotedTotalMonthsInGrade += tig;
            PromotedTotalMonthsInService += tis;
        }

        public void TrackRankTransferFrom(RankChangeEventArgs args, IterationDate currentDate)
        {
            // Get time in service and grade in months
            int tis = args.Soldier.EntryServiceDate.Date.MonthDifference(currentDate.Date);
            int tig = args.Promotion.TimeSinceLastGradeChange;

            // Increment total values
            TotalSoldiersOutgoing += 1;
            TotalMonthsInGrade += tig;
            TotalMonthsInService += tis;

            TransfersFrom += 1;
            TransfersFromTotalMonthsInGrade += tig;
            TransfersFromTotalMonthsInService += tis;
        }

        public void TrackRankTransferFrom(PositionAndRankChangeEventArgs args, IterationDate currentDate)
        {
            // Get time in service and grade in months
            int tis = args.Soldier.EntryServiceDate.Date.MonthDifference(currentDate.Date);
            int tig = args.Promotion.TimeSinceLastGradeChange;

            // Increment total values
            TotalSoldiersOutgoing += 1;
            TotalMonthsInGrade += tig;
            TotalMonthsInService += tis;

            TransfersFrom += 1;
            TransfersFromTotalMonthsInGrade += tig;
            TransfersFromTotalMonthsInService += tis;
        }

        public void TrackRankTransferInto(SoldierWrapper soldier)
        {
            // DO NOT ADD TO SOLDIER COUNTS! This will be done when promoted or retired!
            TransfersInto += 1;
            TotalSoldiersIncoming += 1;
        }

        public void TrackPromotionIntoGrade(SoldierWrapper soldier)
        {
            TotalSoldiersIncoming += 1;
        }
    }
}
