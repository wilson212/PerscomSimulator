using System;

namespace Perscom
{
    public static class IntExtensions
    {
        /// <summary>
        /// Performs a left circular rotation on the specified 32-bit unsigned integer by a given number of bits.
        /// </summary>
        /// <param name="value">The 32-bit unsigned integer to rotate.</param>
        /// <param name="count">The number of bits to rotate the integer to the left.</param>
        /// <returns>The 32-bit unsigned integer after performing the left circular rotation.</returns>
        public static uint RotateLeft(this uint value, int count)
        {
            return (value << count) | (value >> (32 - count));
        }

        /// <summary>
        /// Converts the specified integer to its ordinal representation as a string.
        /// </summary>
        /// <param name="num">The integer to convert to an ordinal representation.</param>
        /// <returns>A string representing the ordinal form of the integer (e.g., "1st", "2nd", "3rd", "4th", etc.).</returns>
        public static string ToTitleCase(this int num)
        {
            switch (num % 100)
            {
                case 11:
                case 12:
                case 13:
                    return num + "th";
            }

            switch (num % 10)
            {
                case 1:
                    return num + "st";
                case 2:
                    return num + "nd";
                case 3:
                    return num + "rd";
                default:
                    return num + "th";
            }
        }

        /// <summary>
        /// Converts the given integer to its corresponding alphabetical character representation.
        /// </summary>
        /// <param name="number">The integer to convert. For example, 1 corresponds to 'A' or 'a', 2 to 'B' or 'b', and so on.</param>
        /// <param name="isCaps">A boolean value indicating whether the resulting character should be uppercase (true) or lowercase (false).</param>
        /// <returns>A string containing the alphabetical character representation of the given integer.</returns>
        public static string ToCharString(this int number, bool isCaps)
        {
            Char c = (Char)((isCaps ? 65 : 97) + (number - 1));
            return c.ToString();
        }
    }
}
