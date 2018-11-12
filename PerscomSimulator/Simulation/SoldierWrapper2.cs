using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodaTime;
using Perscom.Database;

namespace Perscom.Simulation
{
    public class SoldierWrapper2
    {
        /// <summary>
        /// Gets the soldiers first and last name
        /// </summary>
        public string Name => $"{Soldier.FirstName} {Soldier.LastName}";

        /// <summary>
        /// 
        /// </summary>
        public Soldier Soldier { get; protected set; }

        /// <summary>
        /// Gets the soldiers current <see cref="Database.Rank"/>
        /// </summary>
        public Rank Rank { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Position Position { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime EntryServiceDate { get; set; }

        public DateTime ExitServiceDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        //public DateTime LastPromotionDate { get; set; }

        public DateTime LastGradeChangeDate { get; set; }

        public int TimeInService { get; set; }

        public int TimeInGrade { get; set; }

        /// <summary>
        /// Gets the rank icon image
        /// </summary>
        public Image RankIcon => ImageAccessor.GetImage(Path.Combine("Icons", Rank.Image));

        /// <summary>
        /// Gets the rank image
        /// </summary>
        public Image RankImage => ImageAccessor.GetImage(Path.Combine("Large", Rank.Image));

        public SoldierWrapper2(Soldier soldier, DateTime currentDate)
        {
            Soldier = soldier;
            Rank = soldier.Rank;
            Position = soldier.Assignments.FirstOrDefault()?.Position;

            var difference = soldier.ExitIterationId - soldier.EntryIterationId;
            EntryServiceDate = soldier.EntryServiceDate.Date;
            ExitServiceDate = EntryServiceDate.AddMonths(difference);
            //LastPromotionDate = soldier.LastPromotionDate.Date;
            LastGradeChangeDate = soldier.LastGradeChangeDate.Date;

            TimeInGrade = LastGradeChangeDate.MonthDifference(currentDate);
            TimeInService = (soldier.Retired) 
                ? EntryServiceDate.MonthDifference(ExitServiceDate) 
                : EntryServiceDate.MonthDifference(currentDate);
        }

        public int GetTimeInService(IterationDate currentDate)
        {
            return currentDate.Id - Soldier.EntryIterationId;
        }
    }
}
