using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Demo.CommonFramework.Extensions
{
    public static class StringExtensions
    {

        /// <summary>
        /// Determines whether the specified string is range.
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <returns>
        ///   <c>true</c> if the specified expected is range; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsRange(this string expected)
        {
            return Regex.IsMatch(expected, "[[(]\\d+[,-].*\\d+[])]");
        }

        /// <summary>
        /// Determines whether the specified expected is date.
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <returns>
        ///   <c>true</c> if the specified expected is date; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsDate(this string expected)
        {
            string[] formats = {"M/d/yyyy h:mm:ss tt", "M/d/yyyy h:mm tt",
                               "MM/dd/yyyy hh:mm:ss", "M/d/yyyy h:mm:ss",
                               "M/d/yyyy hh:mm tt", "M/d/yyyy hh tt",
                               "M/d/yyyy h:mm", "M/d/yyyy h:mm",
                               "MM/dd/yyyy hh:mm", "M/dd/yyyy hh:mm"};

            DateTime.TryParse(expected, out DateTime dateTime);
            if (dateTime <= DateTime.MinValue)
            {
                DateTime.TryParseExact(expected, formats, new CultureInfo("en-US"), DateTimeStyles.None, out dateTime);
            }
            return dateTime > DateTime.MinValue;
        }

        /// <summary>
        /// Determines whether this instance is integer.
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <returns>
        ///   <c>true</c> if the specified expected is integer; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInteger(this string expected)
        {
            return int.TryParse(expected, out int i);
        }

        /// <summary>
        /// Determines whether [is integer greater than zero].
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <returns>
        ///   <c>true</c> if [is integer greater than zero] [the specified expected]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsIntegerGreaterThanZero(this string expected)
        {
            return int.TryParse(expected, out int i) && i > 0;
        }

        /// <summary>
        /// Determines whether [is integer lower than zero].
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <returns>
        ///   <c>true</c> if [is integer lower than zero] [the specified expected]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsIntegerLowerThanZero(this string expected)
        {
            return int.TryParse(expected, out int i) && i < 0;
        }

        /// <summary>Determines whether [is positive number].</summary>
        /// <param name="expected">The expected.</param>
        /// <returns>
        ///   <c>true</c> if [is positive number] [the specified expected]; otherwise, <c>false</c>.</returns>
        public static bool IsNumberGreaterThanZero(this string expected)
        {
            if (expected.IsIntegerGreaterThanZero() || expected.IsDecimalGreaterThanZero())
            {
                return true;
            }
            else return false;
        }

        /// <summary>Determines whether [is decimal greater than zero].</summary>
        /// <param name="expected">The expected.</param>
        /// <returns>
        ///   <c>true</c> if [is decimal greater than zero] [the specified expected]; otherwise, <c>false</c>.</returns>
        public static bool IsDecimalGreaterThanZero(this string expected)
        {
            return decimal.TryParse(expected, out decimal i) && i > 0;

        }
    }
}
