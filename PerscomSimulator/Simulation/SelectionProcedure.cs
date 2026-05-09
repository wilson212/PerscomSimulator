namespace Perscom.Simulation
{
    /// <summary>
    /// Represents an array of Soldier selection procedures the <see cref="Simulator"/>
    /// will use to spawn soldiers to fill empty <see cref="Database.PositionBlueprint"/>s
    /// </summary>
    public enum SelectionProcedure
    {
        /// <summary>
        /// Soldiers will either be promoted into the billet, or chosen
        /// from the Lateral selection pool
        /// </summary>
        PromotionOrLateral = 0,

        /// <summary>
        /// Soldiers will only be selected that are of a lower PayGrade
        /// than the Billet
        /// </summary>
        PromotionOnly = 1,

        /// <summary>
        /// Soldiers will only be selected that are of the same
        /// PayGrade as the Billet PayGrade
        /// </summary>
        LateralOnly = 2,

        /// <summary>
        /// Position will be filled by a brand new soldier
        /// </summary>
        CreateNewSoldier = 3,

        /// <summary>
        /// Soldiers will be selected using a fully user-configured
        /// Filter/Group/Sort pipeline
        /// </summary>
        EvaluationBoard = 4
    }
}
