using System.Collections.Generic;
using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    /// <summary>
    /// This entity represents a spawned soldier
    /// </summary>
    [Table]
    public class Soldier
    {
        #region Columns

        /// <summary>
        /// The Unique Soldier ID
        /// </summary>
        [Column, PrimaryKey]
        public int Id { get; protected set; }

        /// <summary>
        /// Gets or sets the first name of this <see cref="Soldier"/> entity
        /// </summary>
        [Column, Required]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name of this <see cref="Soldier"/> entity
        /// </summary>
        [Column, Required]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Rank.Id"/> this <see cref="Soldier"/>
        /// was promoted from
        /// </summary>
        [Column, Required]
        public int RankId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Specialty.Id"/> for this <see cref="Soldier"/>
        /// </summary>
        [Column, Required]
        public int SpecialtyId { get; set; }

        /// <summary>
        /// Gets or sets the the <see cref="DateTime"/> the soldier was created in
        /// the <see cref="Simulator"/>
        /// </summary>
        [Column, Required]
        public int EntryIterationId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Simulator.Iteration"/> that this soldier will retire on
        /// </summary>
        [Column, Required]
        public int ExitIterationId { get; set; }

        /// <summary>
        /// Gets the last promotion date for this soldier
        /// </summary>
        [Column, Required]
        public int LastPromotionIterationId { get; set; }

        /// <summary>
        /// Gets or sets the last Rank Gade change date for this soldier
        /// </summary>
        [Column, Required, Default(0)]
        public int LastGradeChangeIterationId { get; set; }

        /// <summary>
        /// Gets or sets whether this soldier is retired
        /// </summary>
        [Column, Default(false)]
        public bool Retired { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="CareerGenerator.Id"/>
        /// </summary>
        [Column, Required]
        public int SpawnRateId { get; set; }

        #endregion

        #region Foreign Keys

        /// <summary>
        /// Gets the <see cref="Database.Rank"/> entity that this entity references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("RankId",
            OnDelete = ReferentialIntegrity.Restrict,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<Rank> FK_Rank { get; set; }

        /// <summary>
        /// Gets the <see cref="Database.Specialty"/> entity that this entity references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("SpecialtyId",
            OnDelete = ReferentialIntegrity.Restrict,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<Specialty> FK_Specialty { get; set; }

        /// <summary>
        /// Gets the <see cref=IterationDate"/> entity that this entity references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("EntryIterationId",
            OnDelete = ReferentialIntegrity.Restrict,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<IterationDate> FK_Entry { get; set; }

        /// <summary>
        /// Gets the <see cref=IterationDate"/> entity that this entity references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("LastPromotionIterationId",
            OnDelete = ReferentialIntegrity.Restrict,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<IterationDate> FK_Promotion { get; set; }

        /// <summary>
        /// Gets the <see cref=IterationDate"/> entity that this entity references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("LastGradeChangeIterationId",
            OnDelete = ReferentialIntegrity.Restrict,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<IterationDate> FK_Grade { get; set; }


        /// <summary>
        /// Gets the <see cref="CareerSpawnRate"/> entity that this entity references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("SpawnRateId",
            OnDelete = ReferentialIntegrity.Restrict,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<CareerLengthRange> FK_SpawnRate { get; set; }

        #endregion

        #region Foreign Key Properties

        /// <summary>
        /// Gets or sets the <see cref="Perscom.Database.Rank"/> that 
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
        /// Gets or sets the <see cref="Perscom.Database.Specialty"/> that 
        /// this soldier holds.
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

        /// <summary>
        /// Gets or sets the <see cref="IterationDate"/> that this <see cref="Soldier"/> 
        /// was created during the simulation
        /// </summary>
        public IterationDate EntryServiceDate
        {
            get
            {
                return FK_Entry?.Fetch();
            }
            set
            {
                EntryIterationId = value.Id;
                FK_Entry?.Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="IterationDate"/> that this <see cref="Soldier"/> 
        /// earned his last <see cref="Promotion"/>
        /// </summary>
        public IterationDate LastPromotionDate
        {
            get
            {
                return FK_Promotion?.Fetch();
            }
            set
            {
                LastPromotionIterationId = value.Id;
                FK_Promotion?.Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="IterationDate"/> that this <see cref="Soldier"/> 
        /// earned his last <see cref="Promotion"/> that was a Grade change
        /// </summary>
        public IterationDate LastGradeChangeDate
        {
            get
            {
                return FK_Grade?.Fetch();
            }
            set
            {
                LastGradeChangeIterationId = value.Id;
                FK_Grade?.Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="CareerSpawnRate"/> that 
        /// this soldier is created with
        /// </summary>
        public CareerLengthRange SpawnRate
        {
            get
            {
                return FK_SpawnRate?.Fetch();
            }
            set
            {
                SpawnRateId = value.Id;
                FK_SpawnRate?.Refresh();
            }
        }

        #endregion

        #region Child Database Sets

        /// <summary>
        /// Gets a list of current <see cref="Assignment"/> entities that reference this 
        /// <see cref="Soldier"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual IEnumerable<Assignment> Assignments { get; set; }

        /// <summary>
        /// Gets a list of <see cref="PastAssignment"/> entities that reference this 
        /// <see cref="Soldier"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual IEnumerable<PastAssignment> PastAssignments { get; set; }

        /// <summary>
        /// Gets a list of current <see cref="SpecialtyAssignment"/> entities that reference this 
        /// <see cref="Soldier"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual IEnumerable<SpecialtyAssignment> SpecialtyAssignments { get; set; }

        /// <summary>
        /// Gets a list of <see cref="Promotion"/> entities that reference this 
        /// <see cref="Soldier"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual IEnumerable<Promotion> Promotions { get; set; }

        #endregion
    }
}
