using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    [Table]
    public class Echelon
    {
        #region Columns

        /// <summary>
        /// The Unique Echelon ID
        /// </summary>
        [Column, PrimaryKey]
        public int Id { get; protected set; }

        /// <summary>
        /// Gets or Sets the string name of this Echelon
        /// </summary>
        [Column, Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the hierarchy level of this Echelon. Higher value
        /// means higher up the ladder.
        /// </summary>
        [Column, Required]
        public int HierarchyLevel { get; set; }

        #endregion

        #region Child Database Sets

        /// <summary>
        /// Gets a list of <see cref="UnitType"/> entities that reference this 
        /// <see cref="Echelon"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration that fetches all Torque Ratios
        /// that are bound by the foreign key and this Engine.Id.
        /// </remarks>
        public virtual IEnumerable<UnitType> UnitTypes { get; set; }

        #endregion
    }
}
