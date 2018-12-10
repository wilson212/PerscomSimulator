using Perscom.Database;
using System;

namespace Perscom.Simulation
{
    /// <summary>
    /// Provides data for the <see cref="SoldierWrapper.OnRankGradeChange"/>
    /// and <see cref="SoldierWrapper.OnRankTypeChange"/> events
    /// </summary>
    public class RankChangeEventArgs : EventArgs
    {
        public SoldierWrapper Soldier { get; set; }

        public Promotion Promotion { get; set; }
    }
}