using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    [Table(WithoutRowID: true)]
    public class PositionBlueprintOccupation : EntityBase
    {
        #region Columns

        /// <summary>
        /// Gets or Sets the <see cref="PositionBlueprint.Id"/> that this entity references
        /// </summary>
        [Column, PrimaryKey]
        public virtual int PositionBlueprintId { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="Occupation.Id"/> that this entity references
        /// </summary>
        [Column, PrimaryKey]
        public virtual int OccupationId { get; set; }

        #endregion

        #region Foreign Keys

        /// <summary>
        /// Gets or Sets the <see cref="Database.PositionBlueprint"/> that 
        /// this entity references.
        /// </summary>
        [ForeignKey(nameof(PositionBlueprintId))]
        [References(nameof(Database.PositionBlueprint.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual PositionBlueprint Blueprint { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="Database.Occupation"/> that 
        /// this entity references.
        /// </summary>
        [ForeignKey(nameof(OccupationId))]
        [References(nameof(Database.Occupation.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual Occupation Occupation { get; set; }

        #endregion
    }
}