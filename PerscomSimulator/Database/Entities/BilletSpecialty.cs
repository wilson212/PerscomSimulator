using System;
using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    /// <summary>
    /// Represents a [1:{0,1}] relationship bewteen a <see cref="Database.Billet"/> 
    /// and a <see cref="Database.Specialty"/>.
    /// </summary>
    /// <remarks>
    /// If this entity exists, that means a Billet will set the Specialty of
    /// the Soldier who enters the Billet.
    /// </remarks>
    [Table]
    public class BilletSpecialty : IEquatable<BilletSpecialty>
    {
        /// <summary>
        /// The Unique Billet ID (Row ID)
        /// </summary>
        [Column, PrimaryKey]
        public int BilletId { get; protected set; }

        /// <summary>
        /// The Unique Specialty ID
        /// </summary>
        [Column, Required]
        public int SpecialtyId { get; set; }

        #region Virtual Foreign Keys

        /// <summary>
        /// Gets the <see cref="Billet"/> entity that this entity references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("BilletId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<Billet> FK_Billet { get; set; }

        /// <summary>
        /// Gets the <see cref="Specialty"/> entity that this entity references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("SpecialtyId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<Specialty> FK_Specialty { get; set; }

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

        /// <summary>
        /// Gets or Sets the <see cref="Perscom.Database.Specialty"/> that 
        /// this Billit falls under.
        /// </summary>
        public Specialty Specialty
        {
            get
            {
                return FK_Specialty?.Fetch();
            }
            set
            {
                SpecialtyId = value.Id;
                FK_Specialty?.Refresh();
            }
        }

        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="BilletSpecialty"/>
        /// </summary>
        public BilletSpecialty()
        {

        }

        /// <summary>
        /// Creates a new instance of <see cref="BilletSpecialty"/> using the specified
        /// <see cref="Database.Billet"/> and <see cref="Database.Specialty"/>
        /// </summary>
        /// <param name="billet"></param>
        /// <param name="spec"></param>
        public BilletSpecialty(Billet billet, Specialty spec)
        {
            this.BilletId = billet.Id;
            this.SpecialtyId = spec.Id;
        }

        public bool Equals(BilletSpecialty other)
        {
            if (other == null) return false;
            return (BilletId == other.BilletId);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as BilletSpecialty);
        }

        public override int GetHashCode() => BilletId;
    }
}
