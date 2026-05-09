using CrossLite;
using CrossLite.CodeFirst;
using Perscom.Collections;

namespace Perscom.Database
{
    /// <summary>
    /// Represents a candidate eligible for promotion within a specific promotion board.
    /// Each candidate is associated with a soldier, a promotion board, and the iteration
    /// in which they were added. The candidate's eligibility is subject to expiration
    /// based on the promotable length and the current iteration.
    /// </summary>
    [Table(WithoutRowID: true)]
    public class PromotableCandidate : EntityBase, IKeyed<int>
    {
        /// <summary>
        /// The <see cref="Soldier.Id"/> of this candidate
        /// </summary>
        [Column, Required, PrimaryKey]
        public virtual int SoldierId { get; set; }

        /// <summary>
        /// The unique identifier for the associated <see cref="PromotionBoard"/> record.
        /// </summary>
        [Column, Required, PrimaryKey, Index]
        public virtual int PromotionBoardId { get; set; }

        /// <summary>
        /// The iteration (month) they passed the board and got 'P' status
        /// </summary>
        [Column, Required, PrimaryKey, Index]
        public virtual  int IterationAddedId { get; set; }

        /// <summary>
        /// Gets or sets the current score of the candidate
        /// </summary>
        [Column, Required]
        public virtual int CurrentScore { get; set; }

        #region Foreign Key Navigation Properties

        /// <summary>
        /// Gets or sets the <see cref="Database.Soldier"/> entity
        /// </summary>
        [ForeignKey(nameof(SoldierId))]
        [References(nameof(Database.Soldier.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual Soldier Soldier { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="PromotionBoard"/> entity
        /// </summary>
        [ForeignKey(nameof(PromotionBoardId))]
        [References(nameof(Database.PromotionBoard.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual PromotionBoard Board { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="Perscom.Database.IterationDate"/> that 
        /// is tied to this <see cref="PromotableCandidate"/>.
        /// </summary>
        [ForeignKey(nameof(IterationAddedId))]
        [References(nameof(Database.IterationDate.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual IterationDate Iteration { get; set; }

        #endregion
        
        /// <summary>
        /// The unique identifier key for a <see cref="Perscom.Collections.IdentityList{TKey, T}"/>
        /// </summary>
        public int Key => SoldierId;

        public bool IsExpired(int currentIterationId, int promotableLength)
        {
            // If they are on the list longer than the board allows, they lose 'P' status [cite: 4, 6]
            return (currentIterationId - IterationAddedId) >= promotableLength;
        }
    }
}
