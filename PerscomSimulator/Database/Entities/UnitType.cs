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
    public class UnitType
    {
        #region Columns

        /// <summary>
        /// The Unique Unit Type ID
        /// </summary>
        [Column, PrimaryKey]
        public int Id { get; protected set; }

        /// <summary>
        /// Gets or Sets the <see cref="Echelon"/> object
        /// ID that this entity references
        /// </summary>
        [Column, Required]
        public int EchelonId { get; set; }

        /// <summary>
        /// Gets or Sets the string name of this UnitType
        /// </summary>
        [Column, Required]
        public string Name { get; set; }

        #endregion

        #region Virtual Foreign Keys

        [InverseKey("Id")]
        [ForeignKey("EchelonId",
            OnDelete = ReferentialIntegrity.Restrict,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<Echelon> FK_Echelon { get; set; }

        #endregion

        #region Foreign Key Properties

        /// <summary>
        /// Gets or Sets the <see cref="Echelon"/> that 
        /// this unit type falls under.
        /// </summary>
        public Echelon Echelon
        {
            get
            {
                return FK_Echelon?.Fetch();
            }
            set
            {
                EchelonId = value.Id;
                FK_Echelon?.Refresh();
            }
        }

        #endregion

        #region Child Database Sets

        /// <summary>
        /// Gets a list of <see cref="Unit"/> entities that reference this 
        /// <see cref="UnitType"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration that fetches all Torque Ratios
        /// that are bound by the foreign key and this Engine.Id.
        /// </remarks>
        public virtual IEnumerable<Unit> Units { get; set; }

        #endregion
    }
}
