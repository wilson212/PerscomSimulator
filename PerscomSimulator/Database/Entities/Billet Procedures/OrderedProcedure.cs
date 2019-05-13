using CrossLite;
using CrossLite.CodeFirst;
using System.Collections.Generic;

namespace Perscom.Database
{
    [Table]
    public class OrderedProcedure
    {
        #region Columns

        /// <summary>
        /// The Unique OrderedProcedure ID (Row ID)
        /// </summary>
        [Column, PrimaryKey]
        public int Id { get; protected set; }

        /// <summary>
        /// Gets or Sets the string name of this Generator
        /// </summary>
        [Column, Required, Unique, Collation(Collation.NoCase)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="Rank.Id"/> the <see cref="Soldier"/>
        /// holding this billet should be.
        /// </summary>
        [Column, Required]
        public int RankId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column, Required, Default(0)]
        public bool CreatesNewSoldiers { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        [Column, Default(0)]
        public int NewSoldierProbability { get; set; } = 0;

        #endregion

        #region Virtual Foreign Keys

        /// <summary>
        /// Gets the <see cref="Database.Rank"/> entity that this entity references.
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
        /// Gets or Sets the <see cref="Database.Rank"/> that 
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
        /// Gets a list of <see cref="OrderedPool"/> entities that reference this 
        /// <see cref="OrderedProcedure"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual IEnumerable<OrderedPool> ProcedurePools { get; set; }

        // <summary>
        /// Gets a list of <see cref="OrderedProcedureCareer"/> entities that reference this 
        /// <see cref="RandomizedProcedure"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual IEnumerable<OrderedProcedureCareer> NewSoldierCareer { get; set; }

        /// <summary>
        /// Gets a list of <see cref="BilletOrderedProcedure"/> entities that reference this 
        /// <see cref="RandomizedProcedure"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual IEnumerable<BilletOrderedProcedure> Billets { get; set; }

        #endregion
    }
}
