using CrossLite;
using CrossLite.CodeFirst;
using Perscom.Simulation;

namespace Perscom.Database.Entities
{
    [Table]
    public class RankGrade
    {
        /// <summary>
        /// Gets or sets the grade of this rank
        /// </summary>
        [Column, PrimaryKey]
        public RankType Type { get; set; }

        /// <summary>
        /// The Unique Rank ID
        /// </summary>
        [Column, PrimaryKey]
        public int Grade { get; set; }

        /// <summary>
        /// Gets or sets the minimum time (months) a soldier must hold this grade before 
        /// retiring. If the minimum amount is less than the remaining time to live for
        /// the soldier, their retirement date will be adjusted accordingly.
        /// </summary>
        [Column, Default(0)]
        public int MinTimeInGrade { get; set; } = 0;

        /// <summary>
        /// Gets or sets the maximum time (months) a soldier can hold this grade before being
        /// forcefully retired. Retirement date is otherwise un-modified.
        /// </summary>
        [Column, Default(0)]
        public int MaxTimeInGrade { get; set; } = 0;

        /// <summary>
        /// Gets or sets the time in grade (months) to be promotable to the next grade.
        /// </summary>
        [Column, Required, Default(12)]
        public int PromotableAt { get; set; } = 12;

        /// <summary>
        /// Indicates whether the soldiers hodling this Rank will be
        /// promoted to the next rank when they reach the promotableAt value.
        /// </summary>
        [Column, Required, Default(0)]
        public bool AutoPromote { get; set; } = false;
    }
}
