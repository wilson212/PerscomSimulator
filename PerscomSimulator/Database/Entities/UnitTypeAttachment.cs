using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    /// <summary>
    /// Represents a relationship between 2 <see cref="Database.UnitType"/>'s
    /// </summary>
    [Table]
    public class UnitTypeAttachment
    {
        #region Column Properties

        /// <summary>
        /// Gets or sets the parent <see cref="UnitType.Id"/>
        /// </summary>
        [Column, Required, PrimaryKey]
        public int ParentId { get; set; }

        /// <summary>
        /// Gets or sets the child <see cref="UnitType.Id"/>
        /// </summary>
        [Column, Required, Unique, PrimaryKey]
        public int ChildId { get; set; }

        /// <summary>
        /// Gets or sets the number of child <see cref="UnitType"/> units attached
        /// to this parent <see cref="UnitType"/>
        /// </summary>
        public int Count { get; set; }

        #endregion

        #region Virtual Foreign Keys

        [InverseKey("Id")]
        [ForeignKey("ParentId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<UnitType> FK_Parent { get; set; }

        [InverseKey("Id")]
        [ForeignKey("ChildId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<UnitType> FK_Child { get; set; }

        #endregion

        #region Foreign Key Properties

        /// <summary>
        /// Gets or Sets the <see cref="Perscom.Database.Soldier"/> that 
        /// this position will hold.
        /// </summary>
        public UnitType ParentUnitType
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
        public UnitType ChildUnitType
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
    }
}
