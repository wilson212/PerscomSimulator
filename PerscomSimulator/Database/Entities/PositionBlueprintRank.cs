using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    [Table(WithoutRowID: true)]
    public class PositionBlueprintRank : EntityBase
    {
        #region Columns

        /// <summary>
        /// Gets or Sets the <see cref="PositionBlueprint.Id"/> that this entity references
        /// </summary>
        [Column, PrimaryKey]
        public virtual int PositionBlueprintId { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="Rank.Id"/> that this entity references
        /// </summary>
        [Column, PrimaryKey]
        public virtual int RankId { get; set; }

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
        /// Gets or Sets the <see cref="Database.Rank"/> that 
        /// this entity references.
        /// </summary>
        [ForeignKey(nameof(RankId))]
        [References(nameof(Database.Rank.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual Rank Rank { get; set; }

        #endregion
    }
}