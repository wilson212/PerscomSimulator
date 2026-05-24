using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    [Table(WithoutRowID: true)]
    public class EvaluationBoardRank : EntityBase
    {
        #region Columns

        /// <summary>
        /// Gets or Sets the <see cref="EvaluationBoard.Id"/> that this entity references
        /// </summary>
        [Column, PrimaryKey]
        public virtual int EvaluationBoardId { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="Rank.Id"/> that this entity references
        /// </summary>
        [Column, PrimaryKey]
        public virtual int RankId { get; set; }

        #endregion

        #region Foreign Keys

        /// <summary>
        /// Gets or Sets the <see cref="Database.EvaluationBoard"/> that 
        /// this entity references.
        /// </summary>
        [ForeignKey(nameof(EvaluationBoardId))]
        [References(nameof(Database.EvaluationBoard.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual EvaluationBoard EvaluationBoard { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="Database.Rank"/> that 
        /// this entity references.
        /// </summary>
        [ForeignKey(nameof(RankId))]
        [References(nameof(Database.Rank.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual Rank Rank { get; set; }

        #endregion
    }
}