using System;
using System.Collections.Generic;
using System.Linq;
using Perscom.Simulation;

namespace Perscom
{
    /// <summary>
    /// An object that represents a Soldier
    /// </summary>
    public class Soldier : ISpawnable, ICloneable, IEquatable<Soldier>
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
        /// Gets or Sets the position this soldier sits in
        /// </summary>
        public UnitPosition Position { get; protected set; }

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
        public List<Promotion> Promotions { get; set; } = new List<Promotion>();

        /// <summary>
        /// A list of all positions this soldier held in his career
        /// </summary>
        public List<Billet> Positions { get; set; } = new List<Billet>();

        /// <summary>
        /// Creates a new instance of <see cref="Soldier"/>
        /// </summary>
        /// <param name="oneInThousandProbability">The spawn chance out of 1000</param>
        /// <param name="timeToLive">The time (in months) this soldier will live before retiring</param>
        public Soldier(int oneInThousandProbability, Range<int> timeToLive)
        {
            OneInThousandProbability = oneInThousandProbability;
            TimeToLive = timeToLive;
        }

        /// <summary>
        /// Assigns a new <see cref="UnitPosition"/> for this soldier instance
        /// </summary>
        /// <param name="position"></param>
        public void AssignPosition(UnitPosition position, DateTime currentDate)
        {
            // Set end date for last position
            if (Positions.Count > 0)
                Positions.Last().EndDate = currentDate;

            // Log billet
            Positions.Add(new Billet(position, currentDate));

            // Point position references to this soldier
            position.Holder = this;
            Position = position;
        }

        /// <summary>
        /// Retires the soldier by releaseing this instance from their <see cref="Position"/>
        /// </summary>
        public void Retire()
        {
            if (Position != null)
            {
                Position.Holder = null;
                Position = null;
            }
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
            PromotableStatus type;
            return IsPromotable(currentDate, out type);
        }

        /// <summary>
        /// Returns whether the soldier is currently promotable
        /// </summary>
        /// <param name="currentDate">The current date in the simulation</param>
        /// <returns></returns>
        public bool IsPromotable(DateTime currentDate, out PromotableStatus status)
        {
            // Check if soldier is promotable based on TIG
            int months = currentDate.MonthDifference(LastPromotionDate);
            bool promotable = RankInfo.PromotableAt <= months;

            // If the soldier is promotable, then set the status
            if (promotable)
            {
                if (RankInfo.AutoPromotion)
                {
                    status = PromotableStatus.Automatic;
                    return true;
                }
                else if (RankInfo.Grade < Position.Grade)
                {
                    status = PromotableStatus.Position;
                    return true;
                }
                else
                {
                    status = PromotableStatus.Normal;
                    return true;
                }
            }
            else
            {
                status = PromotableStatus.None;
                return false;
            }
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

        public bool Equals(Soldier other)
        {
            if (other == null) return false;
            return (SpawnId == other.SpawnId);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Soldier);
        }

        public override int GetHashCode() => SpawnId;

        /// <summary>
        /// Returns a new instance of <see cref="Soldier"/> using the exact values of
        /// <see cref="Soldier.TimeToLive"/> and <see cref="Soldier.OneInThousandProbability"/>
        /// </summary>
        /// <returns></returns>
        public object Clone() => new Soldier(OneInThousandProbability, TimeToLive);

        /// <summary>
        /// Converts this object into the soldiers name
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{FirstName} {LastName}";
    }
}
