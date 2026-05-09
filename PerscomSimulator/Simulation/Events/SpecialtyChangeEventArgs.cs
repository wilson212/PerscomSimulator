using Perscom.Database;
using System;

namespace Perscom.Simulation
{
    /// <summary>
    /// Provides data for the <see cref="SoldierWrapper.OnSpecialtyChange"/> event
    /// </summary>
    public class SpecialtyChangeEventArgs : EventArgs
    {
        public SoldierWrapper Soldier { get; set; }

        public Occupation From { get; set; }

        public Occupation To { get; set; }
    }
}