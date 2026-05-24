using CrossLite;
using CrossLite.CodeFirst;
using Perscom.Simulation;

namespace Perscom.Database
{
    /// <summary>
    /// Represents a custom selection procedure when a <see cref="SelectionProcedure"/> is set
    /// to <see cref="SelectionProcedure.EvaluationBoard"/>
    /// </summary>
    [Table]
    public class EvaluationBoard : EntityBase
    {
        #region Columns

        /// <summary>
        /// The IsUnique ID (Row ID)
        /// </summary>
        [Column, PrimaryKey]
        public virtual int Id { get; protected set; }
        
        /// <summary>
        /// The Faction ID that this entity references
        /// </summary>
        [Column, Required]
        public virtual int FactionId { get; set; }

        /// <summary>
        /// Gets or sets the string name of this <see cref="SelectionProcedure"/>
        /// </summary>
        [Column, Required]
        public virtual string Name { get; set; }
        

        /// <summary>
        /// 
        /// </summary>
        [Column, Required, Default(0)]
        public virtual PoolSelection PoolSelection { get; set; } = PoolSelection.Collective;

        #endregion
        
        #region Foreign Key Navigation Properties

        /// <summary>
        /// Gets the <see cref="Database.Faction"/> entity that this entity references.
        /// </summary>
        [ForeignKey(nameof(FactionId))]
        [References(nameof(Database.Faction.Id),
            OnDelete = ReferentialAction.Cascade,
            OnUpdate = ReferentialAction.Cascade
        )]
        protected virtual Faction Faction { get; set; }

        #endregion
        
        #region Child Navigation Properties
        
        /// <summary>
        /// Contains the list of scores for this evaluation board
        /// </summary>
        public virtual EntitySet<EvaluationBoardScore> Scores { get; set; }
        
        /// <summary>
        /// Gets a list of <see cref="SelectionSorting"/> entities that reference this entity
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual EntitySet<SelectionSorting> Sorting { get; set; }

        /// <summary>
        /// Gets a list of <see cref="SelectionGroup"/> entities that reference this entity
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual EntitySet<SelectionGroup> Grouping { get; set; }

        /// <summary>
        /// Gets a list of <see cref="SelectionFilter"/> entities that reference this entity
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration
        /// </remarks>
        public virtual EntitySet<SelectionFilter> Filters { get; set; }
        
        #endregion
    }
}
