using Perscom.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using Perscom.Collections;

namespace Perscom.Simulation.Procedures
{
    /// <summary>
    /// Represents a fully user-configured soldier selection procedure that relies
    /// entirely on the Filter/Group/Sort pipeline defined on the <see cref="PositionBlueprint"/>.
    /// Replaces the old Ordered and Randomized procedures.
    /// </summary>
    public class CustomSelectionProcedure : AbstractSelectionProcedure
    {
        public CustomSelectionProcedure(SimDatabase db, PositionBlueprint blueprint) : base(db, blueprint)
        {
        }

        /// <summary>
        /// Selects the best candidate using the user-configured Filter/Group/Sort pipeline.
        /// </summary>
        public override SoldierWrapper SelectCandidate(PositionWrapper position, IterationDate date, out SpawnSoldierType type)
        {
            type = SpawnSoldierType.TakeFromExistingPool;

            if (position.BlueprintWrapper.Id != Blueprint.Id)
                throw new ArgumentException("Position billet does not match this Billet");

            var topUnit = position.PromotionPoolUnit;
            var soldierPool = GetEligibleSoldierPool(topUnit, position.BlueprintWrapper.Rank);

            // 1. Filter
            var primeSoldiers = new DenseList<SoldierWrapper>();
            foreach (var s in soldierPool)
            {
                if (IsCanidateForPosition(s, position, date) && GetLateralPromotionGroupId(s, position, date) <= 3)
                    primeSoldiers.Add(s);
            }

            if (primeSoldiers.Count == 0)
                return null;

            // 2. Grouping
            if (Grouping.Count > 0)
            {
                primeSoldiers = SoldierSelectionHelper.GroupByThenGetPrime(
                    primeSoldiers,
                    x => GetLateralPromotionGroupId(x, position, date),
                    Grouping,
                    date
                );
            }
            else
            {
                primeSoldiers = SoldierSelectionHelper.GroupByAndGetPrime(
                    primeSoldiers,
                    x => GetLateralPromotionGroupId(x, position, date)
                );
            }

            if (primeSoldiers.Count == 0)
                throw new Exception("Group has no prime soldiers, but there was a soldier count");

            // 3. Sorting
            if (Sorting.Count > 0)
            {
                SoldierSelectionHelper.SortSoldiers(primeSoldiers, Sorting, date);
            }
            else
            {
                SoldierSelectionHelper.SortSoldiersDescending(primeSoldiers,
                    x => x.GetLateralSelectionFactor(position, date));
            }

            return primeSoldiers.Count > 0 ? primeSoldiers[0] : null;
        }
    }
}
