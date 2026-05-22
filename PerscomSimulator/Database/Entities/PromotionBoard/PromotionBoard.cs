using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    /// <summary>
    /// Represents a promotion board, which defines the criteria and configuration for evaluating candidates for
    /// promotion within a specific rank and occupation.
    /// </summary>
    /// <remarks>A promotion board is associated with a specific <see cref="Database.Rank"/> or <see cref="Database.RankClassification"/> 
    /// and optionally an <see cref="Database.Occupation"/>. It includes configuration for pass/fail evaluation, scoring thresholds, and other
    /// parameters used to determine promotability.</remarks>
    [Table]
    [CompositeUnique(nameof(RankId), nameof(RankClassificationId), nameof(OccupationId))]
    public class PromotionBoard : EntityBase
    {
        #region Columns

        /// <summary>
        /// The IsUnique PromotionBoard ID
        /// </summary>
        [Column, PrimaryKey]
        public virtual int Id { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="Database.Rank"/> ID that this entity references, if any.
        /// </summary>
        [Column, Default(null)]
        public virtual int? RankId { get; set; }
        
        /// <summary>
        /// Gets or Sets the <see cref="Database.RankClassification"/> ID that this entity references, if any.
        /// </summary>
        [Column, Default(null)]
        public virtual int? RankClassificationId { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="Database.Occupation"/> ID that this entity references, if any.
        /// </summary>
        [Column, Default(null), Index]
        public virtual int? OccupationId { get; set; } = null;

        /// <summary>
        /// The name of the promotion board
        /// </summary>
        [Column, Required]
        public virtual string Name { get; set; }

        /// <summary>
        /// The type of promotion board.
        /// </summary>
        [Column, Required, Default(0)]
        public virtual PromotionBoardType Type { get; set; } = PromotionBoardType.SequenceOrder;

        /// <summary>
        /// Indicates whether this promotion board uses pass/fail evaluation.
        /// </summary>
        [Column, Required, Default(1)]
        public virtual bool IsPassFail { get; set; }

        /// <summary>
        /// The minimum score required for a candidate to pass the promotion board.
        /// </summary>
        [Column, Required, Default(70)]
        public virtual int PassThreshold { get; set; }

        /// <summary>
        /// The length of the promotion status in months.
        /// </summary>
        [Column, Required, Default(12)]
        public virtual int PromotableLength { get; set; }

        /// <summary>
        /// The maximum score a candidate can achieve for Time in Grade.
        /// </summary>
        [Column, Required, Default(0)]
        public virtual int TimeInGradeMaxPoints { get; set; }

        /// <summary>
        /// A configurable integer value used to calculate the contribution of an entity's time in grade
        /// to their overall promotion eligibility score.
        /// </summary>
        [Column, Required, Default(0)]
        public virtual int TimeInGradeFactor { get; set; }
        
        /// <summary>
        /// The maximum score a candidate can achieve for Form Rating.
        /// </summary>
        [Column, Required, Default(0)]
        public virtual int FormRatingMaxPoints { get; set; }
        
        /// <summary>
        /// The maximum score a candidate can achieve for any promotion board metric.
        /// </summary>
        [Column, Required, Default(0)]
        public virtual int PointsCap { get; set; }

        #endregion

        #region Foreign Key Navigation Properties

        /// <summary>
        /// Gets the <see cref="Database.Rank"/> entity that this entity references.
        /// </summary>
        [ForeignKey(nameof(RankId))]
        [References(nameof(Database.Rank.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        protected virtual Rank Rank { get; set; }

        /// <summary>
        /// Gets the <see cref="Database.RankClassification"/> entity that this entity references.
        /// </summary>
        [ForeignKey(nameof(RankClassificationId))]
        [References(nameof(Database.RankClassification.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        protected virtual RankClassification RankClassification { get; set; }

        /// <summary>
        /// Gets the <see cref="Database.Occupation"/> entity that this entity references.
        /// </summary>
        [ForeignKey(nameof(OccupationId))]
        [References(nameof(Database.Occupation.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        protected virtual Occupation Occupation { get; set; }

        #endregion

        #region Child Navigation Properties

        /// <summary>
        /// Contains the list of candidates in the database
        /// </summary>
        public virtual EntitySet<PromotableCandidate> Candidates { get; set; }
        
        /// <summary>
        /// Contains the list of weights for this promotion board
        /// </summary>
        public virtual EntitySet<PromotionBoardWeight> Weights { get; set; }
        
        /// <summary>
        /// Contains the list of weights for this promotion board
        /// </summary>
        public virtual EntitySet<PromotionBoardAddScore> AdditionalScores { get; set; }

        #endregion
    }
}
