using System;
using System.Collections.Generic;
using Perscom.Simulation;

namespace Perscom
{
    /// <summary>
    /// An object that represents a Soldier
    /// </summary>
    public class Soldier : ISpawnable, ICloneable
    {
        /// <summary>
        /// The Soldiers unique itterator number
        /// </summary>
        public int SpawnId { get; set; }

        /// <summary>
        /// Gets the soldiers first name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets the soldiers last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="Rank"/> for this soldier
        /// </summary>
        public Rank RankInfo { get; set; }

        /// <summary>
        /// Gets the <see cref="DateTime"/> the soldier was created in
        /// the <see cref="Simulator"/>
        /// </summary>
        public DateTime ServiceEntryDate { get; set; }

        /// <summary>
        /// Gets the assigned end date for this soldier
        /// </summary>
        public DateTime ExitServiceDate { get; set; }

        /// <summary>
        /// Gets the last promotion date for this soldier
        /// </summary>
        public DateTime LastPromotionDate { get; set; }

        /// <summary>
        /// The range of months this soldier will live
        /// </summary>
        public Range<int> TimeToLive { get; protected set; }

        /// <summary>
        /// Gets the probability value of this soldier
        /// </summary>
        public int OneInThousandProbability { get; set; }

        /// <summary>
        /// A list of all the soldiers promotions
        /// </summary>
        public List<Promotion> Promotions { get; set; } = new List<Promotion>(5);

        public Soldier(int oneInThousandProbability, Range<int> timeToLive)
        {
            OneInThousandProbability = oneInThousandProbability;
            TimeToLive = timeToLive;
        }

        /// <summary>
        /// Returns whether this soldier's has lifespan has expired
        /// </summary>
        /// <param name="currentDate">The current date in the simulation</param>
        /// <returns></returns>
        public bool IsRetiring(DateTime currentDate)
        {
            bool rcp = false;
            if (RankInfo.MaxTimeInGrade > 0)
            {
                int months = currentDate.MonthDifference(LastPromotionDate);
                rcp = months >= RankInfo.MaxTimeInGrade;
            }
            return DateTime.Compare(currentDate, ExitServiceDate) >= 0 || rcp;
        }

        /// <summary>
        /// Returns whether the soldier is currently promotable
        /// </summary>
        /// <param name="currentDate">The current date in the simulation</param>
        /// <returns></returns>
        public bool IsPromotable(DateTime currentDate)
        {
            int months = currentDate.MonthDifference(LastPromotionDate);
            return RankInfo.PromotableAt <= months;
        }

        /// <summary>
        /// Promotes the soldier to the next grade.
        /// </summary>
        /// <param name="currentDate">The current date in the simulation</param>
        /// <param name="toRank">The rank to promote this soldier to</param>
        public void Promote(DateTime currentDate, Rank toRank)
        {
            // Add this promotion to the soldiers promotion list
            Promotions.Add(new Promotion()
            {
                Date = currentDate,
                FromRank = RankInfo,
                ToRank = toRank,
                PreviousTimeInGrade = LastPromotionDate.MonthDifference(currentDate),
                TimeInService = ServiceEntryDate.MonthDifference(currentDate)
            });

            // Update internals
            RankInfo = toRank;
            LastPromotionDate = currentDate;

            // Adjust retirement date if we are on stature
            if (toRank.Stature != null)
            {
                // Get remaining months before retirement
                int monthsToGo = currentDate.MonthDifference(ExitServiceDate);

                // If the months to go is less than the minimum, adjust it
                if (monthsToGo < toRank.Stature.Minimum)
                {
                    if (toRank.Stature.Maximum == 0)
                    {
                        ExitServiceDate = currentDate.AddMonths(toRank.Stature.Minimum);
                    }
                    else
                    {
                        CryptoRandom random = new CryptoRandom();
                        monthsToGo = random.Next(toRank.Stature.Minimum, toRank.Stature.Maximum);
                        ExitServiceDate = currentDate.AddMonths(monthsToGo);
                    }
                }
            }
        }

        public object Clone()
        {
            return new Soldier(OneInThousandProbability, TimeToLive);
        }
    }
}
