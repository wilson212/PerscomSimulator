using System;
using System.Collections.Generic;
using CrossLite;
using CrossLite.CodeFirst;
using CrossLite.QueryBuilder;
using Perscom.Simulation;

namespace Perscom.Database
{
    /// <summary>
    /// Represents an abstract set of rules that child <see cref="Position"/>'s will inherit
    /// </summary>
    [Table]
    public class Billet : IEquatable<Billet>
    {
        #region Columns

        /// <summary>
        /// The Unique Billet ID
        /// </summary>
        [Column, PrimaryKey]
        public int Id { get; protected set; }

        /// <summary>
        /// Gets or Sets the <see cref="UnitType"/> object
        /// ID that this entity references
        /// </summary>
        [Column, Required]
        public int UnitTypeId { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="BilletCatagory"/> object
        /// ID that this entity references
        /// </summary>
        [Column, Required]
        public int BilletCatagoryId { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="Rank.Id"/> the <see cref="Soldier"/>
        /// holding this billet should be.
        /// </summary>
        [Column, Required]
        public int RankId { get; set; }

        /// <summary>
        /// Gets or Sets the maximum <see cref="Rank.Id"/> the <see cref="Soldier"/>
        /// holding this billet can be.
        /// </summary>
        [Column, Required]
        public int MaxRankId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Echelon"/> level in which we will find soldiers
        /// to fill this position
        /// </summary>
        [Column, Required]
        public int PromotionPoolId { get; set; }

        /// <summary>
        /// Gets or Sets the string name of this Unit
        /// </summary>
        [Column, Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the prominence level of the position. A higher value will cause
        /// this <see cref="Billet"/> to be filled with more experienced (time in grade) 
        /// soldiers, while lower values will be filled with more inexperienced soldiers.
        /// </summary>
        /// <remarks>
        /// Entry level billits should be set to a value of zero, while positions that require
        /// seniority should have a higher value.
        /// </remarks>
        [Column, Required, Default(0)]
        public int Stature { get; set; } = 0;

        /// <summary>
        /// Gets or sets the minimum time (months) a soldier must hold this <see cref="Billet"/> 
        /// before being allowed to leave. If the minimum amount is less than the remaining time 
        /// to live for the soldier, their retirement date will be adjusted accordingly.
        /// </summary>
        [Column, Required, Default(0)]
        public int MinTourLength { get; set; } = 0;

        /// <summary>
        /// Gets or sets the maximum time (months) a soldier can hold this <see cref="Billet"/>
        /// before being forcefully pushed out.
        /// </summary>
        [Column, Required, Default(0)]
        public int MaxTourLength { get; set; } = 0;

        /// <summary>
        /// Indicates whether the soldier holding this <see cref="Billet"/> can retire before
        /// meeting the <see cref="MinTourLength"/>.
        /// </summary>
        [Column, Required, Default(1)]
        public bool CanRetireEarly { get; set; } = true;

        /// <summary>
        /// Indicates whether the soldier holding this <see cref="Billet"/> can be selected
        /// for promotion before meeting the <see cref="MinTourLength"/>.
        /// </summary>
        [Column, Required, Default(1)]
        public bool CanBePromotedEarly { get; set; } = true;

        /// <summary>
        /// Indicates whether the soldier holding this <see cref="Billet"/> can be selected
        /// for a lateral promotion before meeting the <see cref="MinTourLength"/>.
        /// </summary>
        [Column, Required, Default(0)]
        public bool CanLateralEarly { get; set; } = false;

        /// <summary>
        /// Indicates whether the soldier holding this <see cref="Billet"/> is limited to
        /// the <see cref="MaxTourLength"/>, or can extend past this
        /// </summary>
        [Column("Repeatable"), Required, Default(1)]
        public bool Waiverable { get; set; } = true;

        /// <summary>
        /// DEPRECIATED
        /// </summary>
        [Column, Required, Default(0)]
        public bool PreferNonRepeats { get; set; }

        /// <summary>
        /// Gets or sets the selection process the <see cref="Simulator"/>
        /// will use to fill this position when it is empty
        /// </summary>
        [Column, Required, Default(0)]
        public BilletSelection Selection { get; set; } = BilletSelection.PromotionOrLateral;

        /// <summary>
        /// Indicates whether the required <see cref="Specialty"/> are inversed, meaning
        /// that the <see cref="Soldier"/> must NOT have the <see cref="Specialty"/> listed
        /// to be considered for this billet
        /// </summary>
        [Column, Required, Default(0)]
        public bool InverseRequirements { get; set; }

        /// <summary>
        /// Gets or sets the experience logic when applying filters
        /// </summary>
        [Column, Required, Default(0)]
        public LogicOperator ExperienceLogic { get; set; }

        /// <summary>
        /// Gets or sets the order in which this Billet will display in the Billet List View
        /// on the <see cref="UnitTypeManagerForm"/>, relative to the other billet ZIndexies.
        /// </summary>
        [Column, Required, Default(0)]
        public int ZIndex { get; set; } = 0;

        /// <summary>
        /// DEPRECIATED
        /// </summary>
        [Column, Required, Default(0)]
        public bool LateralOnly { get; set; }

        #endregion

        #region Virtual Foreign Keys

        /// <summary>
        /// Gets the <see cref="UnitTemplate"/> entity that this entity references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("UnitTypeId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<UnitTemplate> FK_Parent { get; set; }

        /// <summary>
        /// Gets the <see cref="BilletCatagory"/> entity that this entity references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("BilletCatagoryId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<BilletCatagory> FK_Catagory { get; set; }

        /// <summary>
        /// Gets the <see cref="Rank"/> entity that this entity references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("RankId",
            OnDelete = ReferentialIntegrity.Restrict,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<Rank> FK_Rank { get; set; }

        /// <summary>
        /// Gets the <see cref="Rank"/> entity that this entity references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("MaxRankId",
            OnDelete = ReferentialIntegrity.Restrict,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<Rank> FK_MaxRank { get; set; }

        [InverseKey("Id")]
        [ForeignKey("PromotionPoolId",
            OnDelete = ReferentialIntegrity.Restrict,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<Echelon> FK_PromotionPool { get; set; }

        #endregion

        #region Foreign Key Properties

        /// <summary>
        /// Gets or Sets the <see cref="Perscom.Database.UnitTemplate"/> that 
        /// this Billit is attached to.
        /// </summary>
        public UnitTemplate UnitType
        {
            get
            {
                return FK_Parent?.Fetch();
            }
            set
            {
                UnitTypeId = value.Id;
                FK_Parent?.Refresh();
            }
        }

        /// <summary>
        /// Gets or Sets the <see cref="Perscom.Database.BilletCatagory"/> that 
        /// this Billit falls under.
        /// </summary>
        public BilletCatagory Catagory
        {
            get
            {
                return FK_Catagory?.Fetch();
            }
            set
            {
                BilletCatagoryId = value.Id;
                FK_Catagory?.Refresh();
            }
        }

        /// <summary>
        /// Gets or Sets the <see cref="Perscom.Database.Rank"/> that 
        /// this position will hold.
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

        /// <summary>
        /// Gets or Sets the maximum <see cref="Perscom.Database.Rank"/> that 
        /// this position can hold.
        /// </summary>
        public Rank MaxRank
        {
            get
            {
                return FK_MaxRank?.Fetch();
            }
            set
            {
                MaxRankId = value.Id;
                FK_MaxRank?.Refresh();
            }
        }

        /// <summary>
        /// Gets or Sets the <see cref="Database.Echelon"/> unit level that 
        /// this billet will pull soldiers from to fill <see cref="Position"/>s
        /// </summary>
        public Echelon PromotionPool
        {
            get
            {
                return FK_PromotionPool?.Fetch();
            }
            set
            {
                PromotionPoolId = value.Id;
                FK_PromotionPool?.Refresh();
            }
        }

        #endregion

        #region Child Database Sets

        /// <summary>
        /// Gets a list of <see cref="Position"/> entities that reference this 
        /// <see cref="Billet"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual IEnumerable<Position> Positions { get; set; }

        /// <summary>
        /// Gets a list of <see cref="BilletSpecialtyRequirement"/> entities that reference this 
        /// <see cref="Billet"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual IEnumerable<BilletSpecialtyRequirement> Requirements { get; set; }

        /// <summary>
        /// Gets the <see cref="BilletSpawnSetting"/> entity that reference this 
        /// <see cref="Billet"/>, if any
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual IEnumerable<BilletSpawnSetting> SpawnSettings { get; set; }

        /// <summary>
        /// Gets the <see cref="BilletSpecialty"/> entity that reference this 
        /// <see cref="Billet"/>, if any
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual IEnumerable<BilletSpecialty> Specialties { get; set; }

        /// <summary>
        /// Gets a list of <see cref="BilletExperience"/> entities that reference this 
        /// <see cref="Billet"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual IEnumerable<BilletExperience> Experience { get; set; }

        /// <summary>
        /// Gets a list of <see cref="BilletExperienceSorting"/> entities that reference this 
        /// <see cref="Billet"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual IEnumerable<BilletExperienceSorting> Sorting { get; set; }

        /// <summary>
        /// Gets a list of <see cref="BilletExperienceGroup"/> entities that reference this 
        /// <see cref="Billet"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual IEnumerable<BilletExperienceGroup> Grouping { get; set; }

        /// <summary>
        /// Gets a list of <see cref="BilletExperienceFilter"/> entities that reference this 
        /// <see cref="Billet"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual IEnumerable<BilletExperienceFilter> Filters { get; set; }

        #endregion

        /// <summary>
        /// Compares a <see cref="Billet"/> with this one, and returns whether
        /// or not the RankId and Names match
        /// </summary>
        /// <remarks>Used in the <see cref="UnitTypeManagerForm"/></remarks>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsDuplicateOf(Billet other)
        {
            return (RankId == other.RankId && Name.Equals(other.Name, StringComparison.InvariantCultureIgnoreCase));
        }

        public bool Equals(Billet other)
        {
            if (other == null) return false;
            return (Id == other.Id);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Billet);
        }

        public override int GetHashCode() => Id;

        public override string ToString() => Name;
    }
}
