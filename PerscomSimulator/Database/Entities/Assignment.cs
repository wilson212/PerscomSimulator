using System;
using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    /// <summary>
    /// This Entity represents a (1:1 or 1:0) relationship between a 
    /// <see cref="Perscom.Database.Soldier"/> and his current Assignment
    /// to a <see cref="Database.Position"/>
    /// </summary>
    /// <remarks>
    /// Soldiers that are considered Retired will not have an Assignment,
    /// hence being a 1:1 or 1:0 relationship!
    /// </remarks>
    [Table]
    public class Assignment : EntityBase, IEquatable<Assignment>
    {
        #region Column Properties

        /// <summary>
        /// Gets or sets the parent <see cref="Soldier.Id"/>
        /// </summary>
        [Column, PrimaryKey]
        public virtual int SoldierId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Position.Id"/> this
        /// entity holds
        /// </summary>
        [Column, Unique]
        public virtual int PositionId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IterationDate.Id"/> this position was assigned to the 
        /// <see cref="Soldier"/>
        /// </summary>
        [Column, Required]
        public virtual int EntryIterationId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Rank.Id"/> this <see cref="Database.Soldier"/>
        /// was when moving into this position
        /// </summary>
        [Column, Required]
        public virtual int EntryRankId { get; set; }

        #endregion

        #region Foreign Keys

        /// <summary>
        /// Gets or Sets the <see cref="Perscom.Database.Soldier"/> that 
        /// this position will hold.
        /// </summary>
        [ForeignKey(nameof(SoldierId))]
        [References(nameof(Database.Soldier.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual Soldier Soldier { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="Perscom.Database.Position"/> that 
        /// this soldier holds.
        /// </summary>
        [ForeignKey(nameof(PositionId))]
        [References(nameof(Database.Position.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual Position Position { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="Perscom.Database.IterationDate"/> that 
        /// this soldier was assigned this <see cref="Assignment"/>.
        /// </summary>
        [ForeignKey(nameof(EntryIterationId))]
        [References(nameof(Database.IterationDate.Id),
            OnDelete = ReferentialAction.Restrict,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual IterationDate AssignedOn { get; set; }

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

        #endregion

        public bool Equals(Assignment other)
        {
            if (other == null) return false;
            return (SoldierId == other.SoldierId);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Assignment);
        }

        public override int GetHashCode() => SoldierId;
    }
}
