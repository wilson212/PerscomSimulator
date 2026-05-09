using System;
using System.Collections.Generic;
using System.Linq;
using Perscom.Collections;
using Perscom.Database;

namespace Perscom.Simulation
{
    /// <summary>
    /// Represents a wrapper class for managing a hierarchical military unit structure,
    /// providing functionality for managing subunits, positions, soldiers, and promotion chains.
    /// </summary>
    public class UnitWrapper
    {
        /// <summary>
        /// Gets or Sets the name of this Unit instance
        /// </summary>
        public string Name => Unit.Name;

        /// <summary>
        /// The parent unit of this unit instance
        /// </summary>
        public Unit Unit { get; set; }

        /// <summary>
        /// Gets the <see cref="Database.Echelon"/> level of this unit
        /// </summary>
        public Echelon Echelon { get; set; }

        /// <summary>
        /// The parent unit of this unit instance
        /// </summary>
        public UnitWrapper Parent { get; set; }
        
        /// <summary>
        /// Gets the promotion boards available at this unit level, keyed by the target RankId
        /// </summary>
        public KeyedList<(int RankId, int? OccupationId), CandidatePool> PromotableCandidates { get; set; }

        /// <summary>
        /// Gets the topmost unit in which this unit's billets
        /// can pull soldiers from
        /// </summary>
        public UnitWrapper PromotionPoolUnit { get; set; }
        
        /// <summary>
        /// Gets or Sets the <see cref="Database.Faction"/> this unit belongs to.
        /// </summary>
        public int FactionId { get; set; }
        
        /// <summary>
        /// Gets or Sets a list of all soldier positions in this Unit (excluding sub units).
        /// </summary>
        public DenseList<PositionWrapper> Positions { get; set; }

        /// <summary>
        /// A list of all <see cref="Unit"/>s that fall under this one
        /// </summary>
        public List<UnitWrapper> Subunits { get; set; } = [];

        // 1. Instant lookup by Soldier ID (For when you need a specific guy)
        public IdentityList<int, SoldierWrapper> AllSoldiers { get; set; }

        /// <summary>
        /// Gets or sets a dictionary where the key represents the rank,
        /// and the value is a keyed list of soldiers associated with that rank.
        /// </summary>
        public Dictionary<int, IdentityList<int, SoldierWrapper>> SoldiersByRank { get; set; }

        /// <summary>
        /// Gets or sets a mapping of soldier groupings based on their grade (combination of rank type and identifier).
        /// Each key is a tuple consisting of a <see cref="RankType"/> and an integer representing the grade,
        /// and the value is a <see cref="DenseList{T}"/> of <see cref="SoldierWrapper"/> instances.
        /// </summary>
        public Dictionary<(RankType, int), DenseList<SoldierWrapper>> SoldiersByGrade { get; set; } = new();

        /// <summary>
        /// Creates a new instance of UnitWrapper
        /// </summary>
        public UnitWrapper(Unit unit, UnitTemplateWrapper wrapper, UnitWrapper parent)
        {
            Unit = unit;
            Parent = parent;
            Echelon = wrapper.Echelon;
            var stats = UnitBuilder.GetUnitStatistics(wrapper.Blueprint);
            
            Positions = new DenseList<PositionWrapper>(stats.PositionCount + 1);
            AllSoldiers = new IdentityList<int, SoldierWrapper>(stats.TotalSoldiers);
            SoldiersByRank = new();
            PromotableCandidates = new ();

            // Get our soldier promotion pool
            // @todo when loading from the database mid-simulation
        }

        /// <summary>
        /// Returns a List of all positions in this unit, and it's sub units.
        /// </summary>
        /// <returns></returns>
        public void GetAllPositions(List<PositionWrapper> bufferList)
        {
            // Add our positions
            bufferList.AddRange(Positions);
    
            // Tell the kids to add theirs to the SAME list
            foreach (UnitWrapper sub in Subunits)
            {
                sub.GetAllPositions(bufferList);
            }
        }

        /// <summary>
        /// Adds a soldier to the unit and updates relevant data structures.
        /// </summary>
        /// <param name="soldier">The soldier to be added to the unit.</param>
        public void AddSoldier(SoldierWrapper soldier)
        {
            if (soldier == null) return;
    
            var rank = soldier.Rank;
            SoldiersByRank[rank.Id].Add(soldier);
            AllSoldiers.Add(soldier);
    
            var gradeKey = (rank.Type, rank.PayGrade);
            if (!SoldiersByGrade.TryGetValue(gradeKey, out var gradeList))
            {
                var stats = UnitBuilder.GetUnitStatistics(Unit.Blueprint);
                int capacity = stats.SoldierCountsByGrade.GetValueOrDefault(gradeKey, 8);
                gradeList = new DenseList<SoldierWrapper>(capacity);
                SoldiersByGrade[gradeKey] = gradeList;
            }
            gradeList.Add(soldier);

            // Recursive
            Parent?.AddSoldier(soldier);
        }

        /// <summary>
        /// Removes the <see cref="Soldier"/> to the unit roster.
        /// </summary>
        /// <param name="soldier"></param>
        public void RemoveSoldier(SoldierWrapper soldier)
        {
            if (soldier == null) return;

            if (!AllSoldiers.ContainsKey(soldier.Entity.Id))
            {
                // We have a major problem!
                throw new Exception("Soldier not found in unit");
            }

            var rank = soldier.Rank;
            SoldiersByRank[rank.Id].Remove(soldier);
            AllSoldiers.Remove(soldier);
            
            var gradeKey = (rank.Type, rank.PayGrade);
            SoldiersByGrade[gradeKey].Remove(soldier);

            // Check if soldier exists
            if (AllSoldiers.ContainsKey(soldier.Entity.Id))
            {
                // We have a major problem!
                throw new Exception("Soldier is not being removed properly from UnitWrapper!");
            }

            // Recursive
            Parent?.RemoveSoldier(soldier);
        }

        /// <summary>
        /// Deregisters a soldier from the promotable candidates in the current unit and all parent units up the chain.
        /// </summary>
        /// <param name="candidate">The soldier to be deregistered from the promotable candidates pool.</param>
        public void DeregisterPromotableUpChain(SoldierWrapper candidate)
        {
            var currentUnit = this;
            int targetRankId = candidate.Entity.TargetRankId.Value;
            int? occupationId = candidate.Occupation?.Id;

            // Walk up the chain and remove the pointer from every echelon's dictionary
            while (currentUnit != null)
            {
                var key = (targetRankId, occupationId);
                if (currentUnit.PromotableCandidates.TryGetValue(key, out var pool))
                {
                    pool.RemoveCandidate(candidate);
                }

                if (occupationId != null)
                {
                    key = (targetRankId, null);
                    if (currentUnit.PromotableCandidates.TryGetValue(key, out pool))
                    {
                        pool.RemoveCandidate(candidate);
                    }
                }

                currentUnit = currentUnit.Parent;
            }
        }

        /// <summary>
        /// Registers a promotable candidate into the appropriate promotion pools
        /// across the chain of parent units.
        /// </summary>
        /// <param name="candidate">The promotable candidate to register within the promotion pool.</param>
        /// <param name="targetRankId"></param>
        /// <param name="occupationId"></param>
        /// <param name="board"></param>
        public void RegisterPromotableUpChain(PromotableCandidate candidate, int targetRankId, int? occupationId, PromotionBoardWrapper board)
        {
            var currentUnit = this;

            while (currentUnit != null)
            {
                var key = (targetRankId, occupationId);
                if (!currentUnit.PromotableCandidates.TryGetValue(key, out var pool))
                {
                    pool = new CandidatePool(board);
                    currentUnit.PromotableCandidates.Add(key, pool);
                }
                pool.AddCandidate(candidate);

                if (occupationId != null)
                {
                    var genericKey = (targetRankId, (int?)null);
                    if (!currentUnit.PromotableCandidates.TryGetValue(genericKey, out pool))
                    {
                        pool = new CandidatePool(board);
                        currentUnit.PromotableCandidates.Add(genericKey, pool);
                    }
                    pool.AddCandidate(candidate);
                }

                currentUnit = currentUnit.Parent;
            }
        }

        /// <summary>
        /// Calculates a score for a candidate soldier for an open position based on performance models
        /// and the candidate's attributes.
        /// </summary>
        /// <param name="candidate">The soldier being evaluated for the position.</param>
        /// <param name="openPosition">The position for which the candidate is being evaluated.</param>
        /// <returns>The calculated score as a double value, representing the suitability of the candidate
        /// for the specified position.</returns>
        private double CalculateScoreForPosition(SoldierWrapper candidate, PositionWrapper openPosition)
        {
            double score = 0;

            // Use the position blueprint's performance model weights
            var blueprint = openPosition.BlueprintWrapper;
            if (blueprint?.PerformanceModels != null)
            {
                foreach (var weight in blueprint.PerformanceModels)
                {
                    if (candidate.AttributesWithModifiers.TryGetValue(weight.Key, out int attrValue))
                    {
                        score += attrValue * weight.Value;
                    }
                }
            }

            // Factor in Form Rating
            score += candidate.CalculateFormRating() * 10.0;

            return score;
        }

        /// <summary>
        /// Retrieves a list of soldiers currently assigned to the positions
        /// within this unit.
        /// </summary>
        /// <returns>
        /// A list of <c>SoldierWrapper</c> objects representing the soldiers
        /// assigned to the positions of this unit.
        /// </returns>
        public List<SoldierWrapper> GetAssignedSoldiers()
        {
            return Positions.Where(p => p.Holder != null).Select(p => p.Holder).ToList();
        }

        /// <summary>
        /// Returns a string representation of the current UnitWrapper instance.
        /// </summary>
        /// <returns>A string containing the name of the unit, and if applicable, concatenated with the name of the parent UnitWrapper.</returns>
        public override string ToString()
        {
            return (Parent == null) ? Name : String.Concat(Name, ", ", Parent.ToString());
        }
    }
}
