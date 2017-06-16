using System;
using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    /// <summary>
    /// Represents a relationship between <see cref="Unit"/>'s
    /// </summary>
    [Table]
    public class UnitAttachment : IEquatable<UnitAttachment>
    {
        #region Column Properties

        /// <summary>
        /// The Unique UnitAttachment ID (Row ID)
        /// </summary>
        [Column, PrimaryKey]
        public int Id { get; protected set; }

        /// <summary>
        /// Gets or sets the parent <see cref="Unit.Id"/>
        /// </summary>
        [Column, Required]
        public int ParentId { get; set; }

        /// <summary>
        /// Gets or sets the child <see cref="Unit.Id"/>
        /// </summary>
        [Column, Required, Unique]
        public int ChildId { get; set; }

        #endregion

        #region Virtual Foreign Keys

        [InverseKey("Id")]
        [ForeignKey("ParentId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<Unit> FK_Parent { get; set; }

        [InverseKey("Id")]
        [ForeignKey("ChildId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<Unit> FK_Child { get; set; }

        #endregion

        #region Foreign Key Properties

        /// <summary>
        /// Gets or Sets the <see cref="Perscom.Database.Soldier"/> that 
        /// this position will hold.
        /// </summary>
        public Unit ParentUnit
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
        public Unit ChildUnit
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

        public bool Equals(UnitAttachment other)
        {
            if (other == null) return false;
            return (Id == other.Id);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as UnitAttachment);
        }

        public override int GetHashCode() => Id;
    }
}
