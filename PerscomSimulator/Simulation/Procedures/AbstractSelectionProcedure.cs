using Perscom.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using Perscom.Collections;

namespace Perscom.Simulation.Procedures
{
    /// <summary>
    /// Represents a base soldier selection procedure for billets
    /// </summary>
    public abstract class AbstractSelectionProcedure
    {
        /// <summary>
        /// The billet this Selection Procedure belongs to
        /// </summary>
        public PositionBlueprint Blueprint { get; protected set; }

        public List<SelectionFilter> Filters { get; protected set; }
        public List<SelectionGroup> Grouping { get; protected set; }
        public List<SelectionSorting> Sorting { get; protected set; }
        
        protected DenseList<SoldierWrapper> PrimeSoldiers { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="AbstractSelectionProcedure"/>
        /// </summary>
        /// <param name="db"></param>
        /// <param name="blueprint"></param>
        public AbstractSelectionProcedure(SimDatabase db, PositionBlueprint blueprint)
        {
            this.Blueprint = blueprint ?? throw new ArgumentNullException();

            // Designate grouping, filtering and sorting
            Filters = db.SelectionFilters.FindAll(blueprint.Id).OrderBy(x => x.Precedence).ToList();
            Grouping = db.SelectionGroups.FindAll(blueprint.Id).OrderBy(x => x.Precedence).ToList();
            Sorting = db.SelectionSortings.FindAll(blueprint.Id).OrderBy(x => x.Precedence).ToList();
            
            PrimeSoldiers = new DenseList<SoldierWrapper>();
        }

        /// <summary>
        /// Gets the best candidate for the provided position based off of the selected procedure option,
        /// as well as the <see cref="Database.PositionBlueprint"/>'s filtering, grouping and sorting of the <see cref="Soldier"/>'s
        /// within the <see cref="Position"/>'s promotion pool.
        /// </summary>
        /// <param name="position">The position to be filled</param>
        /// <param name="currentDate">The current <see cref="IterationDate"/> of the <see cref="Simulator"/></param>
        /// <param name="type"></param>
        /// <returns>Fetches the best candidate or null if there are no condidate's</returns>
        public abstract SoldierWrapper SelectCandidate(PositionWrapper position, IterationDate currentDate, out SpawnSoldierType type);

        /// <summary>
        /// Gets the best candidate for the provided position based off of selected procedure option,
        /// as well as ensuring candidacy to the other position.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public virtual SoldierWrapper FindLateralCandidate(PositionWrapper position, IterationDate date)
        {
            if (position.BlueprintWrapper.Id != Blueprint.Id)
                throw new ArgumentException("Position billet does not match this Billet");

            if (position.IsEmpty)
                throw new Exception("Position is empty");

            SelectionProcedure[] illegalSelections = {
                SelectionProcedure.PromotionOnly
            };

            var topUnit = position.PromotionPoolUnit;
            var soldierPool = GetEligibleSoldierPool(topUnit, position.BlueprintWrapper.Rank);
            var val = (position.BlueprintWrapper.FillProcedure == SelectionProcedure.LateralOnly) ? 3 : 2;

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

            // Loop through sorted candidates checking lateral eligibility
            for (int i = 0; i < primeSoldiers.Count; i++)
            {
                var soldier = primeSoldiers[i];
                var lateralPosition = soldier.Position;

                if (illegalSelections.Contains(lateralPosition.BlueprintWrapper.FillProcedure))
                    continue;

                return soldier;
            }

            return null;
        }

        /// <summary>
        /// Returns a group rating for a soldier that is a candidate for a lateral promotion.
        /// A higher returned number indicates a reduced need or desire for a lateral promotion.
        /// </summary>
        /// <param name="soldier"></param>
        /// <param name="position">The position we are potentially moving into</param>
        /// <param name="date">The current <see cref="IterationDate"/></param>
        /// <returns></returns>
        /// <remarks>
        /// 4 = Soldier Cant really move...
        /// 3 = Downgrade position based on stature, Soldier doesn't want to
        /// 2 = Upgrade to current position, Soldier wants to
        /// 1 = Soldier should To move up very soon (Past MaxTourLength [Waiverable] or Very Near [Not Waiverable])
        /// 0 = Soldier needs to move NOW (Not Waiverable, Will be forced to retire)
        /// </remarks>
        protected virtual int GetLateralPromotionGroupId(SoldierWrapper soldier, PositionWrapper position, IterationDate date)
        {
            //
            // CAN WE EVEN?
            //
            if (soldier.IsLockedInPosition(date))
            {
                if (!soldier.Position.BlueprintWrapper.Blueprint.CanLateralEarly)
                    return 4;
            }

            //
            // DO WE NEED TO?
            //

            // If we are getting close to our maximum tour length,
            // or we have surpassed our max tour length, return true
            if (soldier.IsNearMaxTourLength(date))
            {
                if (soldier.Position.BlueprintWrapper.Blueprint.Waiverable)
                {
                    // We'll take it just for a change of scenery
                    return (soldier.IsPastMaxTourLength(date)) ? 1 : 2;
                }
                else
                {
                    // We are nearing max tour length, and the position
                    // is NOT repeatable... shit! This is high candidacy
                    return (soldier.IsPastMaxTourLength(date)) ? 0 : 1;
                }
            }

            //
            // Do We WANT to?
            //

            // If the stature is higher, OF COURSE we want it!
            return (soldier.Position.BlueprintWrapper.Stature < position.BlueprintWrapper.Stature) ? 2 : 3;
        }

        /// <summary>
        /// This method determines if the specified soldier meets the requirements
        /// to accept the specified position
        /// </summary>
        /// <param name="soldier"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        protected virtual bool IsCanidateForPosition(SoldierWrapper soldier, PositionWrapper position, IterationDate date)
        {
            // Prevent multiple calls to the database for the same information
            int pRankPayGrade = position.BlueprintWrapper.Rank.Classification.PayGrade;
            int sRankPayGrade = soldier.Rank.Classification.PayGrade;

            // A soldier can only can move once per iteration!
            // Positions are ordered at the start of the simulation by
            // PayGrade and Stature anyways, so it works out
            if (soldier.Assignment.EntryIterationId == date.Id)
                return false;

            // Make sure we are not retiring THIS round!
            if (soldier.IsRetiring(date))
                return false;

            // Don't move to the same billet we already sitting in
            if (position.BlueprintWrapper.Id == soldier.Position.BlueprintWrapper.Id)
                return false;

            // Quit if this is a lateral only position
            if (position.BlueprintWrapper.FillProcedure == SelectionProcedure.LateralOnly && (pRankPayGrade != sRankPayGrade))
                return false;

            // Is there a MOS requirement?
            if (position.BlueprintWrapper.RequiredOccupations.Length > 0)
            {
                if (position.BlueprintWrapper.RequiredOccupations.Contains(soldier.Entity.OccupationId))
                {
                    // If requirements are inversed, that means the soldier 
                    // MUST NOT have the required specialty to be a canidate!
                    if (position.BlueprintWrapper.Blueprint.InverseSpecialtyRequirements)
                    {
                        return false;
                    }
                }
                else if (!position.BlueprintWrapper.Blueprint.InverseSpecialtyRequirements)
                {
                    // Position required the specialty, but this soldier
                    // does not have it!
                    return false;
                }
            }

            // Apply Billet Filtering
            foreach (var filter in Filters)
            {
                if (!soldier.EvaluateFilter(filter, date))
                    return false;
            }

            // Check if we are under ranked, and if so, check for position lock
            if (soldier.IsStandIn())
            {
                // Is this position an even higher grade than what we have?
                if (pRankPayGrade > sRankPayGrade)
                {
                    return true;
                }
                else
                {
                    // Never laterally move, when locked into a position, if we are a stand in
                    return (!soldier.IsLockedInPosition(date));
                }
            }

            // Are we locked into our current billet?
            if (soldier.IsLockedInPosition(date))
            {
                // is this a promotion?
                bool isPromotion = (sRankPayGrade < pRankPayGrade);
                bool isLateral = (sRankPayGrade == pRankPayGrade);
                if (isPromotion && soldier.Position.BlueprintWrapper.Blueprint.CanBePromotedEarly)
                    return true;
                else if (isLateral && soldier.Position.BlueprintWrapper.Blueprint.CanLateralEarly)
                    return true;
                else
                    return false;
            }

            // if we are here, we meet all requirements!
            return true;
        }

        /// <summary>
        /// Retrieves a list of eligible soldiers for the specified unit and position rank,
        /// considering the maximum depth of hierarchical traversal.
        /// </summary>
        /// <param name="topUnit">The top-level unit from which to begin the search for eligible soldiers.</param>
        /// <param name="positionRank">The rank of the position for which eligibility is being determined.</param>
        /// <param name="maxDepth">The maximum depth to traverse within the unit hierarchy. Defaults to 2 if not specified.</param>
        /// <returns>A <see cref="DenseList{T}"/> containing the eligible soldiers.</returns>
        public static DenseList<SoldierWrapper> GetEligibleSoldierPool(
            UnitWrapper topUnit,
            Rank positionRank,
            int maxDepth = 2)
        {
            var result = new DenseList<SoldierWrapper>();
            CollectFeeders(topUnit, positionRank, result, maxDepth);
            return result;
        }

        /// <summary>
        /// Recursively collects feeder soldiers based on the target rank and adds them to the provided result list.
        /// </summary>
        /// <param name="topUnit">The top-level unit to search for soldiers.</param>
        /// <param name="targetRank">The rank being targeted for feeder soldier collection.</param>
        /// <param name="result">The list where the found soldiers will be added.</param>
        /// <param name="remainingDepth">The maximum depth allowed for recursion in the unit hierarchy.</param>
        private static void CollectFeeders(
            UnitWrapper topUnit,
            Rank targetRank,
            DenseList<SoldierWrapper> result,
            int remainingDepth)
        {
            if (remainingDepth <= 0) return;

            if (targetRank.Classification.HasSplitRankLanes 
                && RankCache.FeederRankMap.TryGetValue(targetRank.Id, out var feederRankIds))
            {
                // Branching: only pull from specific feeder ranks
                for (int i = 0; i < feederRankIds.Count; i++)
                {
                    int feederId = feederRankIds[i];
                    if (topUnit.SoldiersByRank.TryGetValue(feederId, out var soldiers) && soldiers.Count > 0)
                    {
                        foreach (var s in soldiers)
                            result.Add(s);
                    }
                    else if (RankCache.RanksById.TryGetValue(feederId, out var feederRank))
                    {
                        // No soldiers at this feeder rank — walk deeper
                        CollectFeeders(topUnit, feederRank, result, remainingDepth - 1);
                    }
                }
            }
            else
            {
                // Non-branching: pull all soldiers from previous pay grade
                int targetGrade = targetRank.PayGrade - 1;
                if (targetGrade < 1) return;

                RankType targetType = targetRank.Type;
                var gradeKey = (targetType, targetGrade);

                if (topUnit.SoldiersByGrade.TryGetValue(gradeKey, out var gradePool) && gradePool.Count > 0)
                {
                    foreach (var s in gradePool)
                        result.Add(s);
                }
                else
                {
                    // Nobody at grade-1, walk deeper
                    if (RankCache.RanksByGrade.TryGetValue(targetType, out var gradeMap)
                        && gradeMap.TryGetValue(targetGrade, out var ranksAtGrade))
                    {
                        for (int i = 0; i < ranksAtGrade.Count; i++)
                        {
                            CollectFeeders(topUnit, ranksAtGrade[i], result, remainingDepth - 1);
                        }
                    }
                }
            }
        }
    }
}
