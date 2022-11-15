using System;
using System.Collections.Generic;
using System.Text;
using Demo.CommonFramework.Extensions;
using Demo.CommonFramework.Helpers;
using Demo.TestReport.Framework.Core;
using Newtonsoft.Json.Linq;

namespace Demo.RestServiceFramework
{
    public static class Common
    {
        /// <summary>
        /// Compares the response.
        /// </summary>
        /// <param name="result">The SOAP result.</param>
        /// <param name="expected">The expected.</param>
        /// <param name="error">The error.</param>
        /// <returns></returns>
        public static bool? CompareResponse(object result, object expected, out string error)
        {
            error = string.Empty;
            if (BaseJsonHelper.IsValidJson((string)expected) || BaseJsonHelper.IsValidJsonArray((string)expected))
            {
                return BaseJsonHelper.CompareJsonObjects((string)result, (string)expected, out error);
            }
            else
            {
                return BaseUtils.CompareSimpleObjects((string)result, (string)expected, "response", out error);
            }
        }

        /// <summary>
        /// Checks the values in response.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="error">The error.</param>
        /// <param name="atLeastOne">if set to <c>true</c> [at least one].</param>
        /// <returns></returns>
        public static bool? CheckValuesInResponse(string response, Dictionary<string, string> dictionary, out string error, bool atLeastOne = true)
        {
            bool equals = true;
            StringBuilder finalError = new StringBuilder();
            foreach (var item in dictionary)
            {
                if (string.IsNullOrEmpty(item.Key) || string.IsNullOrWhiteSpace(item.Key)) continue;

                List<JToken> values = BaseJsonHelper.GetElementValuesFromJsonString(response, item.Key);

                if (values == null || values.Count == 0)
                {
                    finalError.Append(item.Key + " not found in the response. Please check the response: " + response);
                    equals = false;
                    continue;
                }

                foreach (var value in values)
                {
                    string valueString = GetValueStringFromJToken(value);

                    ExtentTestManager.LogInfo(item.Key + " received: " + valueString + ", expected: " + item.Value);
                    if (BaseJsonHelper.hashExpressionsList.Contains(item.Value.ToLower()))
                    {
                        JObject jObject = JObject.Parse("{\"Item\": \"" + item.Value + "\"}");
                        JToken expectedToken = jObject.CustomSelectToken("Item");
                        finalError.Append(BaseJsonHelper.CheckHashExpression(item.Key, expectedToken, value));
                        equals = string.IsNullOrEmpty(finalError.ToString());
                    }
                    else
                    {
                        if (BaseUtils.CheckEqualityOrInRange(item.Value, value.Value<string>(), item.Key, out string errorAux).Equals(atLeastOne))
                        {
                            equals &= atLeastOne;
                            finalError.Append(string.IsNullOrEmpty(errorAux) ? string.Empty : Environment.NewLine + errorAux);
                            break;
                        }
                        finalError.Append(string.IsNullOrEmpty(errorAux) ? string.Empty : Environment.NewLine + errorAux);
                    }
                }
            }

            error = finalError.ToString();
            return equals;
        }

        /// <summary>
        /// Checks the ordered by.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="orderType">Type of the order.</param>
        /// <param name="orderBy">The order by.</param>
        /// <returns></returns>
        public static bool? CheckOrderedBy(string response, BaseListUtils.EOrderType orderType, string orderBy)
        {
            List<JToken> values = BaseJsonHelper.GetElementValuesFromJsonString(response, orderBy);
            List<string> stringValues = new List<string>();
            values.ForEach(x => stringValues.Add(x.Value<string>()));

            return orderType.Equals(BaseListUtils.EOrderType.Ascendant) ?
                    BaseListUtils.IsListOrdered(stringValues) : BaseListUtils.IsListOrderedByDescending(stringValues);
        }

        /// <summary>
        /// Gets the value string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private static string GetValueStringFromJToken(JToken value)
        {
            string valueString = string.Empty;
            if (value.GetType() == typeof(JObject))
            {
                valueString = value.ToObject<JObject>().Value<string>();
            }
            else if (value.GetType() == typeof(JToken))
            {
                valueString = value.Value<string>();
            }
            else if (value.GetType() == typeof(JValue))
            {
                valueString = value.ToObject<JValue>().Value?.ToString();
            }
            else if (value.GetType() == typeof(JArray))
            {
                valueString = value.ToObject<JArray>().ToString();
            }

            return valueString;
        }
    }
}
