using System.Collections.Generic;
using System.Linq;
using Demo.CommonFramework.ExceptionHandler;
using Demo.CommonFramework.Helpers;
using Newtonsoft.Json.Linq;

namespace Demo.CommonFramework.Extensions
{
    /// <summary>
    /// Add extension methods to the JToken object
    /// </summary>
    public static class JsonExtensions
    {
        /// <summary>
        /// Gets the valid json path.
        /// </summary>
        /// <param name="elementPath">The element path.</param>
        /// <returns></returns>
        private static string GetValidJsonPath(string elementPath)
        {
            if (!elementPath.Contains(" ") || elementPath.Contains("@")) return elementPath;

            string currentPath = string.Empty;
            List<string> elementPathSplit = elementPath.Replace("'", "\\'").Split('.').ToList();
            elementPathSplit.ForEach(x => currentPath += x.Contains(" ") ? "['" + x + "']." : x + ".");
            return currentPath.Remove(currentPath.LastIndexOf('.'));
        }

        /// <summary>
        /// Customs the select token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="elementPath">The element path.</param>
        /// <returns></returns>
        public static JToken CustomSelectToken(this JObject token, string elementPath)
        {
            return token.SelectToken(GetValidJsonPath(elementPath));
        }

        /// <summary>
        /// Customs the select tokens.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="elementPath">The element path.</param>
        /// <returns></returns>
        public static IEnumerable<JToken> CustomSelectTokens(this JToken token, string elementPath)
        {
            return token.SelectTokens(GetValidJsonPath(elementPath));
        }

        /// <summary>
        /// Determines whether [is valid hash expression].
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>
        ///   <c>true</c> if [is valid hash expression] [the specified token]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidHashExpression(this JToken token)
        {
            if (token.Value<string>() == null) return false;

            bool valid = BaseJsonHelper.hashExpressionsList.Contains(token.Value<string>().ToLower());

            if (!valid && token.Value<string>().StartsWith(Constants.StepHashVariablesIndicatorChar.ToString()))
            {
                throw new FrameworkException("Hash expression is not correct: " + token.Value<string>() + ", please try one of these: "
                                               + string.Join(", ", BaseJsonHelper.hashExpressionsList));
            }

            return valid;
        }
    }
}
