using System;

namespace Perscom
{
    public static class DateTimeExt
    {
        /// <summary>
        /// Calculates the absolute difference in months between two DateTime values.
        /// </summary>
        /// <param name="lValue">The first DateTime value.</param>
        /// <param name="rValue">The second DateTime value.</param>
        /// <returns>The absolute number of months between the two DateTime values.</returns>
        public static int MonthDifference(this DateTime lValue, DateTime rValue)
        {
            return Math.Abs((lValue.Month - rValue.Month) + 12 * (lValue.Year - rValue.Year));
        }
    }
}
