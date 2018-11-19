using CrossLite;
using CrossLite.CodeFirst;
using CrossLite.QueryBuilder;
using System;

namespace Perscom.Database
{
    [Table]
    [CompositeUnique(nameof(ExperienceId), nameof(BilletId))]
    public class BilletExperienceGroup
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

        [Column, Required]
        public ConditionOperator Operator { get; set; }

        /// <summary>
        /// The value to Group By if <see cref="SortMode"/> is set to <see cref="SortMode.GroupByValue"/>
        /// </summary>
        [Column, Required, Default(0)]
        public int Value { get; set; }

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
        /// Compares a <see cref="BilletExperienceGroup"/> with this one, and returns whether
        /// or not the RankId and GeneratorId's match
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsDuplicateOf(BilletExperienceGroup other)
        {
            return (
                BilletId == other.BilletId
                && ExperienceId == other.ExperienceId
            );
        }

        public bool Equals(BilletExperienceGroup other)
        {
            if (other == null) return false;
            return (this.IsDuplicateOf(other));
        }
    }
}
