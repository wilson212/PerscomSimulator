using CrossLite;
using CrossLite.CodeFirst;
using System;

namespace Perscom.Database
{
    [Table]
    public class SoldierExperience : EntityBase, IEquatable<SoldierExperience>
    {
        #region Columns

        /// <summary>
        /// Gets or sets the <see cref="Soldier.Id"/>
        /// </summary>
        [Column, PrimaryKey]
        public virtual int SoldierId { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="Experience.Id"/> that this entity references
        /// </summary>
        [Column, PrimaryKey]
        public virtual int ExperienceId { get; set; }

        /// <summary>
        /// The condition value
        /// </summary>
        [Column, Required, Default(0)]
        public virtual int Value { get; set; }

        #endregion Columns

        #region Foreign Keys

        /// <summary>
        /// Gets or Sets the <see cref="Database.Soldier"/> that 
        /// this entity references.
        /// </summary>
        [ForeignKey(nameof(SoldierId))]
        [References(nameof(Database.Soldier.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual Soldier Soldier { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="Database.Experience"/> that 
        /// this entity references.
        /// </summary>
        [ForeignKey(nameof(ExperienceId))]
        [References(nameof(Database.Experience.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual Experience Experience { get; set; }

        #endregion

        /// <summary>
        /// Compares a <see cref="SoldierExperience"/> with this one, and returns whether
        /// or not the RankId and GeneratorId's match
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsDuplicateOf(SoldierExperience other)
        {
            if (other == null) return false;
            return (SoldierId == other.SoldierId && ExperienceId == other.ExperienceId);
        }

        public bool Equals(SoldierExperience other)
        {
            return IsDuplicateOf(other);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SoldierExperience);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (SoldierId * 397) ^ ExperienceId;
            }
        }
    }
}
