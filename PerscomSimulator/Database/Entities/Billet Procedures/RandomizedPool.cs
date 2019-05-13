using System;
using System.Collections.Generic;
using System.Linq;
using CrossLite;
using CrossLite.CodeFirst;
using CrossLite.QueryBuilder;
using Perscom.Simulation;

namespace Perscom.Database
{
    [Table]
    [CompositeUnique(nameof(RandomizedProcedureId), nameof(RankId))]
    public class RandomizedPool : ISpawnable, IEquatable<RandomizedPool>
    {
        #region Columns

        /// <summary>
        /// The Unique SoldierGeneratorSetting ID (Row ID)
        /// </summary>
        [Column, PrimaryKey]
        public int Id { get; protected set; }

        /// <summary>
        /// Gets or Sets the <see cref="RandomizedProcedure.Id"/> that this entity references
        /// </summary>
        [Column, Required]
        public int RandomizedProcedureId { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="Rank.Id"/> the <see cref="Soldier"/>
        /// holding this billet should be.
        /// </summary>
        [Column, Required]
        public int RankId { get; set; }

        /// <summary>
        /// The chances of this object spawning relative to the maximum Probability
        /// of the <see cref="SpawnGenerator{T}"/>
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
        /// Gets or sets the logic operator when using filtering
        /// </summary>
        [Column, Required, Default(0)]
        public LogicOperator FilterLogic { get; set; }

        /// <summary>
        /// Indicates whether Soldier pool ordering is preformed before Billet Ordering
        /// </summary>
        [Column, Required, Default(0)]
        public bool OrdersBeforeBilletOrdering { get; set; }

        #endregion

        #region Virtual Foreign Keys

        /// <summary>
        /// Gets the <see cref="Database.RandomizedProcedure"/> entity that this entity references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("RandomizedProcedureId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<RandomizedProcedure> FK_Generator { get; set; }

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
        /// Gets or Sets the <see cref="Perscom.Database.RandomizedProcedure"/> that 
        /// this entity references.
        /// </summary>
        public RandomizedProcedure RandomizedProcedure
        {
            get
            {
                return FK_Generator?.Fetch();
            }
            set
            {
                RandomizedProcedureId = value.Id;
                FK_Generator?.Refresh();
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
        /// Gets a list of <see cref="RandomizedPoolCareer"/> entities that reference this 
        /// <see cref="RandomizedPool"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual IEnumerable<RandomizedPoolCareer> CareerSettings { get; set; }

        /// <summary>
        /// Returns the <see cref="Database.CareerGenerator"/> linked to this 
        /// <see cref="RandomizedPool"/> if any.
        /// </summary>
        public CareerGenerator CareerGenerator
        {
            get
            {
                var item = CareerSettings?.FirstOrDefault();
                if (item == null || item == default(RandomizedPoolCareer))
                    return null;

                return item.CareerGenerator;
            }
        }

        /// <summary>
        /// Gets a list of <see cref="RandomizedPoolFilter"/> entities that reference this 
        /// <see cref="RandomizedPool"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual IEnumerable<RandomizedPoolFilter> SoldierFiltering { get; set; }

        /// <summary>
        /// Gets a list of <see cref="RandomizedPoolSorting"/> entities that reference this 
        /// <see cref="RandomizedPool"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual IEnumerable<RandomizedPoolSorting> SoldierSorting { get; set; }

        #endregion

        /// <summary>
        /// Indicates  whether this <see cref="RandomizedPool"/> was modified 
        /// in the <see cref="RandomizedPoolForm"/> since it was last loaded
        /// from the database
        /// </summary>
        public bool EditedInEditorForm { get; set; } = false;

        /// <summary>
        /// Indicates a <see cref="Database.CareerGenerator"/> selection change from the
        /// selected <see cref="Database.CareerGenerator"/> in the database
        /// </summary>
        public CareerGenerator TemporaryCareer { get; set; }

        /// <summary>
        /// Indicates a modified set of <see cref="RandomizedPoolSorting"/> selections
        /// from the database version of this instance
        /// </summary>
        public List<RandomizedPoolSorting> TemporarySoldierSorting { get; set; }

        /// <summary>
        /// Indicates a modified set of <see cref="RandomizedPoolFilter"/> selections
        /// from the database version of this instance
        /// </summary>
        public List<RandomizedPoolFilter> TemporarySoldierFiltering { get; set; }

        /// <summary>
        /// Compares a <see cref="RandomizedPool"/> with this one, and returns whether
        /// or not the RankId and GeneratorId's match
        /// </summary>
        /// <remarks>Used in the <see cref="RandomizedProcedureForm"/></remarks>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsDuplicateOf(RandomizedPool other)
        {
            return (RankId == other.RankId && RandomizedProcedureId == other.RandomizedProcedureId);
        }

        public bool Equals(RandomizedPool other)
        {
            if (other == null) return false;
            return (Id == other.Id);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as RandomizedPool);
        }

        public override int GetHashCode() => Id;
    }
}
