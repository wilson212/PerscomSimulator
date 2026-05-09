using System.Collections.Generic;
using Perscom.Database;
using Perscom.Simulation;

namespace Perscom.Collections;

/// <summary>
/// Represents a pool of promotable candidates for a specific rank within a specific Unit.
/// Provides operations for adding, removing, and managing candidate data.
/// This pool is designed to be attached to a <see cref="UnitWrapper"/>
/// </summary>
public class CandidatePool
{
    /// <summary>
    /// Represents the underlying promotion board entity associated with this wrapper.
    /// This property links the promotion process with its corresponding board configuration
    /// and metadata, providing access to rules and thresholds governing promotions.
    /// </summary>
    private PromotionBoardWrapper Board { get; set; }
    
    /// <summary>
    /// Indicates whether the list of candidates associated with the promotion board
    /// requires sorting. This property is a flag used to trigger appropriate sorting
    /// operations on candidate collections when their order or priority may have
    /// changed due to new additions, deletions, or updates.
    /// It ensures the integrity of candidate rankings or sequences based on configured
    /// sorting criteria.
    /// </summary>
    private bool NeedsSorting { get; set; } = true;
    
    /// <summary>
    /// Represents a collection of promotable candidates participating in the promotion board.
    /// This property holds all the current candidates eligible for evaluation and promotion.
    /// Candidates are dynamically managed based on the iterative evaluation process within the system,
    /// including processes such as addition, removal, and sorting based on promotion criteria.
    /// </summary>
    private IdentityList<int, PromotableCandidate> Candidates { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="board"></param>
    /// <param name="existingRoster"></param>
    public CandidatePool(PromotionBoardWrapper board, List<PromotableCandidate> existingRoster = null)
    {
        Board = board;
        Candidates = new IdentityList<int, PromotableCandidate>(existingRoster?.Count ?? 48);
    }

    /// <summary>
    /// Adds the specified candidate to the candidate pool.
    /// </summary>
    /// <param name="candidate">
    /// The candidate to be added, represented by a PromotableCandidate instance.
    /// </param>
    public void AddCandidate(PromotableCandidate candidate)
    {
        Candidates.Add(candidate);
        NeedsSorting = true;
    }

    /// <summary>
    /// Removes the specified candidate from the candidate pool.
    /// </summary>
    /// <param name="candidate">
    /// The candidate to be removed, represented by a SoldierWrapper instance.
    /// </param>
    public void RemoveCandidate(SoldierWrapper candidate)
    {
        Candidates.Remove(candidate.Entity.Id);
        NeedsSorting = true;
    }
    
    /// <summary>
    /// Retrieves the next candidate from the list of promotable candidates,
    /// removes them from the candidate list, and marks them for deletion
    /// as they have been successfully assigned to a position. This method does NOT
    /// check to see if the candidate is retired.
    /// </summary>
    /// <returns>
    /// The next promotable candidate from the list. Returns null if no candidates are available.
    /// </returns>
    public PromotableCandidate GetNextCandidate()
    {
        if (Candidates.Count == 0) return null;
            
        // Ensure we have sorted this iteration
        SortStandingList();

        // Pop the best candidate off the end of the array (O(1) removal, preserves sort)
        if (Candidates.TryPop(out var winner))
        {
            // Flag them to be deleted from the "Waiting List" table since they got the job
            Board.RemoveCandidate(winner);
            return winner;
        }

        return null;
    }
    
    /// <summary>
    /// Sorts the list of candidates within the promotion board based on the board's
    /// type. If the board is of type Ordered Merit List, candidates are sorted in descending
    /// order of their current scores. If the board is of type Sequence Order, candidates
    /// are sorted by the order in which they were added. Once sorting is performed, it
    /// marks the list as sorted to prevent unnecessary re-sorting during subsequent calls.
    /// </summary>
    private void SortStandingList()
    {
        if (!NeedsSorting) return;
    
        // Important, to improve Simulation performance by reducing unnecessary memory-shifting,
        // we are going to sort the list with the lowest scores first
        // Reverse Sorts: We want the "winner" at the very end of the list for O(1) removal.
        if (Board.Entity.Type == PromotionBoardType.OrderedMeritList)
        {
            // Ascending: Lowest score at index 0, Highest score at the end
            Candidates.Sort((a, b) => a.CurrentScore.CompareTo(b.CurrentScore));
        }
        else if (Board.Entity.Type == PromotionBoardType.SequenceOrder)
        {
            // Descending: Newest date at index 0, Oldest date at the end
            Candidates.Sort((a, b) => b.IterationAddedId.CompareTo(a.IterationAddedId));
        }
    
        NeedsSorting = false;
    }
}