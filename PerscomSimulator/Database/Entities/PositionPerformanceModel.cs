using System;
using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    /// <summary>
    /// Represents a model for evaluating performance attributes associated
    /// with a specific position blueprint within the system. This class
    /// defines expected performance levels for various attributes tied to
    /// a position.
    /// </summary>
    /// <remarks>
    /// This entity is mapped to a database table and uses Code-First
    /// attributes to define its schema. The table is linked to
    /// <see cref="PositionBlueprint"/> and stores performance expectations
    /// for a position based on different attributes.
    /// </remarks>
    [Table(WithoutRowID = true)]
    public class PositionPerformanceModel : EntityBase
    {
        /// <summary>
        /// The IsUnique PositionBlueprint ID (Row ID)
        /// </summary>
        [Column, Required, PrimaryKey]
        public virtual int PositionBlueprintId { get; set; }

        /// <summary>
        /// Represents the specific attribute associated with a position's performance model.
        /// Defines the type of trait or characteristic to be evaluated, such as Leadership or Discipline.
        /// </summary>
        [Column, Required, PrimaryKey]
        public virtual AttributeType Attribute { get; set; }

        /// <summary>
        /// Represents the expected level of performance or proficiency required
        /// for a specific attribute in the associated position blueprint.
        /// </summary>
        [Column, Required]
        public virtual int ExpectedLevel
        {
            get => _expectedLevel; 
            set => _expectedLevel = Math.Clamp(value, 0, 20);
        }
        
        /// <summary>
        /// Represents the internally stored expected performance level value
        /// for a specific position attribute. This value is subject to validation
        /// and is clamped between 0 and 20 during assignment to ensure it stays
        /// within acceptable bounds.
        /// </summary>
        private int _expectedLevel = 0;
        
        #region Foreign Key Navigation Properties

        /// <summary>
        /// Gets or sets the <see cref="Database.PositionBlueprint"/> that 
        /// this performance model is associated with.
        /// </summary>
        [ForeignKey(nameof(PositionBlueprintId))]
        [References(nameof(Database.PositionBlueprint.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual PositionBlueprint PositionBlueprint { get; set; }

        #endregion
    }
}