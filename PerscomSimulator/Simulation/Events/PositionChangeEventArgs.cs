using Perscom.Database;
using System;

namespace Perscom.Simulation
{
    /// <summary>
    /// Provides data for the <see cref="SoldierWrapper.OnPositionChange"/> event
    /// </summary>
    public class PositionChangeEventArgs : EventArgs
    {
        public SoldierWrapper Soldier { get; set; }

        public PositionWrapper FromPosition { get; set; }

        public PositionWrapper ToPosition { get; set; }

        public Specialty FromSpecialty { get; set; }

        public Specialty ToSpecialty { get; set; }

        public int FromPositionTimeInBillet { get; set; }
    }
}