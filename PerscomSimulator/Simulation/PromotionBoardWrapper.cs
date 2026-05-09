using Perscom.Database;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Perscom.Collections;

namespace Perscom.Simulation
{
    /// <summary>
    /// Represents a wrapper for managing promotion boards and their associated candidates
    /// in the simulation. Provides functionality to evaluate soldiers, convene boards, and
    /// handle candidate management, such as tracking new candidates to be inserted or
    /// expired candidates to be removed.
    /// </summary>
    public class PromotionBoardWrapper
    {
        /// <summary>
        /// Represents the underlying promotion board entity associated with this wrapper.
        /// This property links the promotion process with its corresponding board configuration
        /// and metadata, providing access to rules and thresholds governing promotions.
        /// </summary>
        public PromotionBoard Entity { get; private set; }

        /// <summary>
        /// Represents a collection of promotable candidates participating in the promotion board.
        /// This property holds all the current candidates eligible for evaluation and promotion.
        /// Candidates are dynamically managed based on the iterative evaluation process within the system,
        /// including processes such as addition, removal, and sorting based on promotion criteria.
        /// </summary>
        private KeyedList<int, PromotableCandidate> Candidates { get; set; }

        /// <summary>
        /// Represents a collection of newly identified promotable candidates
        /// that need to be inserted into the database. These candidates are typically
        /// generated during the iterative evaluation process when soldiers meet
        /// specific promotion criteria.
        /// This property is cleared and re-populated at the start of each board convening
        /// to track candidates requiring persistent storage.
        /// </summary>
        private DenseList<PromotableCandidate> NewCandidatesToInsert { get; set; }

        /// <summary>
        /// Represents a list of candidates who have been deemed expired and are scheduled for removal
        /// from the promotion board system. This property is used for tracking candidates that no longer meet
        /// the eligibility criteria, ensuring proper cleanup and synchronization with the database.
        /// Candidates in this list are flagged for database deletion during the board convening process or
        /// upon promotion-related operations.
        /// </summary>
        private DenseList<PromotableCandidate> ExpiredCandidatesToDelete { get; set; }

        /// <summary>
        /// Represents a collection of promotion board results that are pending further processing.
        /// This property stores the results of candidate evaluations temporarily before they are
        /// persisted to the database or used in subsequent operations. It acts as a staging area
        /// during the promotion board simulation workflow.
        /// </summary>
        private DenseList<PromotionBoardResult> PendingResults { get; set; }

        /// <summary>
        /// Indicates whether the list of candidates associated with the promotion board
        /// requires sorting. This property is a flag used to trigger appropriate sorting
        /// operations on candidate collections when their order or priority may have
        /// changed due to new additions, deletions, or updates.
        /// It ensures the integrity of candidate rankings or sequences based on configured
        /// sorting criteria.
        /// </summary>
        protected bool NeedsSorting { get; set; } = true;

        /// <summary>
        /// Represents a thread-safe queue used to temporarily store
        /// promotion board results before they are processed and persisted
        /// to the database. This queue facilitates the asynchronous handling
        /// of evaluation results, ensuring safe concurrent access and efficient
        /// processing during simulation iterations.
        /// </summary>
        private readonly ConcurrentQueue<PromotionBoardResult> _pendingResultsQueue = new();

        /// <summary>
        /// Represents a wrapper for managing promotion boards and their associated candidates
        /// in the simulation. Provides functionality to evaluate soldiers, convene boards, and
        /// handle candidate management, such as tracking new candidates to be inserted or
        /// expired candidates to be removed.
        /// </summary>
        public PromotionBoardWrapper(PromotionBoard entity)
        {
            Entity = entity;
            NewCandidatesToInsert = new();
            ExpiredCandidatesToDelete = new();
            PendingResults = new();
            
            Candidates = new KeyedList<int, PromotableCandidate>(1000);
        }

        /// <summary>
        /// Evaluates a soldier candidate for promotion eligibility based on specific criteria
        /// such as time in grade, form rating, and basic skills. Updates the promotion board
        /// with the candidate's evaluation results.
        /// </summary>
        /// <param name="soldier">The soldier being evaluated for promotion.</param>
        /// <param name="date">The current iteration date used for evaluation calculations.</param>
        /// <param name="db">The simulation database instance to access data and perform updates.</param>
        public bool EvaluateCandidate(SoldierWrapper soldier, IterationDate date, SimDatabase db)
        {
            double score = CalculateCandidateScore(soldier, date);
            var result = db.PromotionBoardResults.Create();
            result.SoldierId = soldier.Entity.Id;
            result.PromotionBoardId = Entity.Id;
            result.IterationId = date.Id;
            result.Score = (int)score;
            result.Passed = !Entity.IsPassFail || score >= Entity.PassThreshold;

            _pendingResultsQueue.Enqueue(result);
            soldier.LatestBoardResult = result;

            return result.Passed;
        }

        /// <summary>
        /// Retrieves a promotable candidate by their unique soldier ID from the list of candidates.
        /// Returns null if the candidate is not found.
        /// </summary>
        /// <param name="soldierId">The unique identifier of the soldier whose promotable candidate information is to be retrieved.</param>
        /// <returns>
        /// A <see cref="PromotableCandidate"/> object representing the soldier's promotable candidate information,
        /// or null if the candidate does not exist in the collection.
        /// </returns>
        public PromotableCandidate GetCandidateById(int soldierId)
        {
            return Candidates.TryGetValue(soldierId, out var candidate) ? candidate : null;
        }

        /// <summary>
        /// Manages the convening of the promotion board for the current iteration,
        /// handling the identification of expired candidates for removal and
        /// resetting tracking lists for new candidates to be added.
        /// </summary>
        /// <remarks>
        /// Called At the Beginning of the Iteration using <see cref="Parallel.ForEach{TSource}(IEnumerable{TSource}, Action{TSource})"/>
        /// </remarks>
        /// <param name="currentIteration">The current iteration date used to evaluate candidates for expiration.</param>
        public void ConveneBoard(IterationDate currentIteration, Simulator simulator)
        {
            // Remove all expired candidates — single-pass O(N) using RemoveAll
            int removed = Candidates.RemoveAll(c =>
            {
                if ((currentIteration.Id - c.IterationAddedId) >= Entity.PromotableLength)
                {
                    ExpiredCandidatesToDelete.Add(c);
                    
                    // Get soldier, and null thier latest board result
                    var soldier = simulator.ActiveDutySoldiers.TryGetValue(c.SoldierId, out var s) ? s : null;
                    if (soldier != null)
                    {
                        soldier.LatestBoardResult = null;
                    }
                    
                    return true;
                }
                return false;
            });
        }

        /// <summary>
        /// Removes a promotable candidate from the list of candidates using their unique soldier ID.
        /// If the candidate is found, it is marked for deletion by adding it to the list of expired candidates.
        /// </summary>
        /// <param name="soldierId">The unique identifier of the soldier whose promotable candidate is to be removed.</param>
        public void RemoveCandidateById(int soldierId)
        {
            var record = GetCandidateById(soldierId);
            if (record != null)
            {
                ExpiredCandidatesToDelete.Add(record);
            }
        }

        /// <summary>
        /// Removes a specified promotable candidate from the tracking system by marking
        /// them for deletion. The candidate is added to the list of expired candidates
        /// that will be removed during the next update process.
        /// </summary>
        /// <param name="candidate">The promotable candidate to be removed from the system.</param>
        public void RemoveCandidate(PromotableCandidate candidate)
        {
            ExpiredCandidatesToDelete.Add(candidate);
        }

        /// <summary>
        /// Applies pending changes to the database by adding new promotable candidates
        /// and removing expired candidates from the database.
        /// </summary>
        /// <remarks>
        /// Called At the End of the Iteration using <see cref="Parallel.ForEach{TSource}(IEnumerable{TSource}, Action{TSource})"/>
        /// </remarks>
        /// <param name="db">An instance of the simulation database where the changes
        /// should be persisted.</param>
        /// <param name="simulator"></param>
        public void SaveChanges(SimDatabase db, Simulator simulator)
        {
            while (_pendingResultsQueue.TryDequeue(out var result))
            {
                // Drain into a list
                PendingResults.Add(result);

                if (result.Passed)
                {
                    var candidate = db.PromotableCandidates.Create();
                    candidate.PromotionBoardId = result.PromotionBoardId;
                    candidate.SoldierId = result.SoldierId;
                    candidate.IterationAddedId = result.IterationId;
                    candidate.CurrentScore = result.Score;

                    Candidates.Add(candidate.SoldierId, candidate);
                    NewCandidatesToInsert.Add(candidate);
                }
            }
            
            // Save pending results
            db.PromotionBoardResults.AddRange(PendingResults);
            
            // Insert new candidates
            db.PromotableCandidates.AddRange(NewCandidatesToInsert);
            foreach (var c in NewCandidatesToInsert)
            {
                if (simulator.ActiveDutySoldiers.TryGetValue(c.SoldierId, out var soldier))
                {
                    soldier.Position?.ParentUnit?.RegisterPromotableUpChain(c, soldier.Entity.TargetRankId.Value, 
                        soldier.Occupation?.Id, this);
                }
            }

            // Delete expired candidates
            db.PromotableCandidates.RemoveRange(ExpiredCandidatesToDelete);
            foreach (var c in ExpiredCandidatesToDelete)
            {
                if (simulator.ActiveDutySoldiers.TryGetValue(c.SoldierId, out var soldier))
                {
                    soldier.Position?.ParentUnit?.DeregisterPromotableUpChain(soldier);
                }
            }
            
            // Clear the tracking lists for this iteration
            NewCandidatesToInsert.Clear();
            ExpiredCandidatesToDelete.Clear();
            PendingResults.Clear();
        }

        /// <summary>
        /// Calculates a numerical score for a candidate based on their attributes, time in grade,
        /// and the user-defined weights configured for the promotion board.
        /// Scores are determined by combining the time in grade factor with weighted attribute evaluations.
        /// </summary>
        /// <param name="soldier">The soldier for whom the score is being calculated. Contains relevant attributes and modifiers.</param>
        /// <param name="date">The current iteration date used to evaluate time in grade and other relevant calculations.</param>
        /// <returns>Returns the calculated score as a double value, representing the candidate's overall evaluation.</returns>
        private double CalculateCandidateScore(SoldierWrapper soldier, IterationDate date)
        {
            // 1. Time in Grade factor
            int tig = soldier.GetTimeInGrade(date);
            double score = Math.Min(tig * Entity.TimeInGradeFactor, Entity.TimeInGradeMaxPoints);

            // 2. Score based on board's user-defined attribute weights
            foreach (var weight in Entity.Weights)
            {
                if (soldier.AttributesWithModifiers.TryGetValue(weight.Attribute, out int attrValue))
                {
                    score += attrValue * weight.Weight;
                }
            }

            return score;
        }
    }
}
