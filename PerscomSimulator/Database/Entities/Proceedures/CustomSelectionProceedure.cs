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
    public class CustomSelectionProceedure : EntityBase
    {
        #region Columns

        /// <summary>
        /// The IsUnique ID (Row ID)
        /// </summary>
        [Column, PrimaryKey]
        public virtual int Id { get; protected set; }

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
    }
}
