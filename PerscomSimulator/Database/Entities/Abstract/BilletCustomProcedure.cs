using System;
using CrossLite;
using CrossLite.CodeFirst;
using Perscom.Simulation;

namespace Perscom.Database
{
    public abstract class BilletCustomProcedure : IEquatable<BilletCustomProcedure>
    {
        /// <summary>
        /// The Unique Billet ID (Row ID)
        /// </summary>
        [Column, PrimaryKey]
        public int BilletId { get; protected set; }

        /// <summary>
        /// Indicates whether the <see cref="Database.Specialty"/> is to be
        /// changed for all soldiers, or just newly created ones
        /// </summary>
        [Column, Required, Default(0)]
        public bool ChangesSpecialtyForAll { get; set; }

        public abstract SelectionProcedure ProcedureType { get; }

        #region Virtual Foreign Keys

        /// <summary>
        /// Gets the <see cref="Database.Billet"/> entity that this entity references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("BilletId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<Billet> FK_Billet { get; set; }

        #endregion

        #region Foreign Key Properties

        /// <summary>
        /// Gets or Sets the <see cref="Perscom.Database.Billet"/>
        /// </summary>
        public Billet Billet
        {
            get
            {
                return FK_Billet?.Fetch();
            }
            set
            {
                BilletId = value.Id;
                FK_Billet?.Refresh();
            }
        }

        #endregion

        public abstract int GetProcedureId();

        public bool Equals(BilletCustomProcedure other)
        {
            if (other == null) return false;
            return (BilletId == other.BilletId);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as BilletCustomProcedure);
        }

        public override int GetHashCode() => BilletId;

    }
}
