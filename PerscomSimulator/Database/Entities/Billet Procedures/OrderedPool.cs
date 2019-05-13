using CrossLite;
using CrossLite.CodeFirst;
using CrossLite.QueryBuilder;
using System.Collections.Generic;
using System.Linq;

namespace Perscom.Database
{
    [Table]
    //[CompositeUnique(nameof(OrderedProcedureId), nameof(Precedence))]
    public class OrderedPool
    {
        #region Columns

        /// <summary>
        /// The Unique ProcedurePool ID (Row ID)
        /// </summary>
        [Column, PrimaryKey]
        public int Id { get; protected set; }

        /// <summary>
        /// Gets or Sets the <see cref="OrderedProcedure.Id"/> that this entity references
        /// </summary>
        [Column, Required]
        public int OrderedProcedureId { get; set; }

        /// <summary>
        /// Indicates the order or priority this condition is applied
        /// </summary>
        [Column, Required]
        public int Precedence { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="Rank.Id"/> the <see cref="Soldier"/>
        /// holding this billet should be.
        /// </summary>
        [Column, Required]
        public int RankId { get; set; }

        /// <summary>
        /// The chances of this pool producing a soldier. An example could
        /// be the interest level of the billet in question
        /// </summary>
        [Column, Required]
        public int Probability { get; set; }

        /// <summary>
        /// Gets or sets the whether we select by <see cref="Rank.Id"/>
        /// or by <see cref="Rank.Grade"/>.
        /// </summary>
        [Column, Required, Default(0)]
        public bool UseRankGrade { get; set; }

        /// <summary>
        /// Indicates whether the soldier being selected is to recieve
        /// a <see cref="RandomizedPoolCareer"/> for their new <see cref="Database.Rank"/>
        /// </summary>
        [Column, Required, Default(0)]
        public bool NewCareerLength { get; set; }

        /// <summary>
        /// Indicates whether the soldier being selected from an exisiting
        /// pool must be promotable to be spawnable
        /// </summary>
        [Column, Required, Default(0)]
        public bool MustBePromotable { get; set; }

        /// <summary>
        /// Indicates whether the soldiers being selected from an exisiting
        /// pool must not be locked into their current position by Minimum
        /// time in billet
        /// </summary>
        [Column, Required, Default(0)]
        public bool NotLockedInBillet { get; set; }

        /// <summary>
        /// Indicates whether to select the first soldier in the stack after
        /// sorting is applied, or ignore sorting and select a random soldier
        /// </summary>
        [Column, Required, Default(0)]
        public bool SelectRandom { get; set; }

        /// <summary>
        /// Indicates whether to group soldiers by thier desire rating for the position.
        /// This is only useful for lateral movement
        /// </summary>
        [Column, Required, Default(0)]
        public bool GroupByDesire { get; set; }

        /// <summary>
        /// Indicates whether the <see cref="Specialty"/> requirements are inversed, meaning
        /// that the <see cref="Soldier"/> must NOT have the <see cref="Specialty"/> listed
        /// to be considered for this billet
        /// </summary>
        [Column, Required, Default(0)]
        public bool InverseSpecialtyRequirements { get; set; }

        /// <summary>
        /// Gets or sets the logic operator when using filtering
        /// </summary>
        [Column, Required, Default(0)]
        public LogicOperator FilterLogic { get; set; }

        #endregion

        #region Virtual Foreign Keys

        /// <summary>
        /// Gets the <see cref="OrderedProcedure"/> entity that this entity references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("OrderedProcedureId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<OrderedProcedure> FK_Procedure { get; set; }

        /// <summary>
        /// Gets the <see cref="Rank"/> entity that this entity references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("RankId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<Rank> FK_Rank { get; set; }

        #endregion

        #region Foreign Key Properties

        /// <summary>
        /// Gets or Sets the <see cref="Perscom.Database.OrderedProcedure"/> that 
        /// this entity references.
        /// </summary>
        public OrderedProcedure OrderedProcedure
        {
            get
            {
                return FK_Procedure?.Fetch();
            }
            set
            {
                OrderedProcedureId = value.Id;
                FK_Procedure?.Refresh();
            }
        }

        /// <summary>
        /// Gets or Sets the <see cref="Perscom.Database.Rank"/> that 
        /// this entity references.
        /// </summary>
        public Rank Rank
        {
            get
            {
                return FK_Rank?.Fetch();
            }
            set
            {
                RankId = value.Id;
                FK_Rank?.Refresh();
            }
        }

        #endregion

        #region Child Database Sets

        /// <summary>
        /// Gets a list of <see cref="OrderedPoolCareer"/> entities that reference this 
        /// <see cref="OrderedPool"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual IEnumerable<OrderedPoolCareer> CareerSettings { get; set; }

        /// <summary>
        /// Returns the <see cref="Database.CareerGenerator"/> linked to this 
        /// <see cref="RandomizedPool"/> if any.
        /// </summary>
        public CareerGenerator CareerGenerator
        {
            get
            {
                var item = CareerSettings?.FirstOrDefault();
                if (item == null || item == default(OrderedPoolCareer))
                    return null;

                return item.CareerGenerator;
            }
        }

        /// <summary>
        /// Gets a list of <see cref="OrderedPoolFilter"/> entities that reference this 
        /// <see cref="OrderedPool"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual IEnumerable<OrderedPoolFilter> SoldierFilters { get; set; }

        /// <summary>
        /// Gets a list of <see cref="OrderedPoolGroup"/> entities that reference this 
        /// <see cref="OrderedPool"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual IEnumerable<OrderedPoolGroup> SoldierGroups { get; set; }

        /// <summary>
        /// Gets a list of <see cref="OrderedPoolSorting"/> entities that reference this 
        /// <see cref="OrderedPool"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual IEnumerable<OrderedPoolSorting> SoldierSorting { get; set; }

        /// <summary>
        /// Gets a list of <see cref="OrderedPoolSpecialty"/> entities that reference this 
        /// <see cref="OrderedPool"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual IEnumerable<OrderedPoolSpecialty> Requirements { get; set; }

        #endregion

        public bool Equals(OrderedPool other)
        {
            if (other == null) return false;
            return (Id == other.Id);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as OrderedPool);
        }

        public override int GetHashCode() => Id;
    }
}
