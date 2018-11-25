using System;
using CrossLite;
using CrossLite.CodeFirst;

namespace Perscom.Database
{
    /// <summary>
    /// Maps a Simulation Iteration to a Date String
    /// </summary>
    /// <remarks>
    /// Since the simulator begins on a past date, sometimes hundreds of years
    /// proir to todays date, we cannot use Epoch timestamps for dates. 
    /// Add the fact that SQLite cannot compare dates very quickly, 
    /// we use this object to map a iteration ID to a DateTime.
    /// </remarks>
    [Table]
    public class IterationDate
    {
        /// <summary>
        /// The Unique Simulation Iteration ID
        /// </summary>
        [Column, PrimaryKey, AutoIncrement]
        public int Id { get; protected set; }

        /// <summary>
        /// Gets or sets the <see cref="DateTime"/> for this Simulation Iteration
        /// </summary>
        [Column, Required]
        public DateTime Date { get; set; }

        public int MonthsDifference(IterationDate date)
        {
            return Math.Abs(this.Id - date.Id);
        }
    }
}
