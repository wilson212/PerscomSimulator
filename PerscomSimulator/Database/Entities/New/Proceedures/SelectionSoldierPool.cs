using CrossLite;
using CrossLite.CodeFirst;
using Perscom.Simulation;

namespace Perscom.Database
{
    /// <summary>
    /// Represents a pool of soldiers associated with a specific selection procedure.
    /// This class provides a mechanism for defining the rank and probability of
    /// soldiers being part of the selection process.
    /// </summary>
    [Table]
    [CompositeUnique(nameof(SelectionProceedureId), nameof(RankId))]
    public class SelectionSoldierPool : EntityBase, IProbable
    {
        /// <summary>
        /// The IsUnique ID (Row ID)
        /// </summary>
        [Column, Required, PrimaryKey]
        public virtual int Id { get; set; }

        /// <summary>
        /// Represents the unique identifier for the selection procedure associated
        /// with a soldier pool.
        /// </summary>
        [Column, Required]
        public virtual int SelectionProceedureId { get; set; }

        /// <summary>
        /// Represents the unique identifier for the rank associated with a soldier pool.
        /// </summary>
        [Column, Required]
        public virtual int RankId { get; set; }

        /// <summary>
        /// Represents the probability of a soldier being selected from this pool.
        /// </summary>
        [Column, Required, Default(1)] 
        public virtual int Probability { get; set; } = 1;
    }
}
