using System;
using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    /// <summary>
    /// Represents a constraint that signifies that a <see cref="Database.PositionBlueprint"/> requires 
    /// a <see cref="Soldier"/> to have a specific <see cref="Database.Occupation"/> to enter it.
    /// </summary>
    [Table]
    public class PositionOccupationRequirement : EntityBase, IEquatable<PositionOccupationRequirement>
    {
        #region Column Properties
        
        /// <summary>
        /// Gets or Sets the <see cref="Database.PositionBlueprint"/> object
        /// ID that this entity references
        /// </summary>
        [Column, Required, PrimaryKey]
        public virtual int BlueprintId { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="Database.Occupation"/> the <see cref="Soldier"/>
        /// holding this billet should be.
        /// </summary>
        [Column, Required, PrimaryKey]
        public virtual int OccupationId { get; set; }

        #endregion

        #region Foreign Keys

        /// <summary>
        /// Gets or sets the <see cref="Database.PositionBlueprint"/> entity that this requirement references.
        /// </summary>
        [ForeignKey(nameof(BlueprintId))]
        [References(nameof(Database.PositionBlueprint.Id),
             OnDelete = ReferentialAction.Cascade,
             OnUpdate = ReferentialAction.Cascade
         )]
        public virtual PositionBlueprint Blueprint { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Database.Occupation"/> entity that this requirement references.
        /// </summary>
        [ForeignKey(nameof(OccupationId))]
        [References(nameof(Database.Occupation.Id),
             OnDelete = ReferentialAction.Cascade,
             OnUpdate = ReferentialAction.Cascade
         )]
        public virtual Occupation Occupation { get; set; }

        #endregion

        /// <summary>
        /// Compares a <see cref="PositionOccupationRequirement"/> with this one, and returns whether
        /// or not the <see cref="BlueprintId"/> and <see cref="OccupationId"/> match.
        /// </summary>
        /// <remarks>Used in the <see cref="UnitTypeManagerForm"/></remarks>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsDuplicateOf(PositionOccupationRequirement other)
        {
            if (other == null) return false;
            return (BlueprintId == other.BlueprintId && OccupationId == other.OccupationId);
        }

        public bool Equals(PositionOccupationRequirement other)
        {
            if (other == null) return false;
            return IsDuplicateOf(other);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as PositionOccupationRequirement);
        }

        public override int GetHashCode() => (BlueprintId, OccupationId).GetHashCode();
    }
}
