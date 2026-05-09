using Perscom.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using Perscom.Collections;

namespace Perscom.Simulation.Procedures
{
    /// <summary>
    /// Represents a soldier selection procedure that only allows for forward grade
    /// promotions.
    /// </summary>
    public class PromotionOnlyProcedure : AbstractSelectionProcedure
    {
        public PromotionOnlyProcedure(SimDatabase db, PositionBlueprint blueprint) : base(db, blueprint)
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
            type = SpawnSoldierType.TakeFromExistingPool;

            UnitWrapper topUnit = position.PromotionPoolUnit;
            Rank positionRank = position.BlueprintWrapper.Rank;
            
            // TODO 1. Grab the candidate list from the top unit and find the 
            var key = (positionRank.Id, position.BlueprintWrapper.Occupation?.Id ?? -1);
            var cand = topUnit.PromotableCandidates[key];

            // 2. Filter
            var primeSoldiers = GetEligibleSoldierPool(topUnit, positionRank);
            for (int i = primeSoldiers.Count; i >= 0; i--)
            {
                if (!IsCanidateForPosition(primeSoldiers[i], position, date))
                    primeSoldiers.Remove(primeSoldiers[i]);
            }

            if (primeSoldiers.Count == 0)
                return null;

            // 3. Grouping
            if (Grouping.Count > 0)
                primeSoldiers = SoldierSelectionHelper.GroupAndGetPrime(primeSoldiers, Grouping, date);

            if (primeSoldiers.Count == 0)
                throw new Exception("Group has no prime soldiers, but there was a soldier count");

            // 4. Sorting
            if (Sorting.Count > 0)
                SoldierSelectionHelper.SortSoldiers(primeSoldiers, Sorting, date);
            else
                SoldierSelectionHelper.SortSoldiersDescending(primeSoldiers, x => x.GetLateralSelectionFactor(position, date));

            return primeSoldiers.Count > 0 ? primeSoldiers[0] : null;
        }
    }
}
