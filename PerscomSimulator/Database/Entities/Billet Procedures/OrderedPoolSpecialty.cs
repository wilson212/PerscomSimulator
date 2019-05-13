using CrossLite;
using CrossLite.CodeFirst;
using System;

namespace Perscom.Database
{
    /// <summary>
    /// Represents a constraint that signifies that a <see cref="Database.OrderedPool"/> requires 
    /// a <see cref="Soldier"/> to have a specific <see cref="Database.Specialty"/> to enter it.
    /// </summary>
    [Table]
    public class OrderedPoolSpecialty : IEquatable<OrderedPoolSpecialty>
    {
        /// <summary>
        /// The Unique Requirement ID (Row ID)
        /// </summary>
        [Column, PrimaryKey]
        public int Id { get; protected set; }

        /// <summary>
        /// Gets or Sets the <see cref="Database.Billet"/> object
        /// ID that this entity references
        /// </summary>
        [Column, Required]
        public int OrderedPoolId { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="Database.Specialty"/> the <see cref="Soldier"/>
        /// holding this billet should be.
        /// </summary>
        [Column, Required]
        public int SpecialtyId { get; set; }

        #region Foreign Keys

        /// <summary>
        /// Gets the <see cref="Database.OrderedPool"/> entity that this position references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("OrderedPoolId",
             OnDelete = ReferentialIntegrity.Cascade,
             OnUpdate = ReferentialIntegrity.Cascade
         )]
        protected virtual ForeignKey<OrderedPool> FK_ProcedurePool { get; set; }

        /// <summary>
        /// Gets the <see cref="Database.Specialty"/> entity that this position references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("SpecialtyId",
             OnDelete = ReferentialIntegrity.Cascade,
             OnUpdate = ReferentialIntegrity.Cascade
         )]
        protected virtual ForeignKey<Specialty> FK_Specialty { get; set; }

        #endregion

        #region Foreign Key Properties

        /// <summary>
        /// Gets the <see cref="Database.OrderedPool"/> entity that this requirement references.
        /// </summary>
        public OrderedPool OrderedPool
        {
            get
            {
                return FK_ProcedurePool?.Fetch();
            }
            set
            {
                OrderedPoolId = value.Id;
                FK_ProcedurePool?.Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Database.Specialty"/> entity that this requirement references.
        /// </summary>
        public Specialty Specialty
        {
            get
            {
                return FK_Specialty?.Fetch();
            }
            set
            {
                SpecialtyId = value.Id;
                FK_Specialty?.Refresh();
            }
        }

        #endregion

        /// <summary>
        /// Compares a <see cref="ProcedurePoolSpecialtyRequirement"/> with this one, and returns whether
        /// or not the <see cref="OrderedPoolId"/> and <see cref="SpecialtyId"/> match.
        /// </summary>
        /// <remarks>Used in the <see cref="UnitTypeManagerForm"/></remarks>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsDuplicateOf(OrderedPoolSpecialty other)
        {
            return (OrderedPoolId == other.OrderedPoolId && SpecialtyId == other.SpecialtyId);
        }

        public bool Equals(OrderedPoolSpecialty other)
        {
            if (other == null) return false;
            return (Id == other.Id);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as OrderedPoolSpecialty);
        }

        public override int GetHashCode() => Id;
    }
}
