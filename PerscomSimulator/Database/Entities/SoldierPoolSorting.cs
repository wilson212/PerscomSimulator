using CrossLite;
using CrossLite.CodeFirst;
using System;

namespace Perscom.Database
{
    [Table]
    public class SoldierPoolSorting : AbstractSort, IEquatable<SoldierPoolSorting>
    {
        #region Columns

        /// <summary>
        /// The Unique SoldierGeneratorSetting ID (Row ID)
        /// </summary>
        [Column, PrimaryKey]
        public int SoldierGeneratorPoolId { get; protected set; }

        #endregion

        #region Virtual Foreign Keys

        /// <summary>
        /// Gets the <see cref="SoldierGenerator"/> entity that this entity references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("SoldierGeneratorPoolId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<SoldierGeneratorPool> FK_Pool { get; set; }

        #endregion

        #region Foreign Key Properties

        /// <summary>
        /// Gets or Sets the <see cref="Perscom.Database.SoldierGenerator"/> that 
        /// this entity references.
        /// </summary>
        public SoldierGeneratorPool SoldierPool
        {
            get
            {
                return FK_Pool?.Fetch();
            }
            set
            {
                SoldierGeneratorPoolId = value.Id;
                FK_Pool?.Refresh();
            }
        }

        #endregion

        /// <summary>
        /// Compares a <see cref="SoldierPoolSorting"/> with this one, and returns whether
        /// the identifying properties match
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsDuplicateOf(SoldierPoolSorting other)
        {
            return (
                Selector == other.Selector
                && SelectorId == other.SelectorId
                && Direction == other.Direction
            );
        }

        public bool Equals(SoldierPoolSorting other)
        {
            if (other == null) return false;
            return (this.IsDuplicateOf(other));
        }
    }
}
