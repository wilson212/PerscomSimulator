using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    [Table]
    public class PositionStatistics : AbstractBilletStatistics
    {
        /// <summary>
        /// 
        /// </summary>
        [Column, PrimaryKey]
        public int PositionId { get; set; }

        #region Virtual Foreign Keys

        /// <summary>
        /// Gets the <see cref="Database.Position"/> entity that this entity references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("PositionId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<Position> FK_Position { get; set; }

        #endregion

        #region Foreign Key Properties

        /// <summary>
        /// Gets or Sets the <see cref="Database.Position"/> that 
        /// this entity references.
        /// </summary>
        public Position Position
        {
            get
            {
                return FK_Position?.Fetch();
            }
            set
            {
                PositionId = value.Id;
                FK_Position?.Refresh();
            }
        }

        #endregion
    }
}
