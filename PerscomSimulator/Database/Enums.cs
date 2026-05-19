namespace Perscom.Database
{
    public enum SoldierFunction
    {
        TimeInService,
        TimeInGrade,
        TimeInRank,
        TimeInPosition,
        TimeToRetirement,
        PayGrade
    }

    public enum PositionFunction
    {
        PositionBlueprintId,
        PositionStature,
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

    public enum PositionFlag
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

    /// <summary>
    /// Represents the various types of attributes that can be associated with an individual <see cref="Soldier"/>
    /// </summary>
    /// <remarks>The <see cref="AttributeType"/> enumeration categorizes attributes into three main groups:
    /// <list type="bullet"> <item> <description><b>Improvable:</b> Attributes that can be developed or enhanced over
    /// time, such as <see cref="Leadership"/> and <see cref="Teamwork"/>.</description> </item> <item>
    /// <description><b>Innate:</b> Attributes that are inherent to an individual, such as <see cref="Agreeableness"/>
    /// and <see cref="Extraversion"/>.</description> </item> <item> <description><b>Static:</b> Attributes that are
    /// generally fixed and not subject to significant change, such as <see cref="Intellect"/> and <see
    /// cref="Improvability"/>.</description> </item> </list> This enumeration can be used to classify and manage
    /// attributes in systems that evaluate or track personal or organizational traits.</remarks>
    public enum AttributeType
    {
        // Improvable Skills
        
        /// <summary>
        /// 
        /// </summary>
        Leadership,

        /// <summary>
        ///     <para>
        ///         The ability to maintain emotional and mental stability under stress or pressure.
        ///     </para>
        ///     <para>
        ///         Dictates how well they handle the stress of leadership without getting frustrated.
        ///     </para>
        /// </summary>
        Composure,

        /// <summary>
        /// Measures proficiency and skill in marksmanship.
        /// </summary>
        Marksmanship,

        Fitness,

        Teamwork,

        Discipline,

        // Innate

        /// <summary>
        ///     <para>
        ///         Represents the tendency to be cooperative, compassionate, and harmonious in interactions.
        ///     </para>
        ///     <para>
        ///         If a Squad Leader has 2 Agreeableness (they are a nightmare to work with), it acts as a
        ///         Burnout multiplier for everyone else in that squad.
        ///     </para>
        /// </summary>
        Agreeableness,

        Ambition,

        Conscientiousness,

        Extraversion,  

        Mindfullness,

        Courage,

        // Static
        Intellect,

        Improvability,

        /// <summary>
        ///     <para>
        ///         This dictates how well they handle sudden roster changes, bad weather, or chaotic game modes
        ///     </para>
        ///     <para>
        ///         When Combined: A player with high Composure and Adaptability has massive Burnout Resistance.
        ///         They can play Squad Leader for 2 years straight. A player with low Composure will spike in Burnout
        ///         after just one month in a leadership spot.
        ///     </para>
        /// </summary>
        Adaptability
    }

    public enum PayGradeSelection
    {
        /// <summary>
        /// Indicates that this PayGrade is an entry level rank, and will fill automatically
        /// with new soldiers.
        /// </summary>
        EntryLevel,

        /// <summary>
        /// Indicates that a RankClassification promotion is automatic after a set amount of time.
        /// </summary>
        Automatic,

        /// <summary>
        /// Indicates that a RankClassification promotion is done by a promotion board.
        /// </summary>
        PromotionBoard,

        /// <summary>
        /// Custom selection through an evaluation board or otherwise.
        /// </summary>
        SelectionProcedure
    }

    public enum PromotionBoardType
    {
        /// <summary>
        /// Pass or fail board required. First in, first out method
        /// </summary>
        SequenceOrder,

        /// <summary>
        /// Everytime someone is added to the promotion pool, the entire
        /// promotable list gets re-evaluated and sorted by board score.
        /// </summary>
        OrderedMeritList,
    }

    public enum EvaluationBoardType
    {
        /// <summary>
        /// Everytime someone is added to the promotion pool, the entire
        /// promotable list gets re-evaluated and sorted by board score.
        /// </summary>
        OrderedMerit,

        /// <summary>
        /// Pass or fail board required. First in, first out method
        /// </summary>
        Custom,
    }

    /// <summary>
    /// Defines the method for selecting soldiers from a pool during the selection process.
    /// </summary>
    public enum PoolSelection
    {
        /// <summary>
        /// Selects from all solider pools, combines all soldiers into one pool, then chooses one based
        /// on custom sorting and grouping
        /// </summary>
        Collective,

        /// <summary>
        /// Goes through each pool in order and rolls until a candidate is found
        /// </summary>
        OrderedPriority,

        /// <summary>
        /// Selects from one random soldier pool only, and checks for a candidate
        /// </summary>
        RandomByProbability
    }

    /// <summary>
    /// Defines the mode used to determine how a stipend value is applied or calculated.
    /// </summary>
    public enum StipendMode
    {
        /// <summary>
        /// The stipend is inherited from the RankClassification.
        /// </summary>
        Inherit,
        
        /// <summary>
        /// The stipend on the Rank overrides the RankClassification.
        /// </summary>
        Override,
        
        /// <summary>
        /// The stipend if added to the RankClassification stipend.
        /// </summary>
        Offset
    }
}
