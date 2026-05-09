using CrossLite;
using CrossLite.CodeFirst;
using Perscom.Simulation;
using System;
using System.Collections.Generic;

namespace Perscom.Database
{
    /// <summary>
    /// Classifies a rank by its PayGrade and Blueprint.
    /// </summary>
    [Table]
    [CompositeUnique(nameof(FactionId), nameof(Type), nameof(PayGrade))]
    public class RankClassification : EntityBase, IEquatable<RankClassification>
    {
        #region Columns

        /// <summary>
        /// The IsUnique Database Id for this RankClassification
        /// </summary>
        [Column, PrimaryKey]
        public virtual int Id { get; protected set; }
        
        /// <summary>
        /// Gets or sets the <see cref="Faction.Id"/> this entity references
        /// </summary>
        [Column, Required]
        public virtual int FactionId { get; set; }

        /// <summary>
        /// Gets or sets the type of this rank
        /// </summary>
        [Column, Required]
        public virtual RankType Type { get; set; }

        /// <summary>
        /// Gets or sets the paygrade of this rank
        /// </summary>
        [Column, Required]
        public virtual int PayGrade { get; set; }

        /// <summary>
        /// Indicates whether promotion to this rank grade is automatic 
        /// from the previous rank grade
        /// </summary>
        [Column, Required, Default(0)]
        public virtual PayGradeSelection Selection { get; set; } = PayGradeSelection.Automatic;

        /// <summary>
        /// Gets or sets the minimum time (months) a soldier must hold this grade before 
        /// being allowed to retire. If the amount is less than the remaining time to live for
        /// the soldier, their retirement date will be adjusted accordingly.
        /// </summary>
        [Column, Required, Default(0)]
        public virtual int LockInTime { get; set; } = 0;

        /// <summary>
        /// Gets or sets the minimum time (months) a soldier must hold this grade before 
        /// being allowed to retire in this grade. If the minimum amount is less than the 
        /// remaining time to live for the soldier, then their retirement grade will be
        /// that of the previous grade.
        /// </summary>
        /// <example>
        /// If the soldier retires as an E7, but thier TimeInGrade was less than the 
        /// E7 MinTimeInGrade, then the soldier will retire as an E6 instead.
        /// </example>
        [Column, Required, Default(0)]
        public virtual int MinTimeInGrade { get; set; } = 0;

        /// <summary>
        /// Gets or sets the maximum time (months) a soldier can hold this grade before being
        /// forcefully retired. Retirement date is otherwise un-modified.
        /// </summary>
        [Column, Required, Default(0)]
        public virtual int MaxTimeInGrade { get; set; } = 0;

        /// <summary>
        /// Gets or sets the required time in grade (months) to be promotable to this rank grade
        /// from the previous rank grade
        /// </summary>
        [Column, Required, Default(12)]
        public virtual int PreviousTimeInGradeRequirement { get; set; } = 12;

        /// <summary>
        /// Gets or sets the length of time a soldier will be considered "Promotable" after passing
        /// the promotion board. After this length of time has passed, they will need to be
        /// re-evaluated again and will lose their promotable status.
        /// </summary>
        [Column, Required, Default(12)]
        public virtual int PromotableLength { get; set; } = 12;

        /// <summary>
        /// 
        /// </summary>
        [Column, Required, Default(0)]
        public virtual double Stipend { get; set; }

        /// <summary>
        /// Indicates whether this rank classification has split rank progression pathways. If true, ranks within this
        /// classification have multiple progression tracks, must define their own <see cref="PromotionBoard"/>
        /// and <see cref="Rank.NextRankId"/>
        /// </summary>
        /// <remarks>
        /// <para>
        ///     For example, using real USA military rankings, For an E-8 board, this would be:
        ///     <list type="bullet">
        ///         <item><b>False:</b> Army System (Generic E-8 Board). </item>
        ///         <item><b>True:</b> USMC System (Apply directly to 1stSgt rank or MSgt rank).</item>
        ///     </list>
        /// </para>
        /// </remarks>
        [Column, Required, Default(false)]
        public virtual bool HasSplitRankLanes { get; set; }

        #endregion

        #region Foreign Key Navigation Properties

        /// <summary>
        /// Represents the faction associated with the rank classification.
        /// This property establishes a foreign key relationship to the <see cref="Faction"/> table
        /// to ensure referential integrity within the database.
        /// </summary>
        [ForeignKey(nameof(FactionId))]
        [References(nameof(Database.Faction.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual Faction Faction { get; set; }
        
        #endregion

        #region Child Database Sets

        /// <summary>
        /// Gets a list of <see cref="Rank"/> entities that reference this 
        /// <see cref="RankClassification"/>.
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration that fetches all Ranks that match this classification.
        /// </remarks>
        public virtual EntitySet<Rank> Ranks { get; set; }

        #endregion

        public bool Equals(RankClassification other)
        {
            if (other == null) return false;
            return (FactionId == other.FactionId && PayGrade == other.PayGrade && Type == other.Type);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as RankClassification);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FactionId, PayGrade, Type);
        }

        public override string ToString()
        {
            switch (Type)
            {
                case RankType.Officer: return "O" + PayGrade.ToString();
                case RankType.Warrant: 
                    if (PayGrade == 1) 
                        return "WO" + PayGrade.ToString();
                    else 
                        return "CW" + PayGrade.ToString();
                default: return "E" + PayGrade.ToString();
            }
        }
    }
}
