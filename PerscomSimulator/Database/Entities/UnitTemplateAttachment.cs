using System;
using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    /// <summary>
    /// Represents a relationship bewteen a number of <see cref="Database.UnitTemplate"/>'s
    /// </summary>
    [Table]
    [CompositeUnique(nameof(ParentId), nameof(ChildId))]
    public class UnitTemplateAttachment : IEquatable<UnitTemplateAttachment>
    {
        #region Column Properties

        /// <summary>
        /// The Unique UnitTemplateAttachment ID (Row ID)
        /// </summary>
        [Column, PrimaryKey]
        public int Id { get; protected set; }

        /// <summary>
        /// Gets or sets the parent <see cref="UnitTemplate.Id"/>
        /// </summary>
        [Column, Required]
        public int ParentId { get; set; }

        /// <summary>
        /// Gets or sets the child <see cref="UnitTemplate.Id"/>
        /// </summary>
        [Column, Required]
        public int ChildId { get; set; }

        /// <summary>
        /// Gets or sets the number of child <see cref="UnitTemplate"/> units attached
        /// to this parent <see cref="UnitTemplate"/>
        /// </summary>
        [Column, Required, Default(0)]
        public int Count { get; set; }

        #endregion

        #region Virtual Foreign Keys

        [InverseKey("Id")]
        [ForeignKey("ParentId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<UnitTemplate> FK_Parent { get; set; }

        [InverseKey("Id")]
        [ForeignKey("ChildId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<UnitTemplate> FK_Child { get; set; }

        #endregion

        #region Foreign Key Properties

        /// <summary>
        /// Gets or Sets the <see cref="Perscom.Database.Soldier"/> that 
        /// this position will hold.
        /// </summary>
        public UnitTemplate Parent
        {
            get
            {
                return FK_Parent?.Fetch();
            }
            set
            {
                ParentId = value.Id;
                FK_Parent?.Refresh();
            }
        }

        /// <summary>
        /// Gets or Sets the <see cref="Perscom.Database.Position"/> that 
        /// this soldier holds.
        /// </summary>
        public UnitTemplate Child
        {
            get
            {
                return FK_Child?.Fetch();
            }
            set
            {
                ChildId = value.Id;
                FK_Child?.Refresh();
            }
        }

        #endregion

        public bool Equals(UnitTemplateAttachment other)
        {
            if (other == null) return false;
            return (Id == other.Id);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as UnitTemplateAttachment);
        }

        public override int GetHashCode() => Id;
    }
}
