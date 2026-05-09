using System;
using System.Collections.Generic;
using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    /// <summary>
    /// Represents a position within a <see cref="Database.Unit"/> that a 
    /// <see cref="Database.Soldier"/> will occupy.
    /// </summary>
    [Table]
    public class Position : EntityBase, IEquatable<Position>
    {
        #region Columns

        /// <summary>
        /// The IsUnique Position ID
        /// </summary>
        [Column, PrimaryKey]
        public virtual int Id { get; protected set; }

        /// <summary>
        /// Gets or Sets the <see cref="Database.PositionBlueprint"/> object
        /// ID that this entity references
        /// </summary>
        [Column, Required]
        public virtual int BlueprintId { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="Database.Unit"/> this position
        /// is attached to
        /// </summary>
        [Column, Required]
        public virtual int UnitId { get; set; }

        /// <summary>
        /// Gets or Sets the string name of this Unit
        /// </summary>
        [Column, Required]
        public virtual string Name { get; set; }

        #endregion

        #region Foreign Keys

        /// <summary>
        /// Gets the <see cref="Database.PositionBlueprint"/> entity that this position references.
        /// </summary>
        [ForeignKey(nameof(BlueprintId))]
        [References(nameof(Database.PositionBlueprint.Id),
             OnDelete = ReferentialAction.Cascade,
             OnUpdate = ReferentialAction.Cascade
         )]
        public virtual PositionBlueprint Blueprint { get; set; }

        /// <summary>
        /// Gets the <see cref="Database.Unit"/> entity that this position is attached to.
        /// </summary>
        [ForeignKey(nameof(UnitId))]
        [References(nameof(Database.Unit.Id),
             OnDelete = ReferentialAction.Cascade,
             OnUpdate = ReferentialAction.Cascade
         )]
        public virtual Unit Unit { get; set; }

        #endregion

        #region Child Database Sets

        /// <summary>
        /// Gets a list of <see cref="Assignment"/> entities that reference this 
        /// <see cref="Position"/>
        /// </summary>
        public virtual EntitySet<Assignment> Assignments { get; set; }

        /// <summary>
        /// Gets a list of <see cref="PastAssignment"/> entities that reference this 
        /// <see cref="Position"/>
        /// </summary>
        public virtual EntitySet<PastAssignment> PastAssignments { get; set; }

        #endregion

        public bool Equals(Position other)
        {
            if (other == null) return false;
            return (Id == other.Id);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Position);
        }

        public override int GetHashCode() => Id;

        public override string ToString() => Name;
    }
}
