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
    public class CareerLengthRange : ISpawnable, IEquatable<CareerLengthRange>
    {
        #region Columns

        /// <summary>
        /// The Unique SpawnRate ID (Row ID)
        /// </summary>
        [Column, PrimaryKey]
        public int Id { get; protected set; }

        /// <summary>
        /// The Unique SpawnRate ID (Row ID)
        /// </summary>
        [Column, Required]
        public int GeneratorId { get; protected set; }

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

        #region Virtual Foreign Keys

        /// <summary>
        /// Gets the <see cref="SoldierGenerator"/> entity that this entity references.
        /// </summary>
        [InverseKey("Id")]
        [ForeignKey("GeneratorId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<CareerGenerator> FK_Parent { get; set; }

        #endregion

        #region Foreign Key Properties

        /// <summary>
        /// Gets or Sets the <see cref="Perscom.Database.CareerGenerator"/> that 
        /// this Billit is attached to.
        /// </summary>
        public CareerGenerator Generator
        {
            get
            {
                return FK_Parent?.Fetch();
            }
            set
            {
                GeneratorId = value.Id;
                FK_Parent?.Refresh();
            }
        }

        #endregion

        /// <summary>
        /// Generates and returns a random career length in months,
        /// between the <see cref="MinCareerLength"/> and <see cref="MaxCareerLength"/>
        /// </summary>
        /// <returns></returns>
        public int GenerateLength()
        {
            CryptoRandom r = new CryptoRandom();
            return r.Next(MinCareerLength, MaxCareerLength);
        }

        /// <summary>
        /// Compares a <see cref="CareerSpawnRate"/> with this one, and returns whether
        /// or not the <see cref="Probability"/>, <see cref="MinCareerLength"/>, and 
        /// <see cref="MaxCareerLength"/> match.
        /// </summary>
        /// <remarks>Used in the <see cref="CareerGeneratorEditorForm"/></remarks>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsDuplicateOf(CareerLengthRange other)
        {
            return (
                Probability == other.Probability && 
                MinCareerLength == other.MinCareerLength && 
                MaxCareerLength == other.MaxCareerLength
            );
        }

        public string StartString()
        {
            // Generate the time period
            LocalDate start = new LocalDate(2000, 1, 1);
            LocalDate end = start.PlusMonths(MinCareerLength);
            Period period = Period.Between(start, end, PeriodUnits.Years | PeriodUnits.Months);

            // Build formated string
            StringBuilder message = new StringBuilder();
            message.AppendIf(period.Years > 0, $"{period.Years} year(s) ");
            message.AppendIf(period.Months > 0, $"{period.Months} month(s) ");
            return message.ToString();
        }

        public string EndString()
        {
            // Generate the time period
            LocalDate start = new LocalDate(2000, 1, 1);
            LocalDate end = start.PlusMonths(MaxCareerLength);
            Period period = Period.Between(start, end, PeriodUnits.Years | PeriodUnits.Months);

            // Build formated string
            StringBuilder message = new StringBuilder();
            message.AppendIf(period.Years > 0, $"{period.Years} year(s) ");
            message.AppendIf(period.Months > 0, $"{period.Months} month(s) ");
            return message.ToString();
        }

        public bool Equals(CareerLengthRange other)
        {
            if (other == null) return false;
            return (Id == other.Id);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as CareerLengthRange);
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

            StringBuilder message = new StringBuilder();
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
