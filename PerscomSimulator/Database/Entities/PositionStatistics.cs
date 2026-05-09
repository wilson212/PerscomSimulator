using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    [Table]
    public class PositionStatistics : AbstractPositionBlueprintStatistics
    {
        /// <summary>
        /// Gets or sets the <see cref="Position.Id"/>
        /// </summary>
        [Column, PrimaryKey]
        public virtual int PositionId { get; set; }

        #region Foreign Keys

        /// <summary>
        /// Gets or Sets the <see cref="Database.Position"/> that 
        /// this entity references.
        /// </summary>
        [ForeignKey(nameof(PositionId))]
        [References(nameof(Database.Position.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual Position Position { get; set; }

        #endregion
    }
}
