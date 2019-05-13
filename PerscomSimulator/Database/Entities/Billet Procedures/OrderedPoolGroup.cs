using CrossLite;
using CrossLite.CodeFirst;
using System;

namespace Perscom.Database
{
    [Table]
    public class OrderedPoolGroup : AbstractFilter, IEquatable<OrderedPoolGroup>
    {
        #region Columns

        /// <summary>
        /// Gets or Sets the <see cref="Database.OrderedPool.Id"/> that this entity references
        /// </summary>
        [Column, PrimaryKey]
        public int OrderedPoolId { get; protected set; }

        #endregion

        #region Virtual Foreign Keys

        /// <summary>
        /// Gets the <see cref="Database.OrderedPool"/> entity that this entity references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("OrderedPoolId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<OrderedPool> FK_Pool { get; set; }

        #endregion

        #region Foreign Key Properties

        /// <summary>
        /// Gets or Sets the <see cref="Database.OrderedPool"/> that 
        /// this entity references.
        /// </summary>
        public OrderedPool OrderedPool
        {
            get
            {
                return FK_Pool?.Fetch();
            }
            set
            {
                OrderedPoolId = value.Id;
                FK_Pool?.Refresh();
            }
        }

        #endregion

        /// <summary>
        /// Compares a <see cref="OrderedPoolGroup"/> with this one, and returns whether
        /// the identifying properties match
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsDuplicateOf(OrderedPoolGroup other)
        {
            return (Selector == other.Selector
                && SelectorId == other.SelectorId
                && Operator == other.Operator
                && RightValue == other.RightValue
            );
        }

        public bool Equals(OrderedPoolGroup other)
        {
            if (other == null) return false;
            return (this.IsDuplicateOf(other));
        }
    }
}
