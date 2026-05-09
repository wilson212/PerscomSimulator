using CrossLite;
using CrossLite.CodeFirst;
using System;

namespace Perscom.Database
{
    /// <summary>
    /// Represents a soldier within the system. This entity contains core information
    /// about a soldier, including personal details, career data, and relational links to
    /// associated entities such as rank, occupation, and past assignments.
    /// </summary>
    /// <remarks>
    /// The Soldier class implements the IEquatable interface for equality comparison
    /// and inherits from EntityBase. It supports associations with other entities such
    /// as Persona, Rank, Occupation, and IterationDate through foreign key relationships.
    /// </remarks>
    [Table]
    public class Soldier : EntityBase, IEquatable<Soldier>
    {
        #region Columns

        /// <summary>
        /// The IsUnique Soldier ID
        /// </summary>
        [Column, PrimaryKey]
        public virtual int Id { get; protected set; }

        /// <summary>
        /// Gets or sets the first name of this <see cref="Soldier"/> entity
        /// </summary>
        [Column, Required]
        public virtual string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name of this <see cref="Soldier"/> entity
        /// </summary>
        [Column, Required]
        public virtual string LastName { get; set; }
        
        /// <summary>
        /// Gets or sets the <see cref="Persona.Id"/> this <see cref="Soldier"/>
        /// was promoted from
        /// </summary>
        [Column, Required]
        public virtual int PersonaId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Rank.Id"/> this <see cref="Soldier"/>
        /// was promoted from
        /// </summary>
        [Column, Required]
        public virtual int RankId { get; set; }

        /// <summary>
        /// Gets or sets the current <see cref="Occupation.Id"/> for this <see cref="Soldier"/>
        /// </summary>
        [Column, Required]
        public virtual int OccupationId { get; set; }

        /// <summary>
        /// Gets or sets the the <see cref="DateTime"/> the soldier was created in
        /// the <see cref="Simulator"/>
        /// </summary>
        [Column, Required]
        public virtual int EntryIterationId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Simulator.Iteration"/> that this soldier retired on,
        /// or null if the soldier is still active and not retired!
        /// </summary>
        [Column, Default(null)]
        public virtual int? ExitIterationId { get; set; }

        /// <summary>
        /// Gets the last promotion date for this soldier, or the Entry date if this soldier has never been promoted
        /// </summary>
        [Column, Required]
        public virtual int LastPromotionIterationId { get; set; }

        /// <summary>
        /// Gets or sets the last Rank Gade change date for this soldier, or the Entry date if this soldier has never been promoted
        /// </summary>
        [Column, Required]
        public virtual int LastGradeChangeIterationId { get; set; }
        
        /// <summary>
        /// Gets or sets the <see cref="CareerLength.Id"/>
        /// </summary>
        [Column, Required]
        public virtual int CareerLengthId { get; set; }

        /// <summary>
        /// Represents the target rank ID for the soldier.
        /// This property indicates the rank that the soldier is expected
        /// or aiming to achieve during the next step in their career progression.
        /// </summary>
        /// <remarks>
        /// Used to target a specific promotion board, since they are indexed by rank and occupation
        /// </remarks>
        [Column, Default(null)]
        public virtual int? TargetRankId { get; set; }

        /// <summary>
        /// Indicates whether the soldier is male.
        /// </summary>
        [Column, Required, Default(true)]
        public virtual bool IsMale { get; set; } = true;

        /// <summary>
        /// Gets or sets whether this soldier is retired
        /// </summary>
        [Column, Default(false)]
        public virtual bool Retired { get; set; }

        /// <summary>
        /// The age of the soldier in years, when they ENTERED the simulation.
        /// </summary>
        [Column, Required]
        public virtual int Age { get; set; }

        /// <summary>
        /// Represents the racial category of a soldier.
        /// </summary>
        [Column, Required]
        public virtual Race Race { get; set; }

        /// <summary>
        /// Represents the soldier's ego level, which influences their perception of their abilities and decisions.
        /// </summary>
        [Column, Required]
        public virtual int Ego { get; set; }
        
        /// <summary>
        /// Represents the target Time-In-Service (TIS) duration, measured in iterations,
        /// associated with the soldier's career progression. Things such as Morale and
        /// Flight Risk can slightly affect how long a soldier will stay in service.
        /// </summary>
        [Column, Required, Default(0)]
        public virtual int TargetTIS { get; set; }
        
        /// <summary>
        /// Represents the current morale level of the soldier, expressed as an integer value.
        /// Defaults to 70 if not explicitly set.
        /// </summary>
        [Column, Required, Default(70)]
        public virtual int Morale { get; set; } = 70;

        /// <summary>
        /// Represents the soldier's level of burnout, measured as a numerical value.
        /// </summary>
        /// <remarks>
        /// This property is influenced by factors such as time in service, morale,
        /// financial strain, and inherent traits. The value is clamped between 0 and 100,
        /// where higher values indicate greater burnout.
        /// </remarks>
        [Column, Required, Default(0)]
        public virtual int Burnout { get; set; } = 0;

        #endregion

        #region Foreign Key Navigation Properties

        /// <summary>
        /// Represents the Persona entity in the database, which created this character
        /// within the context of the application.
        /// </summary>
        [ForeignKey(nameof(PersonaId))]
        [References(nameof(Database.Persona.Id),
            OnDelete = ReferentialAction.Restrict,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual Persona Persona { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Perscom.Database.Rank"/> that 
        /// this soldier holds.
        /// </summary>
        [ForeignKey(nameof(RankId))]
        [References(nameof(Database.Rank.Id),
            OnDelete = ReferentialAction.Restrict,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual Rank Rank { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Perscom.Database.Specialty"/> that 
        /// this soldier holds.
        /// </summary>
        [ForeignKey(nameof(OccupationId))]
        [References(nameof(Database.Occupation.Id),
            OnDelete = ReferentialAction.Restrict,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual Occupation Occupation { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IterationDate"/> that this <see cref="Soldier"/> 
        /// was created during the simulation
        /// </summary>
        [ForeignKey(nameof(EntryIterationId))]
        [References(nameof(Database.IterationDate.Id),
            OnDelete = ReferentialAction.Restrict,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual IterationDate EntryServiceDate { get; set; }
        
        /// <summary>
        /// Gets or sets the <see cref="IterationDate"/> that this <see cref="Soldier"/> 
        /// exited or retired from the simulation.
        /// </summary>
        [ForeignKey(nameof(ExitIterationId))]
        [References(nameof(Database.IterationDate.Id),
            OnDelete = ReferentialAction.Restrict,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual IterationDate ExitServiceDate { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IterationDate"/> that this <see cref="Soldier"/> 
        /// earned his last promotion.
        /// </summary>
        [ForeignKey(nameof(LastPromotionIterationId))]
        [References(nameof(Database.IterationDate.Id),
            OnDelete = ReferentialAction.Restrict,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual IterationDate LastPromotionDate { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IterationDate"/> that this <see cref="Soldier"/> 
        /// earned his last PayGrade change.
        /// </summary>
        [ForeignKey(nameof(LastGradeChangeIterationId))]
        [References(nameof(Database.IterationDate.Id),
            OnDelete = ReferentialAction.Restrict,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual IterationDate LastGradeChangeDate { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="CareerLength"/> that 
        /// this soldier is created with.
        /// </summary>
        [ForeignKey(nameof(CareerLengthId))]
        [References(nameof(Database.CareerLength.Id),
            OnDelete = ReferentialAction.Restrict,
            OnUpdate = ReferentialAction.Cascade
        )]
        public virtual CareerLength CareerLength { get; set; }

        #endregion

        #region Child Navigation Properties

        /// <summary>
        /// Gets a list of current <see cref="Assignment"/> entities that reference this 
        /// <see cref="Soldier"/>
        /// </summary>
        public virtual EntitySet<Assignment> Assignments { get; set; }
        
        /// <summary>
        /// A collection of attributes associated with a soldier. Each attribute represents
        /// specific qualities or properties linked to the soldier's characteristics. These values
        /// are NOT affected by traits or other modifiers.
        /// </summary>
        public virtual EntitySet<SoldierAttribute> Attributes { get; set; }

        /// <summary>
        /// Gets a list of <see cref="PastAssignment"/> entities that reference this 
        /// <see cref="Soldier"/>
        /// </summary>
        public virtual EntitySet<PastAssignment> PastAssignments { get; set; }

        /// <summary>
        /// Represents the collection of promotion board results associated with the soldier.
        /// </summary>
        public virtual EntitySet<PromotionBoardResult> PromotionBoardResults { get; set; }

        /// <summary>
        /// Gets a list of current <see cref="OccupationAssignment"/> entities that reference this 
        /// <see cref="Soldier"/>
        /// </summary>
        public virtual EntitySet<OccupationAssignment> SpecialtyAssignments { get; set; }

        /// <summary>
        /// Gets a list of <see cref="Promotion"/> entities that reference this 
        /// <see cref="Soldier"/>
        /// </summary>
        public virtual EntitySet<Promotion> Promotions { get; set; }

        /// <summary>
        /// Gets a list of <see cref="SoldierExperience"/> entities that reference this 
        /// <see cref="Soldier"/>
        /// </summary>
        public virtual EntitySet<SoldierExperience> Experience { get; set; }

        /// <summary>
        /// A collection of monthly records associated with the soldier.
        /// </summary>
        public virtual EntitySet<SoldierMonthlyRecord> MonthlyRecords { get; set; }

        /// <summary>
        /// A collection of traits associated with the soldier, defining additional attributes or characteristics.
        /// </summary>
        public virtual EntitySet<SoldierTraitAttachment> Traits { get; set; }
        
        #endregion
        
        public static bool operator ==(Soldier left, Soldier right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(Soldier left, Soldier right)
        {
            return !(left == right);
        }

        public bool Equals(Soldier other)
        {
            if (other is null) return false;
            return (Id == other.Id);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Soldier);
        }

        public override int GetHashCode() => Id.GetHashCode();

        public override string ToString() => $"{FirstName} {LastName}";
    }
}
