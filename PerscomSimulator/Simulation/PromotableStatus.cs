namespace Perscom.Simulation
{
    /// <summary>
    /// A set of different promotion status' that a soldier
    /// can fall under.
    /// </summary>
    public enum PromotableStatus
    {
        /// <summary>
        /// Soldier is not promotable
        /// </summary>
        None,

        /// <summary>
        ///  Normal billet based selection process
        /// </summary>
        Normal,

        /// <summary>
        /// Due for an Automatic Time In Grade Promotion
        /// </summary>
        Automatic,

        /// <summary>
        /// Should be promoted because they are in a position greater
        /// than their Rank/Grade
        /// </summary>
        Position
    }
}
