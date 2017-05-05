using System;

namespace Perscom.Simulation
{
    /// <summary>
    /// Represents a Billit, as part of a unit, that a <see cref="Soldier"/>
    /// will occupy while active.
    /// </summary>
    public class UnitPosition : ICloneable
    {
        /// <summary>
        /// Gets or Sets the <see cref="Unit"/> That this position
        /// belongs to.
        /// </summary>
        public Unit ParentUnit { get; set; }

        /// <summary>
        /// Gets or Sets the name of this position
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="Soldier"/> who occupies this position currently
        /// </summary>
        public Soldier Holder { get; set; }

        /// <summary>
        /// Indicates whether this is an entry level position
        /// </summary>
        public bool EntryLevel { get; set; } = false;

        public RankType RankType { get; set; }

        public int Grade { get; set; }

        public object Clone()
        {
            var clone = (UnitPosition)this.MemberwiseClone();
            clone.ParentUnit = null;
            clone.Holder = null;
            return clone;
        }

        public override string ToString()
        {
            return (ParentUnit == null) ? Name : String.Concat(Name, ", ", ParentUnit.ToString());
        }
    }
}
