using CrossLite;
using CrossLite.CodeFirst;
using System;

namespace Perscom.Database
{
    /// <summary>
    /// Represents the blueprint or template for creating unit entities in the application.
    /// Defines key attributes and relationships required for unit creation and management.
    /// </summary>
    [Table]
    public class UnitBlueprint : EntityBase, IEquatable<UnitBlueprint>
    {
        #region Columns

        /// <summary>
        /// The IsUnique Unit PoolSelection ID
        /// </summary>
        [Column, PrimaryKey]
        public virtual int Id { get; protected set; }
        
        /// <summary>
        /// Gets or Sets the <see cref="Faction.Id"/> this entity references
        /// </summary>
        [Column, Required]
        public virtual int FactionId { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="Echelon"/> object
        /// ID that this entity references
        /// </summary>
        [Column, Required]
        public virtual int EchelonId { get; set; }

        /// <summary>
        /// Gets or sets the string name of this <see cref="UnitBlueprint"/>
        /// </summary>
        [Column, Required]
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the name format string for the instanced <see cref="Unit"/>
        /// version of this <see cref="UnitBlueprint"/>
        /// </summary>
        /// <remarks>
        /// %n = The index number of this Unit Template of the same type (Ex: 1st, 2nd, 3rd)
        /// %c = The index Alpha code of this Unit Template of the same type (Ex: A, B, C)
        /// </remarks>
        [Column, Required, Default("")]
        public virtual string UnitNameFormat { get; set; }

        /// <summary>
        /// Gets or sets the short name format string for the instanced <see cref="Unit"/>
        /// version of this <see cref="UnitBlueprint"/>
        /// </summary>
        /// <remarks>
        /// %n = The index number of this Unit Template of the same type (Ex: 1st, 2nd, 3rd)
        /// %c = The index Alpha code of this Unit Template of the same type (Ex: A, B, C)
        /// </remarks>
        [Column, Required]
        public virtual string UnitCodeFormat { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Echelon"/> level in which we will find soldiers
        /// to fill this position
        /// </summary>
        [Column, Required]
        public virtual int PromotionPoolId { get; set; }

        #endregion

        #region Foreign Key Properties
        
        /// <summary>
        /// Gets or Sets the <see cref="Database.Faction"/> that 
        /// this unit type falls under.
        /// </summary>
        [ForeignKey(nameof(FactionId))]
        [References(nameof(Database.Faction.Id), 
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual Faction Faction { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="Database.Echelon"/> that 
        /// this unit type falls under.
        /// </summary>
        [ForeignKey(nameof(EchelonId))]
        [References(nameof(Database.Echelon.Id), 
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual Echelon Echelon { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="Database.Echelon"/> unit level that 
        /// this unit will pull soldiers from to fill <see cref="Position"/>s
        /// </summary>
        [ForeignKey(nameof(PromotionPoolId))]
        [References(nameof(Database.Echelon.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual Echelon PromotionEchelon { get; set; }

        #endregion

        #region Child Database Sets

        /// <summary>
        /// Gets a list of <see cref="PositionBlueprint"/> entities that reference this 
        /// <see cref="UnitBlueprint"/>
        /// </summary>
        public virtual EntitySet<PositionBlueprint> PositionBlueprints { get; set; }

        /// <summary>
        /// Gets a list of <see cref="UnitBlueprintAttachment"/> entities that reference this 
        /// <see cref="UnitBlueprint"/>
        /// </summary>
        [InverseForeignKey(nameof(UnitBlueprintAttachment.ParentId))]
        public virtual EntitySet<UnitBlueprintAttachment> SubUnitBlueprints { get; set; }
        
        /// <summary>
        /// Gets a list of <see cref="UnitBlueprintAttachment"/> entities that reference this 
        /// <see cref="UnitBlueprint"/>
        /// </summary>
        [InverseForeignKey(nameof(UnitBlueprintAttachment.ChildId))]
        public virtual EntitySet<UnitBlueprintAttachment> ParentUnitBlueprints { get; set; }

        /// <summary>
        /// Gets a list of <see cref="Unit"/> entities that reference this 
        /// <see cref="UnitBlueprint"/>
        /// </summary>
        public virtual EntitySet<Unit> Units { get; set; }

        #endregion

        public bool Equals(UnitBlueprint other)
        {
            if (other == null) return false;
            return (Id == other.Id);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as UnitBlueprint);
        }

        public override int GetHashCode() => Id;

        public override string ToString() => Name;
    }
}
