using System;
using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    /// <summary>
    /// Represents a promotion event for a soldier within the system. This class
    /// contains details about the promotion, including the soldier involved,
    /// the ranks transitioned from and to, and related timing information.
    /// </summary>
    [Table]
    public class Promotion : EntityBase, IEquatable<Promotion>
    {
        #region Columns

        /// <summary>
        /// The IsUnique Promotion ID
        /// </summary>
        [Column, PrimaryKey]
        public virtual int Id { get; protected set; }

        /// <summary>
        /// Gets or Sets the <see cref="Soldier"/> object
        /// ID that this entity references
        /// </summary>
        [Column, Required]
        public virtual int SoldierId { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="Rank.Id"/> this <see cref="Soldier"/>
        /// was promoted from
        /// </summary>
        [Column, Required]
        public virtual int FromRankId { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="Rank.Id"/> this <see cref="Soldier"/>
        /// was promoted to
        /// </summary>
        [Column, Required]
        public virtual int ToRankId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IterationDate"/> of this promotion
        /// </summary>
        [Column, Required]
        public virtual int IterationId { get; set; }

        /// <summary>
        /// Gets or sets the number of months this <see cref="Soldier"/>
        /// held the <see cref="FromRank"/>
        /// </summary>
        [Column, Required]
        public virtual int PreviousTimeInRank { get; set; }

        /// <summary>
        /// Gets or sets the number of months Time in Service this <see cref="Soldier"/>
        /// had when promoted to the <see cref="ToRank"/>
        /// </summary>
        [Column, Required]
        public virtual int TimeInService { get; set; }

        /// <summary>
        /// Gets or sets the number of months Time in PayGrade for this <see cref="Soldier"/>
        /// </summary>
        [Column, Required]
        public virtual int TimeSinceLastGradeChange { get; set; }

        #endregion

        #region Foreign Keys

        /// <summary>
        /// Gets the <see cref="Soldier"/> entity that this entity references.
        /// </summary>
        [ForeignKey(nameof(SoldierId))]
        [References(nameof(Database.Soldier.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual Soldier Soldier { get; set; }

        /// <summary>
        /// Gets the <see cref="Rank"/> entity that this entity references.
        /// </summary>
        [ForeignKey(nameof(FromRankId))]
        [References(nameof(Database.Rank.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual Rank FromRank { get; set; }

        /// <summary>
        /// Gets the <see cref="Rank"/> entity that this entity references.
        /// </summary>
        [ForeignKey(nameof(ToRankId))]
        [References(nameof(Database.Rank.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual Rank ToRank { get; set; }

        /// <summary>
        /// Gets the <see cref="IterationDate"/> of when this promotion was recieved
        /// </summary>
        [ForeignKey(nameof(IterationId))]
        [References(nameof(Database.IterationDate.Id),
            OnDelete = ReferentialAction.Restrict,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual IterationDate Date { get; set; }

        #endregion

        public bool Equals(Promotion other)
        {
            if (other == null) return false;
            return (Id == other.Id);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Promotion);
        }

        public override int GetHashCode() => Id;

        public override string ToString() => Date?.Date.ToShortDateString() ?? IterationId.ToString();
    }
}
