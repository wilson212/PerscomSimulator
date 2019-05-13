namespace Perscom.Simulation
{
    /// <summary>
    /// Represents an array of Soldier selection procedures the <see cref="Simulator"/>
    /// will use to spawn soldiers to fill empty <see cref="Database.Billet"/>s
    /// </summary>
    public enum SelectionProcedure
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
        /// Position will be filled by a brand new soldier
        /// </summary>
        CreateNewSoldier = 3,

        /// <summary>
        /// Soldiers will be selected using an <see cref="Database.OrderedProcedure"/>
        /// </summary>
        OrderedProcedure = 4,

        /// <summary>
        /// Soldiers will be selected using an <see cref="Database.RandomizedProcedure"/>
        /// </summary>
        RandomizedProcedure = 5
    }
}
