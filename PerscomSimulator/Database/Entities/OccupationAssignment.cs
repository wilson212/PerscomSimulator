using System;
using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    [Table]
    public class OccupationAssignment : EntityBase, IEquatable<OccupationAssignment>
    {
        #region Column Properties

        /// <summary>
        /// The IsUnique Assignment ID
        /// </summary>
        [Column, PrimaryKey]
        public virtual int Id { get; protected set; }

        /// <summary>
        /// Gets or sets the parent <see cref="Soldier.Id"/>
        /// </summary>
        [Column, Required]
        public virtual int SoldierId { get; set; }

        /// <summary>
        /// Gets or sets the parent <see cref="Occupation.Id"/>
        /// </summary>
        [Column, Required]
        public virtual int OccupationId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IterationDate.Id"/> this position was assigned to the 
        /// <see cref="Soldier"/>
        /// </summary>
        [Column, Required]
        public virtual int AssignedIteration { get; set; }

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
        /// Gets or Sets the <see cref="Perscom.Database.Occupation"/> that 
        /// this soldier holds.
        /// </summary>
        [ForeignKey(nameof(OccupationId))]
        [References(nameof(Database.Occupation.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual Occupation Occupation { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="Perscom.Database.IterationDate"/> that 
        /// this soldier was assigned this <see cref="OccupationAssignment"/>.
        /// </summary>
        [ForeignKey(nameof(AssignedIteration))]
        [References(nameof(Database.IterationDate.Id),
            OnDelete = ReferentialAction.Restrict,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual IterationDate AssignedOn { get; set; }

        #endregion

        public bool Equals(OccupationAssignment other)
        {
            if (other == null) return false;
            return (Id == other.Id);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as OccupationAssignment);
        }

        public override int GetHashCode() => Id;
    }
}
