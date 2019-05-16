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
    public class SoldierFormWrapper
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
        /// The soldier current <see cref="Position"/> or null if retired.
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
        /// Indicates whether this is a past assignment or current
        /// </summary>
        public bool IsPast { get; protected set; }

        /// <summary>
        /// Gets the rank icon image
        /// </summary>
        public Image CurrentRankIcon => ImageAccessor.GetImage(Path.Combine("Icons", Rank.Image));

        /// <summary>
        /// Gets the rank image
        /// </summary>
        public Image CurrentRankImage => ImageAccessor.GetImage(Path.Combine("Large", Rank.Image));

        public Image EntryRankIcon { get; set; }

        public Image EntryRankImage { get; set; }

        public Image ExitRankIcon { get; set; }

        public Image ExitRankImage { get; set; }

        public SoldierFormWrapper(Soldier soldier, DateTime currentDate)
        {
            Soldier = soldier;
            Rank = soldier.Rank;

            var assignment = soldier.Assignments.FirstOrDefault();
            if (assignment != null)
            {
                Position = assignment.Position;

                var eRankImage = assignment.EntryRank.Image;
                EntryRankIcon = ImageAccessor.GetImage(Path.Combine("Icons", eRankImage));
                EntryRankImage = ImageAccessor.GetImage(Path.Combine("Large", eRankImage));
            }
            else
            {
                EntryRankImage = CurrentRankImage;
                EntryRankIcon = CurrentRankIcon;
            }

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

        public SoldierFormWrapper(Soldier soldier, IterationDate currentDate, PastAssignment assignment)
        {
            Soldier = soldier;
            Rank = soldier.Rank;
            Position = soldier.Assignments.FirstOrDefault()?.Position;

            var eRankImage = assignment.EntryRank.Image;
            EntryRankIcon = ImageAccessor.GetImage(Path.Combine("Icons", eRankImage));
            EntryRankImage = ImageAccessor.GetImage(Path.Combine("Large", eRankImage));

            eRankImage = assignment.ExitRank.Image;
            ExitRankIcon = ImageAccessor.GetImage(Path.Combine("Icons", eRankImage));
            ExitRankImage = ImageAccessor.GetImage(Path.Combine("Large", eRankImage));

            var difference = soldier.ExitIterationId - soldier.EntryIterationId;
            EntryServiceDate = soldier.EntryServiceDate.Date;
            ExitServiceDate = EntryServiceDate.AddMonths(difference);
            //LastPromotionDate = soldier.LastPromotionDate.Date;
            LastGradeChangeDate = assignment.LastGradeChangeDate.Date;

            TimeInGrade = LastGradeChangeDate.MonthDifference(currentDate.Date);
            TimeInService = (soldier.Retired)
                ? EntryServiceDate.MonthDifference(ExitServiceDate)
                : EntryServiceDate.MonthDifference(currentDate.Date);
        }

        public int GetTimeInService(IterationDate currentDate)
        {
            return currentDate.Id - Soldier.EntryIterationId;
        }
    }
}
