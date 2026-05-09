using Perscom.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using Perscom.Collections;

namespace Perscom.Simulation.Procedures
{
    /// <summary>
    /// Represents a soldier selection procedure that only for forward
    /// promotions as well as lateral promotions.
    /// </summary>
    public class PromotionOrLateralProcedure : AbstractSelectionProcedure
    {
        public PromotionOrLateralProcedure(SimDatabase db, PositionBlueprint blueprint) : base(db, blueprint)
        {
            // Nothing to do here...
        }

        /// <summary>
        /// Overrides <see cref="AbstractSelectionProcedure.SelectCandidate(PositionWrapper, IterationDate, out SpawnSoldierType)"/>
        /// </summary>
        public override SoldierWrapper SelectCandidate(PositionWrapper position, IterationDate date, out SpawnSoldierType type)
        {
            type = SpawnSoldierType.TakeFromExistingPool;

            UnitWrapper topUnit = position.PromotionPoolUnit;
            Rank positionRank = position.BlueprintWrapper.Rank;

            // --- Try lateral first (same rank) ---
            if (topUnit.SoldiersByRank.TryGetValue(positionRank.Id, out var lateralPool) && lateralPool.Count > 0)
            {
                int val = (position.BlueprintWrapper.FillProcedure == SelectionProcedure.LateralOnly) ? 3 : 2;
                var lateralCandidates = new DenseList<SoldierWrapper>();
                foreach (var s in lateralPool)
                {
                    if (IsCanidateForPosition(s, position, date) && GetLateralPromotionGroupId(s, position, date) <= val)
                        lateralCandidates.Add(s);
                }

                if (lateralCandidates.Count > 0)
                {
                    // Grouping + Sorting (same as before)
                    if (Grouping.Count > 0)
                        lateralCandidates = SoldierSelectionHelper.GroupByThenGetPrime(
                            lateralCandidates, x => GetLateralPromotionGroupId(x, position, date), Grouping, date);
                    else
                        lateralCandidates = SoldierSelectionHelper.GroupByAndGetPrime(
                            lateralCandidates, x => GetLateralPromotionGroupId(x, position, date));

                    if (Sorting.Count > 0)
                        SoldierSelectionHelper.SortSoldiers(lateralCandidates, Sorting, date);
                    else
                        SoldierSelectionHelper.SortSoldiersDescending(lateralCandidates, x => x.GetLateralSelectionFactor(position, date));

                    if (lateralCandidates.Count > 0)
                        return lateralCandidates[0];
                }
            }

            // --- Try promotion (feeder ranks) ---
            var promoPool = GetEligibleSoldierPool(topUnit, positionRank);
            var primeSoldiers = new DenseList<SoldierWrapper>(promoPool.Count);
            for (int i = 0; i < promoPool.Count; i++)
            {
                if (IsCanidateForPosition(promoPool[i], position, date))
                    primeSoldiers.Add(promoPool[i]);
            }

            if (primeSoldiers.Count == 0)
                return null;

            if (Grouping.Count > 0)
                primeSoldiers = SoldierSelectionHelper.GroupAndGetPrime(primeSoldiers, Grouping, date);

            if (Sorting.Count > 0)
                SoldierSelectionHelper.SortSoldiers(primeSoldiers, Sorting, date);
            else
                SoldierSelectionHelper.SortSoldiersDescending(primeSoldiers, x => x.GetLateralSelectionFactor(position, date));

            return primeSoldiers.Count > 0 ? primeSoldiers[0] : null;
        }
    }
}
