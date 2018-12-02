namespace Perscom.Database
{
    public enum SoldierFunction
    {
        TimeInService,
        TimeInGrade,
        TimeInRank,
        TimeInPosition,
        TimeToRetirement,
    }

    public enum PositionFunction
    {
        BilletId,
        BilletStature,
        IsNormalAssignment,
        IsSpecialAssignment,
        IsCommandPosition,
        IsStaffPosition
    }

    public enum ClauseLeftSelector
    {
        SoldierValue,
        SoldierPosition,
        SoldierExperience,
    }

    public enum BilletFlag
    {
        /// <summary>
        /// A normally assigned position with no flags
        /// </summary>
        NormalAssignment,

        /// <summary>
        /// A special assignment, which may exempt the soldier from
        /// lateral or promotional movement
        /// </summary>
        SpecialAssignment,

        /// <summary>
        /// A command position assignment
        /// </summary>
        CommandPosition,

        /// <summary>
        /// A staff position assignment
        /// </summary>
        StaffPosition
    }
}
