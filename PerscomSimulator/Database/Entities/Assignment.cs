using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    [Table]
    public class Assignment
    {
        #region Column Properties

        /// <summary>
        /// Gets or sets the parent <see cref="Soldier.Id"/>
        /// </summary>
        [Column, Required, PrimaryKey]
        public int SoldierId { get; set; }

        /// <summary>
        /// Gets or sets the parent <see cref="Position.Id"/>
        /// </summary>
        [Column, Required, PrimaryKey]
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
    }
}
