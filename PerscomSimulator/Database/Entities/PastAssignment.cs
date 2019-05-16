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
    public class PastAssignment : IEquatable<PastAssignment>
    {
        #region Column Properties

        /// <summary>
        /// The Unique Assingment History ID
        /// </summary>
        [Column, PrimaryKey]
        public int Id { get; protected set; }

        /// <summary>
        /// Gets or sets the <see cref="Soldier.Id"/>
        /// </summary>
        [Column, Required]
        public int SoldierId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Position.Id"/>
        /// </summary>
        [Column, Required]
        public int PositionId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DateTime"/> this position was assigned to the 
        /// <see cref="Soldier"/>
        /// </summary>
        [Column, Required]
        public int EntryIterationId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DateTime"/> this position was removed from the 
        /// <see cref="Soldier"/>
        /// </summary>
        [Column, Required]
        public int ExitIterationId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Rank.Id"/> this <see cref="Soldier"/>
        /// was when moving into this position
        /// </summary>
        [Column, Required]
        public int EntryRankId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Rank.Id"/> this <see cref="Soldier"/>
        /// was promoted from
        /// </summary>
        [Column, Required]
        public int ExitRankId { get; set; }

        /// <summary>
        /// Gets or sets the last Rank Gade change date for this soldier
        /// </summary>
        [Column, Required]
        public int LastGradeChangeIterationId { get; set; }

        #endregion

        #region Virtual Foreign Keys

        [InverseKey("Id")]
        [ForeignKey("SoldierId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<Soldier> FK_Soldier { get; set; }

        [InverseKey("Id")]
        [ForeignKey("PositionId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<Position> FK_Position { get; set; }

        [InverseKey("Id")]
        [ForeignKey("EntryIterationId",
            OnDelete = ReferentialIntegrity.Restrict,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<IterationDate> FK_Start { get; set; }

        [InverseKey("Id")]
        [ForeignKey("ExitIterationId",
            OnDelete = ReferentialIntegrity.Restrict,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<IterationDate> FK_End { get; set; }

        /// <summary>
        /// Gets the <see cref="Database.Rank"/> entity that this entity references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("EntryRankId",
            OnDelete = ReferentialIntegrity.Restrict,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<Rank> FK_RankEntry { get; set; }

        /// <summary>
        /// Gets the <see cref="Database.Rank"/> entity that this entity references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("ExitRankId",
            OnDelete = ReferentialIntegrity.Restrict,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<Rank> FK_RankExit { get; set; }

        /// <summary>
        /// Gets the <see cref=IterationDate"/> entity that this entity references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("LastGradeChangeIterationId",
            OnDelete = ReferentialIntegrity.Restrict,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<IterationDate> FK_Grade { get; set; }

        #endregion

        #region Foreign Key Properties

        /// <summary>
        /// Gets or sets the <see cref="Perscom.Database.Soldier"/> that 
        /// this position held.
        /// </summary>
        public Soldier Soldier
        {
            get
            {
                return FK_Soldier?.Fetch();
            }
            set
            {
                SoldierId = value.Id;
                FK_Soldier?.Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Perscom.Database.Position"/> that 
        /// this soldier held.
        /// </summary>
        public Position Position
        {
            get
            {
                return FK_Position?.Fetch();
            }
            set
            {
                PositionId = value.Id;
                FK_Position?.Refresh();
            }
        }

        public IterationDate EntryDate
        {
            get
            {
                return FK_Start?.Fetch();
            }
            set
            {
                EntryIterationId = value.Id;
                FK_Start?.Refresh();
            }
        }

        public IterationDate ExitDate
        {
            get
            {
                return FK_End?.Fetch();
            }
            set
            {
                ExitIterationId = value.Id;
                FK_End?.Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Perscom.Database.Rank"/> that 
        /// the soldier was when entering this position.
        /// </summary>
        public Rank EntryRank
        {
            get
            {
                return FK_RankEntry?.Fetch();
            }
            set
            {
                EntryRankId = value.Id;
                FK_RankEntry?.Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Perscom.Database.Rank"/> that 
        /// the soldier had when he left this assignment (before promotion).
        /// </summary>
        public Rank ExitRank
        {
            get
            {
                return FK_RankExit?.Fetch();
            }
            set
            {
                ExitRankId = value.Id;
                FK_RankExit?.Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="IterationDate"/> that this <see cref="Soldier"/> 
        /// earned his last <see cref="Promotion"/> that was a Grade change
        /// </summary>
        public IterationDate LastGradeChangeDate
        {
            get
            {
                return FK_Grade?.Fetch();
            }
            set
            {
                LastGradeChangeIterationId = value.Id;
                FK_Grade?.Refresh();
            }
        }

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
