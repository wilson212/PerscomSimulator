using CrossLite;
using CrossLite.CodeFirst;
using System;

namespace Perscom.Database
{
    [Table]
    public class BilletSelectionSorting : AbstractSort, IEquatable<BilletSelectionSorting>
    {
        #region Columns

        /// <summary>
        /// The <see cref="Database.Billet"/> entity we are a child of
        /// </summary>
        [Column, PrimaryKey]
        public int BilletId { get; set; }

        #endregion Columns

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

        #endregion

        #region Foreign Key Properties

        /// <summary>
        /// Gets or Sets the <see cref="Database.Billet"/> that 
        /// this entity references.
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

        #endregion

        /// <summary>
        /// Compares a <see cref="BilletSelectionSorting"/> with this one, and returns whether
        /// the identifying properties match
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsDuplicateOf(BilletSelectionSorting other)
        {
            return (
                Selector == other.Selector
                && SelectorId == other.SelectorId
                && Direction == other.Direction
            );
        }

        public bool Equals(BilletSelectionSorting other)
        {
            if (other == null) return false;
            return (this.IsDuplicateOf(other));
        }
    }
}
