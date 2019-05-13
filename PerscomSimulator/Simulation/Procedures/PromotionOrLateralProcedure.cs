using Perscom.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Perscom.Simulation.Procedures
{
    /// <summary>
    /// Represents a soldier selection procedure that only for forward
    /// promotions as well as lateral promotions.
    /// </summary>
    public class PromotionOrLateralProcedure : AbstractSelectionProcedure
    {
        public PromotionOrLateralProcedure(SimDatabase db, Billet billet) : base(db, billet)
        {
            // Nothing to do here...
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
            int grade = position.Billet.Rank.Grade;

            // Keep searching until we either find a soldier to
            // fill the slot, or run out of rank/grades to pull from.
            while (grade > 0)
            {
                // Grab soldier list
                primeSoldiers = topUnit.SoldiersByGrade[rType][grade].Values;
                IOrderedEnumerable<SoldierWrapper> soldiers;
                bool isLateral = (grade == position.Billet.Rank.Grade);

                // Lateral movement or promotion? The filtering is different!
                if (isLateral)
                {
                    // If this is lateral ONLY position, than we MUST force
                    // people to move from higher stature units, otherwise the position
                    // could be empty forver!
                    int val = (position.Billet.Selection == SelectionProcedure.LateralOnly) ? 3 : 2;

                    //
                    // 1. Apply initial filter, ensuring canadiatcy, and any kind of desire
                    //
                    primeSoldiers = primeSoldiers.Where(
                        x => IsCanidateForPosition(x, position, date) && GetLateralPromotionGroupId(x, position, date) <= val
                    );
                }
                else
                {
                    //
                    // 1. Apply initial filter, ensuring canadiatcy
                    //
                    primeSoldiers = primeSoldiers.Where(x => IsCanidateForPosition(x, position, date));
                }

                // Do we have any soldiers?
                if (primeSoldiers.Count() == 0)
                {
                    --grade;
                    continue;
                }

                //
                // 2. Apply Billet grouping
                //
                IEnumerable<SoldierGroupResult> groups = null;
                if (isLateral)
                {
                    // Do we have selection grouping as well?
                    if (Grouping.Count > 0)
                    {
                        // Apply lateral desire grouping (Need, Want, Dont Want), Then By billet grouping
                        groups = primeSoldiers.GroupSoldiersBy(
                            x => GetLateralPromotionGroupId(x, position, date),
                            Grouping,
                            date
                        );
                    }
                    else
                    {
                        groups = primeSoldiers.GroupSoldiersBy(x => GetLateralPromotionGroupId(x, position, date));
                    }

                    // Get topmost group with at least one soldier in it
                    primeSoldiers = groups.GetPrimeSoldiers();
                }
                else if (Grouping.Count > 0)
                {
                    // Apply groups
                    groups = primeSoldiers.GroupSoldiersBy(Grouping, date);

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
