using Perscom.Database;

namespace Perscom.Simulation
{
    /// <summary>
    /// Provides data for the <see cref="SoldierWrapper.OnPositionAndRankChange"/> event
    /// </summary>
    public class PositionAndRankChangeEventArgs : PositionChangeEventArgs
    {
        public Promotion Promotion { get; set; }
    }
}