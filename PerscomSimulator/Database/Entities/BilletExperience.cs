using CrossLite;
using CrossLite.CodeFirst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perscom.Database
{
    [Table]
    public class BilletExperience : IEquatable<BilletExperience>
    {
        [Column, PrimaryKey]
        public int BilletId { get; set; }

        [Column, PrimaryKey]
        public int ExperienceId { get; set; }

        [Column, Required, Default(1)]
        public int Rate { get; set; } = 1;

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
        /// Compares a <see cref="BilletExperience"/> with this one, and returns whether
        /// or not the RankId and GeneratorId's match
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsDuplicateOf(BilletExperience other)
        {
            return (BilletId == other.BilletId && ExperienceId == other.ExperienceId);
        }

        public bool Equals(BilletExperience other)
        {
            if (other == null) return false;
            return (this.IsDuplicateOf(other));
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
