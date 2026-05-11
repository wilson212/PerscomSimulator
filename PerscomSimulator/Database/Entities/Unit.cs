using System;
using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    /// <summary>
    /// Represents a structure that contains child <see cref="Unit"/>s and <see cref="PositionBlueprint"/>s.
    /// </summary>
    [Table]
    public class Unit : EntityBase, IEquatable<Unit>
    {
        #region Columns

        /// <summary>
        /// The IsUnique Unit ID
        /// </summary>
        [Column, PrimaryKey]
        public virtual int Id { get; protected set; }

        /// <summary>
        /// Gets or Sets the <see cref="UnitBlueprint"/> object
        /// ID that this entity references
        /// </summary>
        [Column, Required]
        public virtual int UnitBlueprintId { get; set; }

        /// <summary>
        /// Gets or Sets the string name of this Unit
        /// </summary>
        [Column, Required]
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or Sets the string name of this Unit
        /// </summary>
        [Column, Required, Default("")]
        public virtual string UnitCode { get; set; }

        /// <summary>
        /// Gets or Sets the Parent Unit Id
        /// </summary>
        [Column, Default(null)]
        public virtual int? ParentUnitId { get; set; } = null;

        #endregion

        #region Foreign Keys

        /// <summary>
        /// Gets the <see cref="UnitBlueprint"/> entity that this entity references.
        /// </summary>
        [ForeignKey(nameof(UnitBlueprintId))]
        [References(nameof(Database.UnitBlueprint.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual UnitBlueprint Blueprint { get; set; }

        /// <summary>
        /// Gets the parent <see cref="Unit"/> entity that this entity references.
        /// </summary>
        [ForeignKey(nameof(ParentUnitId))]
        [References(nameof(Id),
            OnDelete = ReferentialAction.SetNull,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual Unit Parent { get; set; }

        #endregion

        #region Child Database Sets

        /// <summary>
        /// Gets a list of child <see cref="Unit"/> entities that reference this 
        /// <see cref="Unit"/>
        /// </summary>
        public virtual EntitySet<Unit> Children { get; set; }

        #endregion

        public bool Equals(Unit other)
        {
            if (other == null) return false;
            return (Id == other.Id);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Unit);
        }

        public override int GetHashCode() => Id;

        public override string ToString() => Name;
    }
}
