using CrossLite;
using CrossLite.CodeFirst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perscom.Database
{
    [Table(WithoutRowID: true)]
    public class PositionBlueprintExperience : EntityBase, IEquatable<PositionBlueprintExperience>
    {
        #region Columns

        [Column, PrimaryKey]
        public virtual int PositionBlueprintId { get; set; }

        [Column, PrimaryKey]
        public virtual int ExperienceId { get; set; }

        [Column, Required, Default(1)]
        public virtual int Rate { get; set; } = 1;

        #endregion

        #region Foreign Keys

        /// <summary>
        /// Gets or Sets the <see cref="Database.PositionBlueprint"/> that 
        /// this entity references.
        /// </summary>
        [ForeignKey(nameof(PositionBlueprintId))]
        [References(nameof(Database.PositionBlueprint.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual PositionBlueprint Blueprint { get; set; }

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
        /// Compares a <see cref="PositionBlueprintExperience"/> with this one, and returns whether
        /// or not the BilletId and ExperienceId match
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsDuplicateOf(PositionBlueprintExperience other)
        {
            if (other == null) return false;
            return (PositionBlueprintId == other.PositionBlueprintId && ExperienceId == other.ExperienceId);
        }

        public bool Equals(PositionBlueprintExperience other)
        {
            return IsDuplicateOf(other);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as PositionBlueprintExperience);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (PositionBlueprintId * 397) ^ ExperienceId;
            }
        }
    }
}
