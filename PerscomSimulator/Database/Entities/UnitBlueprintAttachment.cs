using System;
using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    /// <summary>
    /// Represents a relationship between a number of <see cref="Database.UnitBlueprint"/>'s
    /// </summary>
    [Table(WithoutRowID = true)]
    [CompositeUnique(nameof(ParentId), nameof(ChildId))]
    public class UnitBlueprintAttachment : EntityBase, IEquatable<UnitBlueprintAttachment>
    {
        #region Column Properties
        
        /// <summary>
        /// Gets or sets the parent <see cref="UnitBlueprint.Id"/>
        /// </summary>
        [Column, Required, PrimaryKey]
        public virtual int ParentId { get; set; }

        /// <summary>
        /// Gets or sets the child <see cref="UnitBlueprint.Id"/>
        /// </summary>
        [Column, Required, PrimaryKey]
        public virtual int ChildId { get; set; }

        /// <summary>
        /// Gets or sets the number of child <see cref="UnitBlueprint"/> units attached
        /// to this parent <see cref="UnitBlueprint"/>
        /// </summary>
        [Column, Required, Default(1)]
        public virtual int Count { get; set; }

        #endregion

        #region Foreign Keys

        /// <summary>
        /// Gets or Sets the parent <see cref="UnitBlueprint"/> entity.
        /// </summary>
        [ForeignKey(nameof(ParentId))]
        [References(nameof(Database.UnitBlueprint.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual UnitBlueprint Parent { get; set; }

        /// <summary>
        /// Gets or Sets the child <see cref="UnitBlueprint"/> entity.
        /// </summary>
        [ForeignKey(nameof(ChildId))]
        [References(nameof(Database.UnitBlueprint.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual UnitBlueprint Child { get; set; }

        #endregion

        public bool Equals(UnitBlueprintAttachment other)
        {
            if (other == null) return false;
            return (ParentId == other.ParentId && ChildId == other.ChildId);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as UnitBlueprintAttachment);
        }

        public override int GetHashCode() => HashCode.Combine(ParentId, ChildId);
    }
}
