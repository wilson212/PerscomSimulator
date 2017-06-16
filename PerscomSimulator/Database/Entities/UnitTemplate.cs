using System;
using System.Collections.Generic;
using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    [Table]
    public class UnitTemplate : IEquatable<UnitTemplate>
    {
        #region Columns

        /// <summary>
        /// The Unique Unit Type ID
        /// </summary>
        [Column, PrimaryKey]
        public int Id { get; protected set; }

        /// <summary>
        /// Gets or Sets the <see cref="Echelon"/> object
        /// ID that this entity references
        /// </summary>
        [Column, Required]
        public int EchelonId { get; set; }

        /// <summary>
        /// Gets or sets the string name of this <see cref="UnitTemplate"/>
        /// </summary>
        [Column, Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name format string for the instanced <see cref="Unit"/>
        /// version of this <see cref="UnitTemplate"/>
        /// </summary>
        /// <remarks>
        /// %n = The index number of this Unit Template of the same type (Ex: 1st, 2nd, 3rd)
        /// %c = The index Alpha code of this Unit Template of the same type (Ex: A, B, C)
        /// </remarks>
        [Column, Required]
        public string UnitNameFormat { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Echelon"/> level in which we will find soldiers
        /// to fill this position
        /// </summary>
        [Column, Required]
        public int PromotionPoolId { get; set; }

        #endregion

        #region Virtual Foreign Keys

        [InverseKey("Id")]
        [ForeignKey("EchelonId",
            OnDelete = ReferentialIntegrity.Restrict,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<Echelon> FK_Echelon { get; set; }

        [InverseKey("Id")]
        [ForeignKey("PromotionPoolId",
            OnDelete = ReferentialIntegrity.Restrict,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<Echelon> FK_PromotionPool { get; set; }

        #endregion

        #region Foreign Key Properties

        /// <summary>
        /// Gets or Sets the <see cref="Database.Echelon"/> that 
        /// this unit type falls under.
        /// </summary>
        public Echelon Echelon
        {
            get
            {
                return FK_Echelon?.Fetch();
            }
            set
            {
                EchelonId = value.Id;
                FK_Echelon?.Refresh();
            }
        }

        /// <summary>
        /// Gets or Sets the <see cref="Database.Echelon"/> unit level that 
        /// this unit will pull soldiers from to fill <see cref="Position"/>s
        /// </summary>
        public Echelon PromotionPool
        {
            get
            {
                return FK_PromotionPool?.Fetch();
            }
            set
            {
                PromotionPoolId = value.Id;
                FK_PromotionPool?.Refresh();
            }
        }

        #endregion

        #region Child Database Sets

        /// <summary>
        /// Gets a list of <see cref="Billet"/> entities that reference this 
        /// <see cref="UnitTemplate"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration that fetches all Billet entities
        /// that are bound by the foreign key and this UnitTemplate.Id.
        /// </remarks>
        public virtual IEnumerable<Billet> Billets { get; set; }

        /// <summary>
        /// Gets a list of <see cref="UnitTemplateAttachment"/> entities that reference this 
        /// <see cref="UnitTemplate"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration that fetches all Unit template attachements
        /// that are bound by the foreign key and this UnitTemplate.Id.
        /// </remarks>
        public virtual IEnumerable<UnitTemplateAttachment> UnitTemplateAttachments { get; set; }

        /// <summary>
        /// Gets a list of <see cref="Unit"/> entities that reference this 
        /// <see cref="UnitTemplate"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration that fetches all Unit entities
        /// that are bound by the foreign key and this UnitTemplate.Id.
        /// </remarks>
        public virtual IEnumerable<Unit> Units { get; set; }

        #endregion

        public bool Equals(UnitTemplate other)
        {
            if (other == null) return false;
            return (Id == other.Id);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as UnitTemplate);
        }

        public override int GetHashCode() => Id;

        public override string ToString() => Name;
    }
}
