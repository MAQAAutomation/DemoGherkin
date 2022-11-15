
using System;
using System.Linq;
using System.Text;
using Demo.CommonFramework.Helpers;

namespace Demo.UIAutomation.Framework.Helpers
{
    public static class RandomGenerator
    {
        private static Random rand = new Random();
        private const string ALPHAS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private const string NUMERICS = "0123456789";
        private const string SPECIALS = "!£$%^&*()_+=-[];'#,./{}:@~<>?";

        public const int RANDOM_LENGTH = -1;

        /// <summary>
        /// Generates a random string of ASCII characters of the specified length.
        /// If no length is passed then the length of the string will be randomly picked from
        /// a length between 1 and 100 inclusive
        /// </summary>
        /// <param name="length">a positive integer greater than 0</param>
        /// <param name="lowercase">if true the output will be lowercase otherwise not</param>
        /// <returns>a string of randomly generated ASCII characters of the specified length</returns>
        public static string GetAlphaString(int length = RANDOM_LENGTH) // -1 means random length from 1 - 100
        {
            return GetString(length, true, false, false);
        }

        /// <summary>
        /// Get a random string of the specified lenght.
        /// The string can contain alpha and or numerics and or specials characters.
        /// </summary>
        /// <param name="length">Length of the string</param>
        /// <param name="alphas">True to include alpha characters into the result string</param>
        /// <param name="numerics">True to include numeric charactesr into the result string</param>
        /// <param name="specials">True to include specials characters into the result string</param>
        public static string GetString(int length, bool alphas = true, bool numerics = true, bool specials = true)
        {
            string choices = string.Empty;
            if (alphas) { choices += ALPHAS; }
            if (numerics) { choices += NUMERICS; }
            if (specials) { choices += SPECIALS; }

            if (length <= 0)
            {
                length = BaseUtils.RandomizeInt(1, 101);
            }

            return GetStringFrom(length, choices);
        }

        /// <summary>
        /// Gets the string from.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <param name="selectionCharacters">The selection characters.</param>
        /// <returns></returns>
        public static string GetStringFrom(int length, string selectionCharacters)
        {
            var result = new StringBuilder();
            if (length <= 0)
            {
                length = BaseUtils.RandomizeInt(1, 101);
            }

            for (var i = 0; i < length; i++)
            {
                var ch = selectionCharacters.ElementAt(BaseUtils.RandomizeInt(0, selectionCharacters.Length));
                result.Append(ch);
            }

            return result.ToString();
        }

        /// <summary>
        /// Gets the double.
        /// </summary>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns></returns>
        public static double GetDouble(double min = 0, double max = double.MaxValue)
        {
            return rand.NextDouble() * (max - min) + min;
        }

        /// <summary>
        /// Remove invalid caracters for a folder or file name. That characters normally are introduced when we run parametriced tests
        /// </summary>
        /// <param name="testName"></param>
        /// <returns></returns>
        public static string RemoveInvalidCharsForFolderOrFilename(string name)
        {
            return name.Replace(@"/", "")
                .Replace(@"\", "")
                .Replace("\"", "")
                .Replace(":", "");
        }

        [Obsolete("GetInt is deprecated, please use the CommonFramework instead.")]
        public static int GetInt(int min = 0, int max = Int32.MaxValue)
        {
            return rand.Next(min, max);
        }

        [Obsolete("GetGuid is deprecated, please use CommonFramework instead.")]
        public static string GetGuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}


