using CrossLite;
using CrossLite.CodeFirst;
using CrossLite.QueryBuilder;
using Perscom.Simulation;
using System;
using System.Collections.Generic;

namespace Perscom.Database
{
    /// <summary>
    /// Represents an abstract set of rules that child <see cref="Position"/>'s will inherit
    /// </summary>
    [Table]
    public class PositionBlueprint : EntityBase, IEquatable<PositionBlueprint>
    {
        #region Columns

        /// <summary>
        /// The IsUnique Billet ID
        /// </summary>
        [Column, PrimaryKey]
        public virtual int Id { get; protected set; }

        /// <summary>
        /// Gets or Sets the string name of this Unit
        /// </summary>
        [Column, Required]
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="Database.UnitBlueprint"/> object
        /// ID that this entity references
        /// </summary>
        [Column, Required]
        public virtual int UnitBlueprintId { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="PositionCatagory"/> object
        /// ID that this entity references
        /// </summary>
        [Column, Required]
        public virtual int CatagoryId { get; set; }
        
        /// <summary>
        /// Gets or sets the optional ID of the <see cref="PositionBlueprint"/> that serves
        /// as the direct supervisor for soldiers holding this position. If null, the leader
        /// quality morale check is skipped for this billet.
        /// </summary>
        [Column, Default(null)]
        public virtual int? SupervisorPositionBlueprintId { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="Rank.Id"/> the <see cref="Soldier"/>
        /// holding this billet should be.
        /// </summary>
        [Column, Required]
        public virtual int TargetRankId { get; set; }
        
        /// <summary>
        /// Gets or Sets the <see cref="Rank.Id"/> the <see cref="Soldier"/>
        /// holding this billet will be set to while holding this position, if any.
        /// </summary>
        [Column, Default(null)]
        public virtual int? PositionalRankId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="PositionFlag"/> of this Billet
        /// </summary>
        [Column, Required]
        public virtual PositionFlag Flag { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Echelon"/> level in which we will find soldiers
        /// to fill this position
        /// </summary>
        [Column, Required]
        public virtual int PromotionEchelonId { get; set; }

        /// <summary>
        /// Represents the identifier for the occupation associated with a position.
        /// </summary>
        [Column, Required]
        public virtual int OccupationId { get; set; }
        
        /// <summary>
        /// Gets or sets the prominence level of the position. A higher value will cause
        /// this <see cref="PositionBlueprint"/> to be filled with more experienced (time in grade) 
        /// soldiers, while lower values will be filled with more inexperienced soldiers.
        /// </summary>
        [Column, Required, Default(0)]
        public virtual int Stature { get; set; } = 0;

        /// <summary>
        /// Represents the prestige level (or "cool" factor) of the position and is used to determine the
        /// relative competitiveness of applicants for the position in the overall simulation.
        /// </summary>
        [Column, Required, Default(50)]
        public virtual int Prestige { get; set; } = 50;

        /// <summary>
        /// Gets or sets the minimum time (months) a soldier must hold this <see cref="PositionBlueprint"/> 
        /// before being allowed to leave. If the minimum amount is less than the remaining time 
        /// to live for the soldier, their retirement date will be adjusted accordingly.
        /// </summary>
        [Column, Required, Default(0)]
        public virtual int MinTourLength { get; set; } = 0;

        /// <summary>
        /// Gets or sets the maximum time (months) a soldier can hold this <see cref="PositionBlueprint"/>
        /// before being forcefully pushed out.
        /// </summary>
        [Column, Required, Default(0)]
        public virtual int MaxTourLength { get; set; } = 0;

        /// <summary>
        /// Indicates whether the soldier holding this <see cref="PositionBlueprint"/> can retire before
        /// meeting the <see cref="MinTourLength"/>.
        /// </summary>
        [Column, Required, Default(1)]
        public virtual bool CanRetireEarly { get; set; } = true;

        /// <summary>
        /// Indicates whether the soldier holding this <see cref="PositionBlueprint"/> can be selected
        /// for promotion before meeting the <see cref="MinTourLength"/>.
        /// </summary>
        [Column, Required, Default(1)]
        public virtual bool CanBePromotedEarly { get; set; } = true;

        /// <summary>
        /// Indicates whether the soldier holding this <see cref="PositionBlueprint"/> can be selected
        /// for a lateral promotion before meeting the <see cref="MinTourLength"/>.
        /// </summary>
        [Column, Required, Default(0)]
        public virtual bool CanLateralEarly { get; set; } = false;

        /// <summary>
        /// Indicates whether the soldier holding this <see cref="PositionBlueprint"/> is limited to
        /// the <see cref="MaxTourLength"/>, or can extend past this
        /// </summary>
        [Column, Required, Default(1)]
        public virtual bool Waiverable { get; set; } = true;

        /// <summary>
        /// Gets or sets the selection process the <see cref="Simulator"/>
        /// will use to fill this position when it is empty
        /// </summary>
        [Column, Required, Default(0)]
        public virtual SelectionProcedure SelectionMethod { get; set; } = SelectionProcedure.PromotionOrLateral;

        /// <summary>
        /// Indicates whether the soldier holding this <see cref="PositionBlueprint"/> will be
        /// demoted in <see cref="Database.Rank.PayGrade"/> if their current grade
        /// is higher than the <see cref="MaxRank.Grade"/>. This option is only
        /// used if the <see cref="SelectionMethod"/> equals <see cref="SelectionProcedure.EvaluationBoard"/>
        /// </summary>
        [Column, Required, Default(0)]
        public virtual bool DemoteOverRanked { get; set; } = false;

        /// <summary>
        /// Indicates whether the soldier holding this <see cref="PositionBlueprint"/> will be
        /// automatically promoted if thier current <see cref="RankClassification.PayGrade"/> is lower than
        /// the <see cref="RankClassification.PayGrade"/> of the <see cref="TargetRank"/>
        /// </summary>
        [Column, Required, Default(0)]
        public virtual bool AutoPromoteInRankRange { get; set; } = false;

        /// <summary>
        /// Indicates whether the required <see cref="Specialty"/> are inversed, meaning
        /// that the <see cref="Soldier"/> must NOT have the <see cref="Specialty"/> listed
        /// to be considered for this billet
        /// </summary>
        [Column, Required, Default(0)]
        public virtual bool InverseSpecialtyRequirements { get; set; }

        /// <summary>
        /// Gets or sets the experience logic when applying filters
        /// </summary>
        [Column, Required, Default(0)]
        public virtual LogicOperator ExperienceLogic { get; set; }

        /// <summary>
        /// Gets or sets the order in which this Blueprint will display in the Position List View
        /// on the <see cref="UnitBlueprintEditor"/>, relative to the other position ZIndexies.
        /// </summary>
        [Column, Required, Default(0)]
        public virtual int ZIndex { get; set; } = 0;

        #endregion

        #region Foreign Key Navigation Properties

        /// <summary>
        /// Gets or Sets the <see cref="Database.UnitBlueprint"/> that 
        /// this Billit is attached to.
        /// </summary>
        [ForeignKey(nameof(UnitBlueprintId))]
        [References(nameof(Database.PositionBlueprint.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual UnitBlueprint UnitBlueprint { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="Database.PositionCatagory"/> that 
        /// this Billit falls under.
        /// </summary>
        [ForeignKey(nameof(CatagoryId))]
        [References(nameof(Database.PositionCatagory.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual PositionCatagory Catagory { get; set; }

        /// <summary>
        /// Represents a reference to another PositionBlueprint entity
        /// that serves as the supervising position blueprint.
        /// This property establishes a self-referencing foreign key relationship
        /// within the PositionBlueprint table.
        /// </summary>
        [ForeignKey(nameof(SupervisorPositionBlueprintId))]
        [References(nameof(Database.PositionBlueprint.Id),
            OnDelete = ReferentialAction.SetNull,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual PositionBlueprint SupervisorPositionBlueprint { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="Database.Rank"/> that 
        /// this position will hold.
        /// </summary>
        [ForeignKey(nameof(TargetRankId))]
        [References(nameof(Database.Rank.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual Rank TargetRank { get; set; }
        
        /// <summary>
        /// Gets or Sets the <see cref="Database.Rank"/> that 
        /// this position will hold.
        /// </summary>
        [ForeignKey(nameof(PositionalRankId))]
        [References(nameof(Database.Rank.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual Rank PositionalRank { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="Database.Echelon"/> unit level that 
        /// this billet will pull soldiers from to fill <see cref="Position"/>s
        /// </summary>
        [ForeignKey(nameof(PromotionEchelonId))]
        [References(nameof(Database.Echelon.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual Echelon PromotionPool { get; set; }

        /// <summary>
        /// Represents the occupation associated with the position blueprint,
        /// defining the specific role or profession required for the position.
        /// </summary>
        [ForeignKey(nameof(OccupationId))]
        [References(nameof(Database.Occupation.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual Occupation Occupation { get; set; }

        #endregion

        #region Child Navigation Properties

        /// <summary>
        /// Gets a list of <see cref="Position"/> entities that reference this 
        /// <see cref="PositionBlueprint"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual EntitySet<Position> Positions { get; set; }

        /// <summary>
        /// Gets a list of <see cref="PositionOccupationRequirement"/> entities that reference this 
        /// <see cref="PositionBlueprint"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual EntitySet<PositionOccupationRequirement> Requirements { get; set; }

        /// <summary>
        /// Gets a list of <see cref="PositionBlueprintExperience"/> entities that reference this 
        /// <see cref="PositionBlueprint"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual EntitySet<PositionBlueprintExperience> Experience { get; set; }

        /// <summary>
        /// Gets a list of <see cref="SelectionSorting"/> entities that reference this 
        /// <see cref="PositionBlueprint"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual EntitySet<SelectionSorting> Sorting { get; set; }

        /// <summary>
        /// Gets a list of <see cref="SelectionGroup"/> entities that reference this 
        /// <see cref="PositionBlueprint"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual EntitySet<SelectionGroup> Grouping { get; set; }

        /// <summary>
        /// Gets a list of <see cref="SelectionFilter"/> entities that reference this 
        /// <see cref="PositionBlueprint"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual EntitySet<SelectionFilter> Filters { get; set; }
        
        /// <summary>
        /// A dictionary associating specific attribute types with their performance values.
        /// A <see cref="Soldier"/>'s performance is determined by comparing thier attributes
        /// to that of the <see cref="PositionBlueprint.PerformanceModels"/>
        /// </summary>
        public virtual EntitySet<PositionPerformanceModel> PerformanceModels { get; set; }
 
        #endregion

        /// <summary>
        /// Compares a <see cref="PositionBlueprint"/> with this one, and returns whether
        /// or not the RankId and Names match
        /// </summary>
        /// <remarks>Used in the <see cref="UnitTypeManagerForm"/></remarks>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsDuplicateOf(PositionBlueprint other)
        {
            return (TargetRankId == other.TargetRankId && Name.Equals(other.Name, StringComparison.InvariantCultureIgnoreCase));
        }

        public bool Equals(PositionBlueprint other)
        {
            if (other == null) return false;
            return (Id == other.Id);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as PositionBlueprint);
        }

        public override int GetHashCode() => Id;

        public override string ToString() => Name;
    }
}
