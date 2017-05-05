using System;

namespace Perscom.Simulation
{
    /// <summary>
    /// An object representing a timespan that a <see cref="Soldier"/> spent
    /// in a particular <see cref="UnitPosition"/>
    /// </summary>
    public class Billet
    {
        /// <summary>
        /// The position
        /// </summary>
        public UnitPosition Position { get; set; }

        /// <summary>
        /// The date when the soldier was assigned to the position
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The date when the position when the soldier was removed from the position
        /// </summary>
        public DateTime EndDate { get; set; }

        public Billet(UnitPosition position, DateTime currentDate)
        {
            Position = position;
            StartDate = currentDate;
        }
    }
}
