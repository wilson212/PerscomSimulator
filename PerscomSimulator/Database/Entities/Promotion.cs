using System;
using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    /// <summary>
    /// Represents a relationship between a <see cref="Database.Soldier"/> and his past <see cref="Promotion"/>
    /// </summary>
    [Table]
    public class Promotion : IEquatable<Promotion>
    {
        #region Columns

        /// <summary>
        /// The Unique Promotion ID
        /// </summary>
        [Column, PrimaryKey]
        public int Id { get; protected set; }

        /// <summary>
        /// Gets or Sets the <see cref="Soldier"/> object
        /// ID that this entity references
        /// </summary>
        [Column, Required]
        public int SoldierId { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="Rank.Id"/> this <see cref="Soldier"/>
        /// was promoted from
        /// </summary>
        [Column, Required]
        public int FromRankId { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="Rank.Id"/> this <see cref="Soldier"/>
        /// was promoted to
        /// </summary>
        [Column, Required]
        public int ToRankId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IterationDate"/> of this promotion
        /// </summary>
        [Column, Required]
        public int IterationId { get; set; }

        /// <summary>
        /// Gets or sets the number of months Time in Grade this <see cref="Soldier"/>
        /// held the <see cref="FromRank"/>
        /// </summary>
        [Column, Required]
        public int PreviousTimeInGrade { get; set; }

        /// <summary>
        /// Gets or sets the number of months Time in Service this <see cref="Soldier"/>
        /// had when promoted to the <see cref="ToRank"/>
        /// </summary>
        [Column, Required]
        public int TimeInService { get; set; }

        #endregion

        #region Foreign Keys

        /// <summary>
        /// Gets the <see cref="Soldier"/> entity that this entity references.
        /// </summary>
        /// <remarks>Eager loaded because it should never be changed!</remarks>
        [InverseKey("Id")]
        [ForeignKey("SoldierId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        public virtual Soldier Soldier { get; private set; }

        /// <summary>
        /// Gets the <see cref="Rank"/> entity that this entity references.
        /// </summary>
        /// <remarks>Eager loaded because it should never be changed!</remarks>
        [InverseKey("Id")]
        [ForeignKey("FromRankId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        public virtual Rank FromRank { get; private set; }

        /// <summary>
        /// Gets the <see cref="Rank"/> entity that this entity references.
        /// </summary>
        /// <remarks>Eager loaded because it should never be changed!</remarks>
        [InverseKey("Id")]
        [ForeignKey("ToRankId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        public virtual Rank ToRank { get; private set; }

        /// <summary>
        /// Gets the <see cref="IterationDate"/> of when this promotion was recieved
        /// </summary>
        /// <remarks>Eager loaded because it should never be changed!</remarks>
        [InverseKey("Id")]
        [ForeignKey("IterationId",
            OnDelete = ReferentialIntegrity.Restrict,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        public virtual IterationDate Date { get; private set; }

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
