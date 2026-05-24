namespace Perscom
{
    public enum ComparisonOperator
    {
        Equals,
        NotEqualTo,
        GreaterThan,
        GreaterThanOrEqualTo,
        LessThan,
        LessThanOrEqualTo
    }

    public enum Race
    {
        White,
        Black,
        Hispanic,
        Asian
    }

    /// <summary>
    /// Specifies the alignment options for the organizational chart.
    /// Determines how elements will be positioned relative to the command trunk line
    /// or other visual structures within the chart layout.
    /// </summary>
    public enum OrgChartAlignment
    {
        /// <summary>
        /// Places the category group box directly on the command trunk line, centered horizontally.
        /// A hub shape is created at the bottom of the box to continue the trunk chain. Used for primary
        /// command-line categories (e.g., Leadership, Executive Staff).
        /// </summary>
        Center,
        
        /// <summary>
        /// Places the category as a side branch to the left of the trunk.
        /// Connected to the trunk via ConnectShapesSideBranch (horizontal elbow routing). Floats alongside the trunk
        /// without consuming vertical space. (e.g., Personal Staff Group on the left)
        /// </summary>
        LeftSide,

        /// <summary>
        /// Aligns the category group box to the right of the command trunk line.
        /// This layout is typically used for subordinate or support categories
        /// that are connected to the primary chain but offset for clarity and distinction.
        /// A linking line is created to bridge the gap between the trunk line and the box.
        /// </summary>
        RightSide,

        /// <summary>
        /// Aligns the category group boxes symmetrically on both sides of the command trunk line, starting on the left.
        /// </summary>
        SplitLeft,

        /// <summary>
        /// Aligns the category group boxes symmetrically on both sides of the command trunk line, starting on the Right.
        /// </summary>
        SplitRight,

        /// <summary>
        /// Aligns the category group box symmetrically on both sides of the command trunk line,
        /// creating an equal division on the canvas. A continuation hub shape is created
        /// at the bottom center of the box to extend the trunk chain. Typically used for
        /// balanced or dual-group structures in the organizational chart.
        /// </summary>
        SplitCenter
    }

    public enum OrgChartPosition
    {
        Leadership,
        PersonalStaff,
        ExecutiveCommand,
        ExecutiveStaff,
        CoordinatingStaff,
        SpecialStaff,
        SupportStaff,
        GeneralStaff,
    }
}
