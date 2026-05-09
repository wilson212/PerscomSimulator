using System.Collections.Concurrent;
using System.Collections.Generic;
using Perscom.Collections;
using Perscom.Database;

namespace Perscom.Simulation
{
    /// <summary>
    /// Manages position vacancies within a hierarchical structure, enabling the
    /// processing of vacant positions and filling them in a controlled manner.
    /// Ensures thread-safety where applicable and adheres to a top-down processing
    /// strategy to prioritize high-ranking positions.
    /// </summary>
    public class PositionManager
    {
        // Array index = Echelon Level (e.g., 0 = E-1, 9 = 4-Star General).
        // Using ConcurrentQueue allows your Parallel.ForEach Phase 1 loop 
        // to safely report retirements simultaneously across all threads.
        private readonly ConcurrentQueue<PositionWrapper>[] _vacantPositionsByEchelon;

        /// <summary>
        /// A collection of position objects that are pending processing within
        /// the hierarchical position management system. This list serves as a
        /// temporary working buffer to hold positions that are dequeued from
        /// the current processing queue for further operations, such as evaluating
        /// and filling vacancies.
        /// The list is cleared and reused during each processing cycle to optimize
        /// memory usage and reduce allocations. It is sized dynamically but is
        /// initialized with a default capacity to handle a typical workload.
        /// </summary>
        private readonly DenseList<PositionWrapper> _positionsToProcess = new(1000);

        /// <summary>
        /// Manages vacant positions within a hierarchical structure by organizing them
        /// into echelons. Provides methods to mark positions as vacant and process
        /// vacancies in a thread-safe manner.
        /// </summary>
        /// <remarks>
        /// The class uses a top-down approach to prioritize higher-echelon positions
        /// and ensures thread-safety where necessary by using concurrent collections.
        /// </remarks>
        public PositionManager(int maxEchelon)
        {
            _vacantPositionsByEchelon = new ConcurrentQueue<PositionWrapper>[maxEchelon + 1];
            for (int i = 0; i <= maxEchelon; i++)
            {
                _vacantPositionsByEchelon[i] = new ConcurrentQueue<PositionWrapper>();
            }
        }

        /// <summary>
        /// Thread-safe method called when a soldier retires, dies, or gets promoted OUT of this spot.
        /// </summary>
        /// <remarks>This is multi-thread safe</remarks>
        public void MarkPositionVacant(PositionWrapper emptyPosition)
        {
            if (emptyPosition == null) return;
            if (!emptyPosition.TryMarkVacantQueued()) return; // Already queued — skip
            
            // IMPORTANT: Do NOT Remove the Soldier here!

            int echelon = emptyPosition.ParentUnit.Echelon.HierarchyLevel;
            _vacantPositionsByEchelon[echelon].Enqueue(emptyPosition);
        }

        /// <summary>
        /// Processes all vacancies top-down (Waterfall Cascade). 
        /// Guaranteed to fill the highest-ranking and highest-prestige positions first.
        /// </summary>
        /// <remarks>This is not multi-thread safe</remarks>
        public void ProcessAllVacancies(IterationDate date)
        {
            for (int currentEchelon = _vacantPositionsByEchelon.Length - 1; currentEchelon >= 0; currentEchelon--)
            {
                var currentQueue = _vacantPositionsByEchelon[currentEchelon];
                if (currentQueue.IsEmpty) continue;

                // Track positions that already failed selection THIS echelon pass.
                // This is our infinite-loop guard.
                var failedThisPass = new HashSet<PositionWrapper>();

                // Keep draining until no new vacancies appear at this echelon
                while (!currentQueue.IsEmpty)
                {
                    // Drain into a working list
                    _positionsToProcess.Clear();
                    while (currentQueue.TryDequeue(out var pos))
                    {
                        _positionsToProcess.Add(pos);
                    }

                    foreach (var openPosition in _positionsToProcess)
                    {
                        var selectionProcedure = openPosition.BlueprintWrapper.Procedure;
                        
                        SoldierWrapper winner = selectionProcedure.SelectCandidate(openPosition, date, out var type);
                        if (winner != null)
                        {
                            PositionWrapper oldPosition = winner.Position;
                            openPosition.AssignSoldier(winner);
                            openPosition.ClearVacantQueued();

                            // Remove from a failed set if it was there — it just got filled
                            failedThisPass.Remove(openPosition);

                            if (oldPosition != null)
                            {
                                // Cascade: the old position is now vacant.
                                // If it's at this same echelon, the outer `while` will pick it up.
                                MarkPositionVacant(oldPosition);
                            }
                        }
                        else
                        {
                            // Already failed once this pass? Give up — defer to the next iteration.
                            if (!failedThisPass.Add(openPosition))
                            {
                                _vacantPositionsByEchelon[currentEchelon].Enqueue(openPosition);
                            }
                            else
                            {
                                // First failure — re-enqueue so the inner while loop retries
                                // after cascading fills may have freed up a candidate.
                                _vacantPositionsByEchelon[currentEchelon].Enqueue(openPosition);
                            }
                        }
                    }
                }
            }
        }
    }
}