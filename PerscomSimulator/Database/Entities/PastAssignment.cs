using System;
using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    /// <summary>
    /// Represents a relationship between a <see cref="Database.Soldier"/> and 
    /// <see cref="Database.Position"/>, where the <see cref="Database.Position"/> 
    /// was once held by the <see cref="Database.Soldier"/> during his career.
    /// </summary>
    [Table]
    public class PastAssignment
    {
        #region Column Properties

        /// <summary>
        /// The Unique Assingment History ID
        /// </summary>
        [Column, PrimaryKey]
        public int Id { get; protected set; }

        /// <summary>
        /// Gets or sets the <see cref="Soldier.Id"/>
        /// </summary>
        [Column, Required]
        public int SoldierId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Position.Id"/>
        /// </summary>
        [Column, Required]
        public int PositionId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DateTime"/> this position was assigned to the 
        /// <see cref="Soldier"/>
        /// </summary>
        [Column, Required]
        public DateTime AssignedOn { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DateTime"/> this position was removed from the 
        /// <see cref="Soldier"/>
        /// </summary>
        [Column, Required]
        public DateTime RemovedFrom { get; set; }

        #endregion

        #region Virtual Foreign Keys

        [InverseKey("Id")]
        [ForeignKey("SoldierId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<Soldier> FK_Soldier { get; set; }

        [InverseKey("Id")]
        [ForeignKey("PositionId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<Position> FK_Position { get; set; }

        #endregion

        #region Foreign Key Properties

        /// <summary>
        /// Gets or sets the <see cref="Perscom.Database.Soldier"/> that 
        /// this position held.
        /// </summary>
        public Soldier Soldier
        {
            get
            {
                return FK_Soldier?.Fetch();
            }
            set
            {
                SoldierId = value.Id;
                FK_Soldier?.Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Perscom.Database.Position"/> that 
        /// this soldier held.
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
