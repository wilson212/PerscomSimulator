using System;
using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    [Table]
    [CompositeUnique(nameof(SoldierId), nameof(PositionId))]
    public class Assignment : IEquatable<Assignment>
    {
        #region Column Properties

        /// <summary>
        /// The Unique Assignment ID
        /// </summary>
        [Column, PrimaryKey]
        public int Id { get; protected set; }

        /// <summary>
        /// Gets or sets the parent <see cref="Soldier.Id"/>
        /// </summary>
        [Column, Required]
        public int SoldierId { get; set; }

        /// <summary>
        /// Gets or sets the parent <see cref="Position.Id"/>
        /// </summary>
        [Column, Required]
        public int PositionId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DateTime"/> this position was assigned to the 
        /// <see cref="Soldier"/>
        /// </summary>
        [Column, Required]
        public DateTime AssignedOn { get; set; }

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
        /// Gets or Sets the <see cref="Perscom.Database.Soldier"/> that 
        /// this position will hold.
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
        /// Gets or Sets the <see cref="Perscom.Database.Position"/> that 
        /// this soldier holds.
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

        public bool Equals(Assignment other)
        {
            if (other == null) return false;
            return (Id == other.Id);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Assignment);
        }

        public override int GetHashCode() => Id;
    }
}
