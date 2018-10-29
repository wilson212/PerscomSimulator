namespace Perscom.Simulation
{
    /// <summary>
    /// This object represents a list of settings that will be applied
    /// during the initial setup of a simulation
    /// </summary>
    public class SimulatorSettings
    {
        public static SimulatorSettings Default
        {
            get { return new SimulatorSettings(); }
        }

        public bool ProcessEnlisted { get; set; } = true;

        public bool ProcessOfficers { get; set; } = true;

        public bool ProcessWarrant { get; set; } = true;

        public bool ProcessRankType(RankType type)
        {
            switch (type)
            {
                case RankType.Enlisted: return ProcessEnlisted;
                case RankType.Officer: return ProcessOfficers;
                case RankType.Warrant: return ProcessWarrant;
                default:
                    throw new System.Exception("Envalid Rank Type");
            }
        }
    }
}
