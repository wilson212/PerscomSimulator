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
    [CompositeUnique(nameof(GeneratorId), nameof(RankId))]
    public class SoldierGeneratorPool : ISpawnable, IEquatable<SoldierGeneratorPool>
    {
        #region Columns

        /// <summary>
        /// The Unique SoldierGeneratorSetting ID (Row ID)
        /// </summary>
        [Column, PrimaryKey]
        public int Id { get; protected set; }

        /// <summary>
        /// Gets or Sets the <see cref="SoldierGenerator.Id"/> that this entity references
        /// </summary>
        [Column, Required]
        public int GeneratorId { get; set; }

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
        /// a <see cref="SoldierCareerAdjustment"/> for their new <see cref="Database.Rank"/>
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
        /// DEPRECIATED
        /// </summary>
        [Column, Required, Default(0)]
        public bool OrderedBySeniority { get; set; }

        /// <summary>
        /// DEPRECIATED
        /// </summary>
        [Column, Required, Default(0)]
        public SoldierSorting FirstOrderedBy { get; set; }

        /// <summary>
        /// DEPRECIATED
        /// </summary>
        [Column, Required, Default(0)]
        public Sorting FirstOrder { get; set; } = Sorting.Ascending;

        /// <summary>
        /// DEPRECIATED
        /// </summary>
        [Column, Required, Default(0)]
        public SoldierSorting ThenOrderedBy { get; set; }

        /// <summary>
        /// DEPRECIATED
        /// </summary>
        [Column, Required, Default(0)]
        public Sorting ThenOrder { get; set; } = Sorting.Ascending;

        #endregion

        #region Virtual Foreign Keys

        /// <summary>
        /// Gets the <see cref="SoldierGenerator"/> entity that this entity references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("GeneratorId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<SoldierGenerator> FK_Generator { get; set; }

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
        /// Gets or Sets the <see cref="Perscom.Database.SoldierGenerator"/> that 
        /// this entity references.
        /// </summary>
        public SoldierGenerator Generator
        {
            get
            {
                return FK_Generator?.Fetch();
            }
            set
            {
                GeneratorId = value.Id;
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
        /// Gets a list of <see cref="SoldierCareerAdjustment"/> entities that reference this 
        /// <see cref="SoldierGeneratorPool"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual IEnumerable<SoldierCareerAdjustment> CareerSettings { get; set; }

        /// <summary>
        /// Returns the <see cref="Database.CareerGenerator"/> linked to this 
        /// <see cref="SoldierGeneratorPool"/> if any.
        /// </summary>
        public CareerGenerator CareerGenerator
        {
            get
            {
                var item = CareerSettings?.FirstOrDefault();
                if (item == null || item == default(SoldierCareerAdjustment))
                    return null;

                return item.CareerGenerator;
            }
        }

        /// <summary>
        /// Gets a list of <see cref="SoldierPoolFilter"/> entities that reference this 
        /// <see cref="SoldierGeneratorPool"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual IEnumerable<SoldierPoolFilter> SoldierFiltering { get; set; }

        /// <summary>
        /// Gets a list of <see cref="SoldierPoolSorting"/> entities that reference this 
        /// <see cref="SoldierGeneratorPool"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual IEnumerable<SoldierPoolSorting> SoldierSorting { get; set; }

        #endregion

        /// <summary>
        /// Indicates  whether this <see cref="SoldierGeneratorPool"/> was modified 
        /// in the <see cref="SoldierGeneratorPoolForm"/> since it was last loaded
        /// from the database
        /// </summary>
        public bool EditedInEditorForm { get; set; } = false;

        /// <summary>
        /// Indicates a <see cref="Database.CareerGenerator"/> selection change from the
        /// selected <see cref="Database.CareerGenerator"/> in the database
        /// </summary>
        public CareerGenerator TemporaryCareer { get; set; }

        /// <summary>
        /// Indicates a modified set of <see cref="SoldierPoolSorting"/> selections
        /// from the database version of this instance
        /// </summary>
        public List<SoldierPoolSorting> TemporarySoldierSorting { get; set; }

        /// <summary>
        /// Indicates a modified set of <see cref="SoldierPoolFilter"/> selections
        /// from the database version of this instance
        /// </summary>
        public List<SoldierPoolFilter> TemporarySoldierFiltering { get; set; }

        /// <summary>
        /// Compares a <see cref="SoldierGeneratorPool"/> with this one, and returns whether
        /// or not the RankId and GeneratorId's match
        /// </summary>
        /// <remarks>Used in the <see cref="SoldierGeneratorEditorForm"/></remarks>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsDuplicateOf(SoldierGeneratorPool other)
        {
            return (RankId == other.RankId && GeneratorId == other.GeneratorId);
        }

        public bool Equals(SoldierGeneratorPool other)
        {
            if (other == null) return false;
            return (Id == other.Id);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as SoldierGeneratorPool);
        }

        public override int GetHashCode() => Id;
    }
}
