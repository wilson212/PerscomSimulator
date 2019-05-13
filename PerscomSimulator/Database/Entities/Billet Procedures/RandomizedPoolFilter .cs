using CrossLite;
using CrossLite.CodeFirst;
using System;

namespace Perscom.Database
{
    [Table]
    public class RandomizedPoolFilter : AbstractFilter, IEquatable<RandomizedPoolFilter>
    {
        #region Columns

        /// <summary>
        /// Gets or Sets the <see cref="RandomizedPool.Id"/> that this entity references
        /// </summary>
        [Column, PrimaryKey]
        public int RandomizedPoolId { get; protected set; }

        #endregion

        #region Virtual Foreign Keys

        /// <summary>
        /// Gets the <see cref="RandomizedProcedure"/> entity that this entity references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("RandomizedPoolId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<RandomizedPool> FK_Pool { get; set; }

        #endregion

        #region Foreign Key Properties

        /// <summary>
        /// Gets or Sets the <see cref="Perscom.Database.RandomizedProcedure"/> that 
        /// this entity references.
        /// </summary>
        public RandomizedPool RandomizedPool
        {
            get
            {
                return FK_Pool?.Fetch();
            }
            set
            {
                RandomizedPoolId = value.Id;
                FK_Pool?.Refresh();
            }
        }

        #endregion

        /// <summary>
        /// Compares a <see cref="RandomizedPoolFilter"/> with this one, and returns whether
        /// the identifying properties match
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsDuplicateOf(RandomizedPoolFilter other)
        {
            return (Selector == other.Selector
                && SelectorId == other.SelectorId
                && Operator == other.Operator
                && RightValue == other.RightValue
            );
        }

        public bool Equals(RandomizedPoolFilter other)
        {
            if (other == null) return false;
            return (this.IsDuplicateOf(other));
        }
    }
}
