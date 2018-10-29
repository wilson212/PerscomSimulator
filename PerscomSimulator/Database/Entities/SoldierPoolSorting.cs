using CrossLite;
using CrossLite.CodeFirst;
using CrossLite.QueryBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perscom.Database
{
    [Table]
    [CompositeUnique(nameof(SoldierGeneratorPoolId), nameof(Precedence))]
    public class SoldierPoolSorting : IEquatable<SoldierPoolSorting>
    {
        #region Columns

        /// <summary>
        /// The Unique SoldierGeneratorSetting ID (Row ID)
        /// </summary>
        [Column, PrimaryKey]
        public int SoldierGeneratorPoolId { get; protected set; }

        /// <summary>
        /// Gets or Sets the <see cref="SoldierGenerator.Id"/> that this entity references
        /// </summary>
        [Column, PrimaryKey]
        public SoldierSorting SortBy { get; set; }

        /// <summary>
        /// Indicates the order or priority this sorting is applied
        /// </summary>
        [Column, Required]
        public int Precedence { get; set; }

        /// <summary>
        /// The sorting direction
        /// </summary>
        [Column, Required]
        public Sorting Direction { get; set; }

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
        /// or not the RankId and GeneratorId's match
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsDuplicateOf(SoldierPoolSorting other)
        {
            return (SoldierGeneratorPoolId == other.SoldierGeneratorPoolId && SortBy == other.SortBy);
        }

        public bool Equals(SoldierPoolSorting other)
        {
            if (other == null) return false;
            return (this.IsDuplicateOf(other));
        }
    }
}
