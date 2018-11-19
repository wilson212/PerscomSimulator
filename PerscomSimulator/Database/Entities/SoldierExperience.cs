using CrossLite;
using CrossLite.CodeFirst;
using System;

namespace Perscom.Database
{
    [Table]
    public class SoldierExperience : IEquatable<SoldierExperience>
    {
        #region Columns

        [Column, PrimaryKey]
        public int SoldierId { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="Experience.Id"/> that this entity references
        /// </summary>
        [Column, PrimaryKey]
        public int ExperienceId { get; set; }

        /// <summary>
        /// The condition value
        /// </summary>
        [Column, Required, Default(0)]
        public int Value { get; set; }

        #endregion Columns

        #region Virtual Foreign Keys

        /// <summary>
        /// Gets the <see cref="Database.Soldier"/> entity that this entity references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("SoldierId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<Soldier> FK_Soldier { get; set; }

        /// <summary>
        /// Gets the <see cref="Database.Experience"/> entity that this entity references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("ExperienceId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<Experience> FK_Experience { get; set; }

        #endregion Virtual Foreign Keys

        #region Foreign Key Properties

        /// <summary>
        /// Gets or Sets the <see cref="Database.Soldier"/> that 
        /// this entity references.
        /// </summary>
        public Soldier Soldier
        {
            get
            {
                return FK_Soldier?.Fetch();
            }
            set
            {
                SoldierId = value.Id;
                FK_Soldier?.Refresh();
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
        /// Compares a <see cref="SoldierExperience"/> with this one, and returns whether
        /// or not the RankId and GeneratorId's match
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsDuplicateOf(SoldierExperience other)
        {
            return (SoldierId == other.SoldierId && ExperienceId == other.ExperienceId);
        }

        public bool Equals(SoldierExperience other)
        {
            if (other == null) return false;
            return (this.IsDuplicateOf(other));
        }
    }
}
