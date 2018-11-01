using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perscom.Simulation
{
    /// <summary>
    /// Represents an array of Spawn options the <see cref="Simulator"/>
    /// can use to spawn soldiers to fill empty <see cref="Database.Billet"/>s
    /// </summary>
    public enum BilletSelection
    {
        /// <summary>
        /// Soldiers will either be promoted into the billet, or chosen
        /// from the Lateral selection pool
        /// </summary>
        PromotionOrLateral = 0,

        /// <summary>
        /// Soldiers will only be selected that are of a lower Grade
        /// than the Billet
        /// </summary>
        PromotionOnly = 1,

        /// <summary>
        /// Soldiers will only be selected that are of the same
        /// Grade as the Billet Grade
        /// </summary>
        LateralOnly = 2,

        /// <summary>
        /// The custom soldier generator will be used only
        /// </summary>
        CustomGenerator = 3
    }
}
