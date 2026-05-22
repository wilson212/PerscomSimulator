using CrossLite;
using CrossLite.CodeFirst;
using System;

namespace Perscom.Database
{
    /// <summary>
    /// Represents the result of a soldier's participation in a promotion board,
    /// including details such as the associated soldier, board, iteration,
    /// score, and whether the result is passing.
    /// </summary>
    [Table(WithoutRowID: true)]
    public class PromotionBoardResult : EntityBase
    {
        /// <summary>
        /// Gets or Sets the <see cref="Soldier.Id"/> of the <see cref="Soldier"/> tied to this entity.
        /// </summary>
        [Column, PrimaryKey]
        public virtual int SoldierId { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="PromotionBoard.Id"/> that this entity references
        /// </summary>
        [Column, PrimaryKey, Index]
        public virtual int PromotionBoardId { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="PromotionBoard.Id"/> that this entity references
        /// </summary>
        [Column, PrimaryKey, Index]
        public virtual int IterationId { get; set; }

        /// <summary>
        /// Gets or sets the Score of the board
        /// </summary>
        [Column, Required]
        public virtual int Score { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="PromotionBoardResult"/>
        /// resulted in a passing score for the associated <see cref="Soldier"/>.
        /// </summary>
        [Column, Required, Default(false)]
        public virtual bool Passed { get; set; }
        
        #region Foreign Key Navigation Properties

        /// <summary>
        /// Gets or Sets the <see cref="Perscom.Database.Soldier"/> that 
        /// is tied to this <see cref="PromotionBoardResult"/>.
        /// </summary>
        [ForeignKey(nameof(SoldierId))]
        [References(nameof(Database.Soldier.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual Soldier Soldier { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="Perscom.Database.PromotionBoard"/> that 
        /// is tied to this <see cref="PromotionBoardResult"/>.
        /// </summary>
        [ForeignKey(nameof(PromotionBoardId))]
        [References(nameof(Database.PromotionBoard.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual PromotionBoard PromotionBoard { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="Perscom.Database.IterationDate"/> that 
        /// is tied to this <see cref="PromotionBoardResult"/>.
        /// </summary>
        [ForeignKey(nameof(IterationId))]
        [References(nameof(Database.IterationDate.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual IterationDate Iteration { get; set; }
        
        #endregion
    }
}
