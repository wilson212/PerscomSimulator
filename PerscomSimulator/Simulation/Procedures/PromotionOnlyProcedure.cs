using Perscom.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Perscom.Simulation.Procedures
{
    /// <summary>
    /// Represents a soldier selection procedure that only allows for forward
    /// promotions.
    /// </summary>
    public class PromotionOnlyProcedure : AbstractSelectionProcedure
    {
        public PromotionOnlyProcedure(SimDatabase db, Billet billet) : base(db, billet)
        {
            // Nothing to do here...
        }

        /// <summary>
        /// Promotion only procedure does not allow for lateral promotions!
        /// </summary>
        /// <param name="position"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public override SoldierWrapper FindLateralCandidate(PositionWrapper position, IterationDate date)
        {
            return null;
        }

        /// <summary>
        /// Overrides <see cref="AbstractSelectionProcedure.SelectCandidate(PositionWrapper, IterationDate, out SpawnSoldierType)"/>
        /// </summary>
        public override SoldierWrapper SelectCandidate(PositionWrapper position, IterationDate date, out SpawnSoldierType type)
        {
            // Set our out variable
            type = SpawnSoldierType.TakeFromExistingPool;

            // Define position specific vars
            IEnumerable<SoldierWrapper> primeSoldiers = null;
            UnitWrapper topUnit = position.PromotionPoolUnit;
            RankType rType = position.Billet.Rank.Type;
            int grade = position.Billet.Rank.Grade - 1;

            // Keep searching until we either find a soldier to
            // fill the slot, or run out of rank/grades to pull from.
            while (grade > 0)
            {
                // Grab soldier list
                primeSoldiers = topUnit.SoldiersByGrade[rType][grade].Values;
                IOrderedEnumerable<SoldierWrapper> soldiers;

                //
                // 1. Apply initial filter, ensuring canadiatcy
                //
                primeSoldiers = primeSoldiers.Where(x => IsCanidateForPosition(x, position, date));

                // Do we have any soldiers?
                if (primeSoldiers.Count() == 0)
                {
                    --grade;
                    continue;
                }

                //
                // 2. Apply Billet grouping
                //
                if (Grouping.Count > 0)
                {
                    // Apply groups
                    var groups = primeSoldiers.GroupSoldiersBy(Grouping, date);

                    // Get topmost group with at least one soldier in it
                    primeSoldiers = groups.GetPrimeSoldiers();
                }

                // Do we have any soldiers?
                if (primeSoldiers.Count() == 0)
                    throw new Exception("Group has no prime soldiers, but there was a soldier count");

                //
                // 3. Apply soldier ordering
                //
                if (Sorting.Count > 0)
                {
                    // Apply sorting
                    soldiers = primeSoldiers.OrderSoldiersBy(Sorting, date);
                }
                else
                {
                    // Apply default sorting
                    soldiers = primeSoldiers.OrderByDescending(x => x.GetTimeInGrade(date));
                }

                // Check for an un-restricted soldier first
                var wrapper = soldiers.FirstOrDefault();
                if (wrapper != null)
                {
                    return wrapper;
                }
                else
                {
                    --grade;
                    continue;
                }
            }

            return null;
        }
    }
}
