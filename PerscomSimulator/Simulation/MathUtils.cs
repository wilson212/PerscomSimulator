using System;

namespace Perscom.Simulation
{
    /// <summary>
    /// A utility class providing various mathematical methods, including generating random
    /// numbers, calculating Gaussian distributions, bell curves, and skewed integers.
    /// </summary>
    public static class MathUtils
    {
        /// <summary>
        /// Generates a random number following a standard Gaussian (Normal) Distribution curve.
        /// </summary>
        public static double NextGaussian(double mean, double standardDeviation)
        {
            // Using .NET 8's Random.Shared which is thread-safe and highly optimized
            double u1 = 1.0 - Random.Shared.NextDouble(); 
            double u2 = 1.0 - Random.Shared.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return mean + (standardDeviation * randStdNormal);
        }

        /// <summary>
        /// Generates a random number following an Asymmetric (Split) Gaussian curve.
        /// </summary>
        public static double NextAsymmetricGaussian(double mean, double leftStandardDeviation, double rightStandardDeviation)
        {
            double u1 = 1.0 - Random.Shared.NextDouble();
            double u2 = 1.0 - Random.Shared.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

            if (randStdNormal < 0)
                return mean + (randStdNormal * leftStandardDeviation);
            else
                return mean + (randStdNormal * rightStandardDeviation);
        }

        /// <summary>
        /// Calculates the exact Probability Density (the Y-axis height) for plotting on a chart.
        /// </summary>
        public static double GetBellCurveY(double x, double mean, double standardDeviation)
        {
            double variance = standardDeviation * standardDeviation;
            double diff = x - mean;
            return (1.0 / Math.Sqrt(2.0 * Math.PI * variance)) * Math.Exp(-(diff * diff) / (2.0 * variance));
        }

        /// <summary>
        /// Calculates the Probability Density for an Asymmetric curve to plot on your RadChartView.
        /// </summary>
        public static double GetAsymmetricBellCurveY(double x, double mean, double leftStandardDeviation, double rightStandardDeviation)
        {
            double stdDev = x < mean ? leftStandardDeviation : rightStandardDeviation;
            double variance = stdDev * stdDev;
            double diff = x - mean;

            // Proper normalization factor for a split normal distribution so the area under the curve equals 1.0
            double normalizationFactor = Math.Sqrt(2.0 / Math.PI) / (leftStandardDeviation + rightStandardDeviation);

            return normalizationFactor * Math.Exp(-(diff * diff) / (2.0 * variance));
        }

        /// <summary>
        /// Generates a random integer from a clamped Gaussian distribution using user-defined bounds.
        /// </summary>
        public static int GenerateSkewedInt(int min, int average, int max)
        {
            if (min > max) 
                throw new ArgumentException("Minimum months cannot be greater than maximum months.");
            
            if (average < min || average > max)
                throw new ArgumentException("Average months must be between minimum and maximum months.");

            int range = max - min;
            
            // For small ranges (≤10), use /3.0 so ±1.5σ covers the full range — 
            // the extremes are unlikely but reachable.
            // For large ranges (>10), keep /6.0 for the classic 99.7% bell curve.
            double divisor = range <= 10 ? 3.0 : 6.0;
            double stdDev = Math.Max(1.0, range / divisor);

            double curveValue = NextGaussian(average, stdDev);
            int result = (int)Math.Round(curveValue);

            return Math.Clamp(result, min, max);
        }

        /// <summary>
        /// Generates a random integer from a clamped Gaussian distribution, allowing you to skew the aggressiveness of the curve.
        /// </summary>
        public static int GenerateSkewedInt(int min, int average, int max, double leftWeight, double rightWeight)
        {
            if (min > max) 
                throw new ArgumentException("Minimum months cannot be greater than maximum months.");
            
            if (average < min || average > max)
                throw new ArgumentException("Average months must be between minimum and maximum months.");

            int range = max - min;
            
            // For small ranges (≤10), use /3.0 so ±1.5σ covers the full range — 
            // the extremes are unlikely but reachable.
            // For large ranges (>10), keep /6.0 for the classic 99.7% bell curve.
            double divisor = range <= 10 ? 3.0 : 6.0;
            double baseStdDev = Math.Max(1.0, range / divisor);

            double leftStdDev = baseStdDev * leftWeight;
            double rightStdDev = baseStdDev * rightWeight;

            double curveValue = NextAsymmetricGaussian(average, leftStdDev, rightStdDev);
            int result = (int)Math.Round(curveValue);

            return Math.Clamp(result, min, max);
        }

        /// <summary>
        /// Generates a random double value within the specified range [minimum, maximum].
        /// </summary>
        /// <param name="minimum">The inclusive lower bound of the range.</param>
        /// <param name="maximum">The exclusive upper bound of the range.</param>
        /// <returns>A random double value in the range [minimum, maximum).</returns>
        public static double GetRandomDoubleInRange(double minimum, double maximum)
        {
            // Random.NextDouble() returns a value in the range [0.0, 1.0)
            // The formula scales it to the desired range [minimum, maximum)
            return Random.Shared.NextDouble() * (maximum - minimum) + minimum;
        }
    }
}