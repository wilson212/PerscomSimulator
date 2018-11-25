using CrossLite;
using CrossLite.CodeFirst;
using System;

namespace Perscom.Database
{
    [Table]
    public class SoldierPoolFilter : AbstractFilter, IEquatable<SoldierPoolFilter>
    {
        #region Columns

        /// <summary>
        /// Gets or Sets the <see cref="SoldierGeneratorPool.Id"/> that this entity references
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
        /// Compares a <see cref="SoldierPoolFilter"/> with this one, and returns whether
        /// the identifying properties match
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsDuplicateOf(SoldierPoolFilter other)
        {
            return (Selector == other.Selector
                && SelectorId == other.SelectorId
                && Operator == other.Operator
                && RightValue == other.RightValue
            );
        }

        public bool Equals(SoldierPoolFilter other)
        {
            if (other == null) return false;
            return (this.IsDuplicateOf(other));
        }
    }
}
