using System;
using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    [Table]
    public class BilletSpawnSetting : IEquatable<BilletSpawnSetting>
    {
        /// <summary>
        /// The Unique Billet ID (Row ID)
        /// </summary>
        [Column, PrimaryKey]
        public int BilletId { get; protected set; }

        /// <summary>
        /// The Unique Soldier Generator ID
        /// </summary>
        [Column]
        public int GeneratorId { get; set; }

        /// <summary>
        /// The Unique Specialty ID
        /// </summary>
        [Column]
        public int SpecialtyId { get; set; }

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

        /// <summary>
        /// Gets the <see cref="Database.SoldierGenerator"/> entity that this entity references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("GeneratorId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<SoldierGenerator> FK_Generator { get; set; }

        /// <summary>
        /// Gets the <see cref="Database.Specialty"/> entity that this entity references.
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
        /// Gets or Sets the <see cref="Perscom.Database.SoldierGenerator"/> that 
        /// this Billit uses.
        /// </summary>
        public SoldierGenerator Generator
        {
            get
            {
                return FK_Generator?.Fetch();
            }
            set
            {
                GeneratorId = value.Id;
                FK_Generator?.Refresh();
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

        public bool Equals(BilletSpawnSetting other)
        {
            if (other == null) return false;
            return (BilletId == other.BilletId);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as BilletSpawnSetting);
        }

        public override int GetHashCode() => BilletId;

    }
}
