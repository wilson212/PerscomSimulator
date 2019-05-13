using Perscom.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Perscom.Simulation.Procedures
{
    /// <summary>
    /// Represents a soldier selection procedure that only allows for Lateral
    /// promotions.
    /// </summary>
    public class LateralOnlyProcedure : AbstractSelectionProcedure
    {
        public LateralOnlyProcedure(SimDatabase db, Billet billet) : base(db, billet)
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

            // Ensure sanity
            if (position.Billet.Id != Billet.Id)
                throw new ArgumentException("Position billet does not match this Billet");

            // Define position specific vars
            UnitWrapper topUnit = position.PromotionPoolUnit;
            RankType rType = position.Billet.Rank.Type;
            int grade = position.Billet.Rank.Grade;

            // Grab soldier list
            IEnumerable<SoldierWrapper> primeSoldiers = topUnit.SoldiersByGrade[rType][grade].Values;
            IOrderedEnumerable<SoldierWrapper> soldiers;
            IEnumerable<SoldierGroupResult> groups = null;

            // We MUST force people to move from higher stature units, otherwise the position
            // could be empty forver!
            int val = (position.Billet.Selection == SelectionProcedure.LateralOnly) ? 3 : 2;

            //
            // 1. Apply initial filter, ensuring candidacy, and any kind of desire
            //
            primeSoldiers = primeSoldiers.Where(
                x => IsCanidateForPosition(x, position, date) && GetLateralPromotionGroupId(x, position, date) <= val
            );

            // Do we have any soldiers?
            if (primeSoldiers.Count() == 0)
            {
                return null;
            }

            //
            // 2. Apply Billet grouping
            //

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

            // Return top-most soldier
            return soldiers.FirstOrDefault();
        }
    }
}
