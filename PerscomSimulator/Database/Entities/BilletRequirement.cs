using System;
using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    /// <summary>
    /// Represents a <see cref="Billet"/> and <see cref="Specialty"/> constraint
    /// </summary>
    [Table]
    [CompositeUnique(nameof(BilletId), nameof(SpecialtyId))]
    public class BilletRequirement : IEquatable<BilletRequirement>
    {
        /// <summary>
        /// The Unique Requirement ID (Row ID)
        /// </summary>
        [Column, PrimaryKey]
        public int Id { get; protected set; }

        /// <summary>
        /// Gets or Sets the <see cref="Database.Billet"/> object
        /// ID that this entity references
        /// </summary>
        [Column, Required]
        public int BilletId { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="Database.Specialty"/> the <see cref="Soldier"/>
        /// holding this billet should be.
        /// </summary>
        [Column, Required]
        public int SpecialtyId { get; set; }

        #region Foreign Keys

        /// <summary>
        /// Gets the <see cref="Database.Billet"/> entity that this position references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("BilletId",
             OnDelete = ReferentialIntegrity.Cascade,
             OnUpdate = ReferentialIntegrity.Cascade
         )]
        protected virtual ForeignKey<Billet> FK_Billet { get; set; }

        /// <summary>
        /// Gets the <see cref="Database.Specialty"/> entity that this position references.
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
        /// Gets the <see cref="Database.Billet"/> entity that this requirement references.
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
        /// Gets or sets the <see cref="Database.Specialty"/> entity that this requirement references.
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
        /// Compares a <see cref="BilletRequirement"/> with this one, and returns whether
        /// or not the <see cref="BilletId"/> and <see cref="SpecialtyId"/> match.
        /// </summary>
        /// <remarks>Used in the <see cref="UnitTypeManagerForm"/></remarks>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsDuplicateOf(BilletRequirement other)
        {
            return (BilletId == other.BilletId && SpecialtyId == other.SpecialtyId);
        }

        public bool Equals(BilletRequirement other)
        {
            if (other == null) return false;
            return (Id == other.Id);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as BilletRequirement);
        }

        public override int GetHashCode() => Id;
    }
}
