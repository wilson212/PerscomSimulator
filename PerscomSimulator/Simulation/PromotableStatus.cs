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
        Position,

        /// <summary>
        /// Should be laterally promoted because they are in a position of 
        /// the same grade as their current rank, but different variant.
        /// </summary>
        Lateral,

        /// <summary>
        /// Should be demoted in grade due to being in a position of
        /// a lower grade than thier current rank
        /// </summary>
        Demotion
    }
}
