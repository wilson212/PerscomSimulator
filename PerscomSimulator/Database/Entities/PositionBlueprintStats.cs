using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    [Table]
    public class PositionBlueprintStats : AbstractPositionBlueprintStatistics
    {
        /// <summary>
        /// Gets or sets the <see cref="PositionBlueprint.Id"/>
        /// </summary>
        [Column, PrimaryKey]
        public virtual int BlueprintId { get; set; }

        #region Foreign Keys

        /// <summary>
        /// Gets or Sets the <see cref="Database.PositionBlueprint"/> that 
        /// this entity references.
        /// </summary>
        [ForeignKey(nameof(BlueprintId))]
        [References(nameof(Database.PositionBlueprint.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual PositionBlueprint Blueprint { get; set; }

        #endregion
    }
}
