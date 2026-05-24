using System;
using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    [Table]
    public class PositionCatagory : EntityBase, IEquatable<PositionCatagory>
    {
        #region Columns

        /// <summary>
        /// The IsUnique PositionCatagory ID
        /// </summary>
        [Column, PrimaryKey]
        public virtual int Id { get; protected set; }

        /// <summary>
        /// Gets or Sets the string name of this catagory
        /// </summary>
        [Column, Required]
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the ordering of this category when filling in DropDownLists. Higher value
        /// means it will appear higher in the list.
        /// </summary>
        [Column, Required]
        public virtual int ZIndex { get; set; }
        
        /// <summary>
        /// The vertical tier/level in the org chart. Lower values appear higher.
        /// E.g., 0 = top (Commander), 1 = staff level, 2 = S-Shop level.
        /// </summary>
        [Column, Required, Default(OrgChartPosition.GeneralStaff)]
        public virtual OrgChartPosition OrgChartLevel { get; set; } = OrgChartPosition.GeneralStaff;

        /// <summary>
        /// Defines how this category is positioned in the org chart tree.
        /// Center = hangs straight down from the parent (main trunk).
        /// Side = branches off to the side, rendered just above the split
        /// of all Center categories at the next level.
        /// </summary>
        [Column, Required, Default(OrgChartAlignment.Center)]
        public virtual OrgChartAlignment OrgChartAlignment { get; set; } = OrgChartAlignment.Center;


        #endregion

        public bool Equals(PositionCatagory other)
        {
            if (other == null) return false;
            return (Id == other.Id);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as PositionCatagory);
        }

        public override int GetHashCode() => Id;

        public override string ToString() => Name;
    }
}
