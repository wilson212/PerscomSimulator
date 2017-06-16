﻿using System;
using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    [Table]
    public class Assignment : IEquatable<Assignment>
    {
        #region Column Properties

        /// <summary>
        /// Gets or sets the parent <see cref="Soldier.Id"/>
        /// </summary>
        [Column, PrimaryKey]
        public int SoldierId { get; set; }

        /// <summary>
        /// Gets or sets the parent <see cref="Position.Id"/>
        /// </summary>
        [Column, PrimaryKey, Unique]
        public int PositionId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IterationDate.Id"/> this position was assigned to the 
        /// <see cref="Soldier"/>
        /// </summary>
        [Column, Required]
        public int AssignedIteration { get; set; }

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

        [InverseKey("Id")]
        [ForeignKey("AssignedIteration",
            OnDelete = ReferentialIntegrity.Restrict,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<IterationDate> FK_Iteration { get; set; }

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

        /// <summary>
        /// Gets or Sets the <see cref="Perscom.Database.IterationDate"/> that 
        /// this soldier was assigned this <see cref="Assignment"/>.
        /// </summary>
        public IterationDate AssignedOn
        {
            get
            {
                return FK_Iteration?.Fetch();
            }
            set
            {
                AssignedIteration = value.Id;
                FK_Iteration?.Refresh();
            }
        }

        #endregion

        public bool Equals(Assignment other)
        {
            if (other == null) return false;
            return (SoldierId == other.SoldierId);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Assignment);
        }

        public override int GetHashCode() => SoldierId;
    }
}
