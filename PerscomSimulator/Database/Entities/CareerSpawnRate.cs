using System;
using System.Text;
using CrossLite;
using CrossLite.CodeFirst;
using NodaTime;
using Perscom.Simulation;

namespace Perscom.Database
{
    /// <summary>
    /// Defines a Spawnable entity that determines the minimum and maximum
    /// career lengths for a <see cref="Database.Soldier"/> entity.
    /// </summary>
    [Table]
    public class CareerSpawnRate : ISpawnable, IEquatable<CareerSpawnRate>
    {
        #region Columns

        /// <summary>
        /// The Unique SpawnRate ID (Row ID)
        /// </summary>
        [Column, PrimaryKey]
        public int Id { get; protected set; }

        /// <summary>
        /// Gets or sets the <see cref="RankType"/> of this SpawnRate
        /// </summary>
        [Column, Required]
        public RankType Type { get; set; }

        /// <summary>
        /// Gets the probability value of this <see cref="CareerSpawnRate"/>
        /// spawning in the <see cref="SpawnGenerator{T}"/> relative to the
        /// other <see cref="CareerSpawnRate"/> entities.
        /// </summary>
        [Column, Required]
        public int Probability { get; set; }

        /// <summary>
        /// Gets or sets the minimum time (months) a soldier will live using this
        /// <see cref="CareerSpawnRate"/>
        /// </summary>
        [Column, Required]
        public int MinCareerLength { get; set; } = 0;

        /// <summary>
        /// Gets or sets the maximum time (months) a soldier will live using this
        /// <see cref="CareerSpawnRate"/>
        /// </summary>
        [Column, Required]
        public int MaxCareerLength { get; set; } = 0;

        #endregion

        public bool Equals(CareerSpawnRate other)
        {
            if (other == null) return false;
            return (Id == other.Id);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as CareerSpawnRate);
        }

        public override int GetHashCode() => Id;

        public override string ToString()
        {
            // Get years
            int period = MaxCareerLength - MinCareerLength;
            LocalDate start = new LocalDate(2000, 1, 1);
            LocalDate end = start.PlusMonths(MinCareerLength);
            Period startPeriod = Period.Between(start, end, PeriodUnits.Years | PeriodUnits.Months);
            end = start.PlusMonths(MaxCareerLength);
            Period endPeriod = Period.Between(start, end, PeriodUnits.Years | PeriodUnits.Months);

            StringBuilder message = new StringBuilder($"{Probability} :: ");
            if (startPeriod.Years > 0)
                message.Append($"{startPeriod.Years} yrs ");

            if (startPeriod.Months > 0)
                message.Append($"{startPeriod.Months}m ");

            message.Append(" through ");

            if (endPeriod.Years > 0)
                message.Append($"{endPeriod.Years} yrs");

            if (endPeriod.Months > 0)
                message.Append($" {endPeriod.Months}m");


            return message.ToString();
        }
    }
}
