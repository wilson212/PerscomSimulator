namespace Perscom.Simulation
{
    public class Rank
    {
        /// <summary>
        /// Gets or Sets the rank name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or Sets the rank abbreviation name
        /// </summary>
        public string Abbreviation { get; set; }

        /// <summary>
        /// Gets or Sets the grade of this rank
        /// </summary>
        public int Grade { get; set; }

        /// <summary>
        /// Indicates the type of rank
        /// </summary>
        public RankType Type { get; set; }

        /// <summary>
        /// Gets the character code that represents this rank type
        /// </summary>
        public char TypeCode
        {
            get
            {
                switch (Type)
                {
                    case RankType.Officer: return 'o';
                    case RankType.Warrant: return 'w';
                    default: return 'e';
                }
            }
        }

        /// <summary>
        /// The minimum time (in months) left before retirement to even
        /// be considered for this position
        /// </summary>
        public int MinTimeForConsideration { get; set; } = 0;

        /// <summary>
        /// The minimum time (in months) to be promotable to the next rank
        /// </summary>
        public int PromotableAt { get; set; } = 12;

        /// <summary>
        /// Indicates the maximum time in grade
        /// </summary>
        public int MaxTimeInGrade { get; set; } = 0;

        /// <summary>
        /// If this rank is a stature rank, then the retirement rank of the soldier
        /// will be adjusted to meet the minimum and maximum range.
        /// </summary>
        public Range<int> Stature { get; set; } = null;

        public override string ToString() => Name;
    }

    public enum RankType
    {
        Enlisted,
        Officer,
        Warrant
    }
}
