using Perscom.Database;
using System;
using Perscom.Collections;

namespace Perscom.Simulation.Procedures
{
    /// <summary>
    /// Represents a soldier selection procedure that only allows for Lateral
    /// promotions.
    /// </summary>
    public class LateralOnlyProcedure : AbstractSelectionProcedure
    {
        public LateralOnlyProcedure(SimDatabase db, PositionBlueprint blueprint) : base(db, blueprint)
        {
            // Nothing to do here...
        }

        /// <summary>
        /// Overrides <see cref="AbstractSelectionProcedure.SelectCandidate(PositionWrapper, IterationDate, out SpawnSoldierType)"/>
        /// </summary>
        public override SoldierWrapper SelectCandidate(PositionWrapper position, IterationDate date, out SpawnSoldierType type)
        {
            type = SpawnSoldierType.TakeFromExistingPool;

            if (position.BlueprintWrapper.Id != Blueprint.Id)
                throw new ArgumentException("Position billet does not match this Billet");

            var topUnit = position.PromotionPoolUnit;
            var val = (position.BlueprintWrapper.FillProcedure == SelectionProcedure.LateralOnly) ? 3 : 2;
            
            // If there is no soldier pool for this rank, return null
            if (!topUnit.SoldiersByRank.TryGetValue(position.BlueprintWrapper.Rank.Id, out var soldierPool) || soldierPool.Count == 0)
                return null;

            // 1. Filter
            var primeSoldiers = new DenseList<SoldierWrapper>(soldierPool.Count);
            foreach (var s in soldierPool)
            {
                if (IsCanidateForPosition(s, position, date) && GetLateralPromotionGroupId(s, position, date) <= val)
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
