using System;

namespace Perscom.Simulation
{
    /// <summary>
    /// Provides data for the <see cref="SoldierWrapper.OnLateralPositionExchange"/> event
    /// </summary>
    public class LateralPositionExchangeEventArgs : EventArgs
    {
        /// <summary>
        /// Provides data for the first <see cref="SoldierWrapper.OnPositionChange"/>
        /// </summary>
        public PositionChangeEventArgs Soldier1EventArgs { get; set; }

        /// <summary>
        /// Provides data for the second <see cref="SoldierWrapper.OnPositionChange"/>
        /// </summary>
        public PositionChangeEventArgs Soldier2EventArgs { get; set; }
    }
}