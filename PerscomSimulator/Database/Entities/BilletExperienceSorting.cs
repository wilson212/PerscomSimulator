using CrossLite;
using CrossLite.CodeFirst;
using CrossLite.QueryBuilder;
using System;

namespace Perscom.Database
{
    [Table]
    public class BilletExperienceSorting : IEquatable<BilletExperienceSorting>
    {
        #region Columns

        /// <summary>
        /// The <see cref="Database.Billet"/> entity we are a child of
        /// </summary>
        [Column, PrimaryKey]
        public int BilletId { get; set; }

        /// <summary>
        /// Indicates the order or priority this sorting is applied
        /// </summary>
        [Column, PrimaryKey]
        public int Precedence { get; set; }

        /// <summary>
        /// The <see cref="Database.Experience"/> entity we are a child of
        /// </summary>
        [Column, Required]
        public int ExperienceId { get; set; }

        /// <summary>
        /// The sorting direction
        /// </summary>
        [Column, Required]
        public Sorting Direction { get; set; }

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

        /// <summary>
        /// Gets the <see cref="Database.Experience"/> entity that this entity references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("ExperienceId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<Experience> FK_Experience { get; set; }

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

        /// <summary>
        /// Gets or Sets the <see cref="Database.Experience"/> that 
        /// this entity references.
        /// </summary>
        public Experience Experience
        {
            get
            {
                return FK_Experience?.Fetch();
            }
            set
            {
                ExperienceId = value.Id;
                FK_Experience?.Refresh();
            }
        }

        #endregion

        /// <summary>
        /// Compares a <see cref="BilletExperienceSorting"/> with this one, and returns whether
        /// or not the RankId and GeneratorId's match
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsDuplicateOf(BilletExperienceSorting other)
        {
            return (BilletId == other.BilletId && ExperienceId == other.ExperienceId);
        }

        public bool Equals(BilletExperienceSorting other)
        {
            if (other == null) return false;
            return (this.IsDuplicateOf(other));
        }
    }
}
