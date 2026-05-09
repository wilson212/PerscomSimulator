using System;
using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    /// <summary>
    /// Represents a relationship between a <see cref="Database.Soldier"/> and 
    /// <see cref="Database.Position"/>, where the <see cref="Database.Position"/> 
    /// was once held by the <see cref="Database.Soldier"/> during his career.
    /// </summary>
    [Table]
    public class PastAssignment : EntityBase, IEquatable<PastAssignment>
    {
        #region Column Properties

        /// <summary>
        /// The IsUnique Assingment History ID
        /// </summary>
        [Column, PrimaryKey]
        public virtual int Id { get; protected set; }

        /// <summary>
        /// Gets or sets the <see cref="Soldier.Id"/>
        /// </summary>
        [Column, Required]
        public virtual int SoldierId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Position.Id"/>
        /// </summary>
        [Column, Required]
        public virtual int PositionId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IterationDate.Id"/> this position was assigned to the 
        /// <see cref="Soldier"/>
        /// </summary>
        [Column, Required]
        public virtual int EntryIterationId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IterationDate.Id"/> this position was removed from the 
        /// <see cref="Soldier"/>
        /// </summary>
        [Column, Required]
        public virtual int ExitIterationId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Rank.Id"/> this <see cref="Soldier"/>
        /// was when moving into this position
        /// </summary>
        [Column, Required]
        public virtual int EntryRankId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Rank.Id"/> this <see cref="Soldier"/>
        /// was promoted from
        /// </summary>
        [Column, Required]
        public virtual int ExitRankId { get; set; }

        /// <summary>
        /// Gets or sets the last Rank Gade change date for this soldier
        /// </summary>
        [Column, Required]
        public virtual int LastGradeChangeIterationId { get; set; }

        #endregion

        #region Foreign Keys

        /// <summary>
        /// Gets or sets the <see cref="Perscom.Database.Soldier"/> that 
        /// this position held.
        /// </summary>
        [ForeignKey(nameof(SoldierId))]
        [References(nameof(Database.Soldier.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual Soldier Soldier { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Perscom.Database.Position"/> that 
        /// this soldier held.
        /// </summary>
        [ForeignKey(nameof(PositionId))]
        [References(nameof(Database.Position.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual Position Position { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IterationDate"/> this position was assigned to the soldier.
        /// </summary>
        [ForeignKey(nameof(EntryIterationId))]
        [References(nameof(Database.IterationDate.Id),
            OnDelete = ReferentialAction.Restrict,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual IterationDate EntryDate { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IterationDate"/> this position was removed from the soldier.
        /// </summary>
        [ForeignKey(nameof(ExitIterationId))]
        [References(nameof(Database.IterationDate.Id),
            OnDelete = ReferentialAction.Restrict,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual IterationDate ExitDate { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Perscom.Database.Rank"/> that 
        /// the soldier was when entering this position.
        /// </summary>
        [ForeignKey(nameof(EntryRankId))]
        [References(nameof(Database.Rank.Id),
            OnDelete = ReferentialAction.Restrict,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual Rank EntryRank { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Perscom.Database.Rank"/> that 
        /// the soldier had when he left this assignment.
        /// </summary>
        [ForeignKey(nameof(ExitRankId))]
        [References(nameof(Database.Rank.Id),
            OnDelete = ReferentialAction.Restrict,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual Rank ExitRank { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IterationDate"/> that this <see cref="Soldier"/> 
        /// earned his last PayGrade change.
        /// </summary>
        [ForeignKey(nameof(LastGradeChangeIterationId))]
        [References(nameof(Database.IterationDate.Id),
            OnDelete = ReferentialAction.Restrict,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual IterationDate LastGradeChangeDate { get; set; }

        #endregion

        public bool Equals(PastAssignment other)
        {
            if (other == null) return false;
            return (Id == other.Id);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as PastAssignment);
        }

        public override int GetHashCode() => Id;
    }
}
