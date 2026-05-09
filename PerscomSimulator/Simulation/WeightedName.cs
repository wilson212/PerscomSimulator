namespace Perscom.Simulation
{
    /// <summary>
    /// Represents a weighted name, with a probability associated with it.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="probability"></param>
    public sealed class WeightedName(string name, int probability) : IProbable
    {
        /// <summary>
        /// Gets the name associated with this instance of the weighted name.
        /// </summary>
        public string Name { get; } = name;

        /// <summary>
        /// Gets the probability associated with this instance of the weighted name.
        /// </summary>
        public int Probability { get; } = probability;
    }
}