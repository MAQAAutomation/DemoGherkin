using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Demo.CommonFramework.ExceptionHandler;
using Demo.CommonFramework.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Demo.CommonFramework.Helpers
{
    public class BaseJsonHelper
    {
        public static List<string> hashExpressionsList = new List<string>
        {
            Constants.HASH_IS_DATE,
            Constants.HASH_IS_INTEGER,
            Constants.HASH_IS_INTEGER_GREAT_ZERO,
            Constants.HASH_IS_INTEGER_LOW_ZERO,
            Constants.HASH_NOT_EMPTY_ARRAY,
            Constants.HASH_NOT_NULL,
            Constants.HASH_NOT_PRESENT,
            Constants.HASH_NULL,
            Constants.HASH_IS_DECIMAL,
            Constants.HASH_IS_NUMBER_GREAT_ZERO
        };

        /// <summary>
        /// Determines whether [is valid json] [the specified json].
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns>
        ///   <c>true</c> if [is valid json] [the specified json]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidJson(string json)
        {
            try
            {
                JObject.Parse(json);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether [is valid json array] [the specified json].
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns>
        ///   <c>true</c> if [is valid json array] [the specified json]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidJsonArray(string json)
        {
            try
            {
                JsonConvert.DeserializeObject<JArray>(json).ToObject<List<JObject>>();
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// XMLtoJson
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static string XMLtoJson(XmlDocument doc)
        {
            // To convert an XML node contained in string xml into a JSON string   
            return JsonConvert.SerializeXmlNode(doc);
        }

        /// <summary>
        /// Compares the json objects.
        /// </summary>
        /// <param name="received">The received.</param>
        /// <param name="expected">The expected.</param>
        /// <param name="error">The error.</param>
        /// <returns></returns>
        public static bool CompareJsonObjects(string received, string expected, out string error)
        {
            error = string.Empty;
            if (!IsValidJson(received) && !IsValidJsonArray(received))
            {
                error = "The received message is not a valid JSON";
                return false;
            }

            if (IsValidJsonArray(received))
            {
                var receivedJsonObjects = JsonConvert.DeserializeObject<JArray>(received).ToObject<List<JObject>>();
                var expectedJsonObjects = JsonConvert.DeserializeObject<JArray>(expected).ToObject<List<JObject>>();
                foreach (var expectedObj in expectedJsonObjects)
                {
                    foreach (var receivedObj in receivedJsonObjects)
                    {
                        string errorAux = CompareJsonObjects(expectedObj, receivedObj).ToString();
                        error += errorAux;
                        if (string.IsNullOrEmpty(errorAux))
                        {
                            error = string.Empty;
                            receivedJsonObjects.Remove(receivedObj);
                            break;
                        }
                    }
                }

                return string.IsNullOrEmpty(error);
            }
            else
            {
                JObject expectedJson = JObject.Parse(expected);
                JObject receivedJson = JObject.Parse(received);
                error = CompareJsonObjects(expectedJson, receivedJson).ToString();

                return string.IsNullOrEmpty(error);
            }
        }

        /// <summary>
        /// Checks the hash expression.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="expected">The expected.</param>
        /// <param name="current">The current.</param>
        /// <returns></returns>
        /// <exception cref="FrameworkException">Hash expression is not correct: " + expected.Value<string>()</exception>
        public static StringBuilder CheckHashExpression(string key, JToken expected, JToken current)
        {
            StringBuilder returnString = new StringBuilder();
            switch (expected.Value<string>().ToLower())
            {
                case Constants.HASH_IS_DATE:
                    if (current == null || !current.Value<string>().IsDate())
                    {
                        returnString.Append("Key " + key + " is not a Date: " + current?.Value<string>() + Environment.NewLine);
                    }
                    break;
                case Constants.HASH_IS_INTEGER:
                    if (current == null || !current.Value<string>().IsInteger())
                    {
                        returnString.Append("Key " + key + " is not an Integer: " + current?.Value<string>() + Environment.NewLine);
                    }
                    break;
                case Constants.HASH_IS_INTEGER_GREAT_ZERO:
                    if (current == null || !current.Value<string>().IsIntegerGreaterThanZero())
                    {
                        returnString.Append("Key " + key + " is not an Integer greater than zero: " + current?.Value<string>() + Environment.NewLine);
                    }
                    break;
                case Constants.HASH_IS_INTEGER_LOW_ZERO:
                    if (current == null || !current.Value<string>().IsIntegerLowerThanZero())
                    {
                        returnString.Append("Key " + key + " is not an Integer lower than zero: " + current?.Value<string>() + Environment.NewLine);
                    }
                    break;
                case Constants.HASH_NOT_EMPTY_ARRAY:
                    if (current == null || !current.Children().Any())
                    {
                        returnString.Append("Key " + key + " is empty, expected to be '"
                                            + Constants.HASH_NOT_EMPTY_ARRAY + "'" + Environment.NewLine);
                    }
                    break;
                case Constants.HASH_NOT_NULL:
                    if (current == null
                        || (current.GetType() == typeof(JObject) && !current.ToObject<JObject>().HasValues)
                        || (current.GetType() == typeof(JToken) && current.Value<string>() == null)
                        || (current.GetType() == typeof(JValue) && current.ToObject<JValue>().Value == null))
                    {
                        returnString.Append("Key " + key + " is null, expected to be '" + Constants.HASH_NOT_NULL + "'" + Environment.NewLine);
                    }
                    break;
                case Constants.HASH_NULL:
                    if (current != null
                        && ((current.GetType() == typeof(JObject) && current.ToObject<JObject>().HasValues)
                            || (current.GetType() == typeof(JToken) && current.Value<string>() != null)
                            || (current.GetType() == typeof(JValue) && current.ToObject<JValue>().Value != null)))
                    {
                        returnString.Append("Key " + key + " found, expected to be '" + expected.Value<string>() + "'" + Environment.NewLine);
                    }
                    break;
                case Constants.HASH_NOT_PRESENT:
                    if (current != null)
                    {
                        returnString.Append("Key " + key + " found, expected to be '" + expected.Value<string>() + "'" + Environment.NewLine);
                    }
                    break;
                case Constants.HASH_IS_DECIMAL:
                    decimal result;
                    if (current == null || current.Value<string>().IsInteger() || (!decimal.TryParse(current.Value<string>(), out result)))
                    {
                        returnString.Append("Key " + key + " is not a Decimal: " + current?.Value<string>() + Environment.NewLine);
                    }
                    break;
                case Constants.HASH_IS_NUMBER_GREAT_ZERO:
                    if (current == null || (!current.Value<string>().IsNumberGreaterThanZero()))
                    {
                        returnString.Append("Key " + key + " is not a Positive Number: " + current?.Value<string>() + Environment.NewLine);
                    }
                    break;
            }

            return returnString;
        }

        /// <summary>
        /// Compares the json object or array.
        /// </summary>
        /// <param name="sourcePair">The source pair.</param>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        private static StringBuilder CompareJsonObjectOrArray(KeyValuePair<string, JToken> sourcePair, JObject target)
        {
            StringBuilder returnString = new StringBuilder();
            if (sourcePair.Value.Type == JTokenType.Object)
            {
                if (target.GetValue(sourcePair.Key) == null)
                {
                    returnString.Append("Key " + sourcePair.Key
                                                            + " not found" + Environment.NewLine);
                }
                else if (target.GetValue(sourcePair.Key).Type != JTokenType.Object)
                {
                    returnString.Append("Key " + sourcePair.Key
                                                            + " is not an object in target" + Environment.NewLine);
                }
                else
                {
                    returnString.Append(CompareJsonObjects(sourcePair.Value.ToObject<JObject>(),
                                            target.GetValue(sourcePair.Key).ToObject<JObject>()));
                }
            }
            else if (sourcePair.Value.Type == JTokenType.Array)
            {
                if (target.GetValue(sourcePair.Key) == null || target.GetValue(sourcePair.Key).ToObject<JArray>() == null)
                {
                    returnString.Append("Key " + sourcePair.Key
                                                           + " not found" + Environment.NewLine);
                }
                else
                {
                    returnString.Append(CompareArrays(sourcePair.Value.ToObject<JArray>(), target.GetValue(sourcePair.Key).ToObject<JArray>(), sourcePair.Key));
                }
            }

            return returnString;
        }

        /// <summary>
        /// Deep compare two NewtonSoft JObjects. If they don't match, returns text diffs
        /// </summary>
        /// <param name="source">The expected results</param>
        /// <param name="target">The actual results</param>
        /// <returns>Text string</returns>
        private static StringBuilder CompareJsonObjects(JObject source, JObject target)
        {
            StringBuilder returnString = new StringBuilder();
            foreach (KeyValuePair<string, JToken> sourcePair in source)
            {
                if (sourcePair.Value.Type == JTokenType.Object
                    || sourcePair.Value.Type == JTokenType.Array)
                {
                    returnString.Append(CompareJsonObjectOrArray(sourcePair, target));
                }
                else
                {
                    JToken expected = sourcePair.Value;
                    JToken current = target.CustomSelectToken(sourcePair.Key);

                    if (sourcePair.Value.IsValidHashExpression())
                    {
                        returnString.Append(CheckHashExpression(sourcePair.Key, expected, current));
                        continue;
                    }

                    if (current == null || current.Type == JTokenType.Array || current.Value<string>() == null)
                    {
                        if (expected == null || expected.Value<string>() == null) continue;
                        returnString.Append("Key " + sourcePair.Key
                                            + " not found" + Environment.NewLine);
                    }
                    else
                    {
                        if (!CompareJsonTokens(current, expected))
                        {
                            returnString.Append("Key " + sourcePair.Key + ": Expected: "
                                                + sourcePair.Value + "; Received: "
                                                + target.Property(sourcePair.Key).Value
                                                + Environment.NewLine);
                        }
                    }
                }
            }

            return returnString;
        }

        /// <summary>
        /// Compares the json tokens.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <param name="expected">The expected.</param>
        /// <returns></returns>
        private static bool CompareJsonTokens(JToken current, JToken expected)
        {
            string expectedValue = expected.Value<string>();

            if (expectedValue == null) return true;

            if (current.GetType() == typeof(JValue) && current.ToObject<JValue>().Value.GetType().Equals(typeof(double)))
            {
                expectedValue = expected.Value<double>().ToString();
            }

            if (!expectedValue.Equals(current.Value<string>(), StringComparison.InvariantCultureIgnoreCase)
                && !Uri.UnescapeDataString(expectedValue).Equals(Uri.UnescapeDataString(current.Value<string>()), StringComparison.InvariantCultureIgnoreCase))
            {
                // Check regex in case the literal comparison goes wrong
                string currentValue = current.Value<string>().Replace("\r", "").Replace("\n", "");
                if (!Regex.IsMatch(currentValue, expectedValue))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Deep compare two NewtonSoft JArrays. If they don't match, returns text diffs
        /// </summary>
        /// <param name="source">The expected results</param>
        /// <param name="target">The actual results</param>
        /// <param name="arrayName">The name of the array to use in the text diff</param>
        /// <returns>Text string</returns>
        private static StringBuilder CompareArrays(JArray source, JArray target, string arrayName = "")
        {
            var returnString = new StringBuilder();
            var erroraux = new StringBuilder();
            for (var index = 0; index < source.Count; index++)
            {
                var expected = source[index];
                if (expected.Type == JTokenType.Object)
                {
                    if (target.Count == 0)
                    {
                        erroraux.Append(arrayName + " does not contain any value! Expected to have at least " + source.Count + " value(s)" + Environment.NewLine);
                    }
                    foreach (var item in target)
                    {
                        var current = (index >= target.Count) ? new JObject() : item;
                        var error = CompareJsonObjects(expected.ToObject<JObject>(), current.ToObject<JObject>());
                        if (string.IsNullOrEmpty(error.ToString()))
                        {
                            erroraux = new StringBuilder();
                            break;
                        }
                        else
                        {
                            if (erroraux.Length + error.Length < 400000)
                            {
                                erroraux.Append(arrayName + " Item " + (index + 1) + ": " + error);
                            }
                        }
                    }
                    returnString.Append(erroraux);
                }
            }

            return returnString;
        }

        /// <summary>
        /// Gets the element value from json string.
        /// </summary>
        /// <param name="jsonFile">The json file.</param>
        /// <param name="elementPath">The element JSON path.</param>
        /// <returns></returns>
        public static string GetElementValueFromJsonString(string jsonFile, string elementPath)
        {
            if (IsValidJson(jsonFile))
            {
                JObject jsonObject = JObject.Parse(jsonFile);
                return (string)jsonObject.CustomSelectToken(elementPath);
            }
            else if (IsValidJsonArray(jsonFile))
            {
                // Support for complex jsonPath like $.[?(@.MarketRefPseudo=='2269')].Id
                var array = JArray.Parse(jsonFile);
                var token = array.CustomSelectTokens(elementPath).ToList();
                if (token.Count > 0) return token[0].ToString();

                var jsonObjects = JsonConvert.DeserializeObject<JArray>(jsonFile).ToObject<List<JObject>>();
                foreach (var obj in jsonObjects)
                {
                    if (obj.CustomSelectToken(elementPath) == null) continue;
                    return obj.CustomSelectToken(elementPath).ToString();
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <param name="jsonString">The json string.</param>
        /// <returns></returns>
        public static string GetChildren(string jsonString, string rootNode)
        {
            JObject jsondata = JObject.Parse(jsonString);
            return jsondata[rootNode]?.ToString();
        }


        /// <summary>
        /// Gets the first children.
        /// </summary>
        /// <param name="jsonString">The json string.</param>
        /// <param name="rootNode">The root node.</param>
        /// <returns></returns>
        public static string GetFirstChildValue(string jsonString, string rootNode)
        {
            JObject jsondata = JObject.Parse(jsonString);
            return ((JProperty)jsondata[rootNode].First).Value.ToString();
        }

        /// <summary>
        /// Adds the single element to json string.
        /// </summary>
        /// <param name="jsonString">The json string.</param>
        /// <param name="elementPath">The element path.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="elementValue">The element value.</param>
        /// <exception cref="NotSupportedException"></exception>
        public static void AddSingleElementToJsonString(ref string jsonString, string elementPath, string elementName, string elementValue)
        {
            if (!IsValidJson(jsonString)) throw new InconclusiveFrameworkException("The json request is not a valid JSON string!: " + jsonString);

            JObject jsonData = JObject.Parse(jsonString);
            var node = jsonData.CustomSelectToken(elementPath);

            if (node == null) throw new InconclusiveFrameworkException("The element path to insert the new element does not exist: " + elementPath);

            switch (node.Type)
            {
                case JTokenType.Object:
                    {
                        string nodes = string.Empty;
                        if (elementValue == null)
                        {
                            nodes = "{\"" + elementName + "\": null }";
                        }
                        else
                        {
                            nodes = "{\"" + elementName + "\": \"" + elementValue + "\"}";
                        }
                        var objectToMerge = JObject.Parse(nodes);
                        ((JObject)node).Merge(objectToMerge);
                        break;
                    }
                case JTokenType.Array:
                    {
                        string nodes = string.Empty;
                        if (elementValue == null)
                        {
                            nodes = "{\"" + elementName + "\": null }";
                        }
                        else
                        {
                            nodes = "{\"" + elementName + "\": \"" + elementValue + "\"}";
                        }
                        var objectToMerge = new JArray(JToken.Parse(nodes));
                        ((JArray)node).Merge(objectToMerge);
                        break;
                    }
                case JTokenType.Integer:
                case JTokenType.Float:
                case JTokenType.String:
                case JTokenType.Null:
                case JTokenType.Boolean:
                    {
                        node.Replace(elementValue);
                        break;
                    }
                case JTokenType.Date:
                    {
                        if (!elementValue.IsDate())
                        {
                            throw new NotSupportedException("Date String Not Supported: " + elementValue);
                        }
                        node.Replace(elementValue);
                        break;
                    }
                default:
                    throw new NotSupportedException("A new element cannot be added to that json type: " + node.Type.ToString());
            }

            jsonString = jsonData.ToString();
        }

        /// <summary>
        /// Adds the multiple elements to json array.
        /// </summary>
        /// <param name="jsonString">The json string.</param>
        /// <param name="elementPath">The element path.</param>
        /// <param name="elements">The elements.</param>
        /// <exception cref="InconclusiveFrameworkException">The json request is not a valid JSON string!: " + jsonString
        /// or
        /// The element path to insert the new element does not exist: " + elementPath</exception>
        /// <exception cref="NotSupportedException">A new element cannot be added to that json type: " + node.Type.ToString()</exception>
        public static void AddMultipleElementsToJsonArray<T>(ref string jsonString, string elementPath, string elementName, T elements)
        {
            if (!IsValidJson(jsonString)) throw new InconclusiveFrameworkException("The json request is not a valid JSON string!: " + jsonString);

            JObject jsonData = JObject.Parse(jsonString);
            var node = jsonData.CustomSelectToken(elementPath);

            if (node == null) throw new InconclusiveFrameworkException("The element path to insert the new element does not exist: " + elementPath);

            switch (node.Type)
            {
                case JTokenType.Object:
                    {
                        var objectToMerge = JObject.Parse("{\"" + elementName + "\": [] }");
                        ((JObject)node).Merge(objectToMerge);
                        jsonString = jsonData.ToString();
                        AddMultipleElementsToJsonArray(ref jsonString, elementPath + "." + elementName, elementName, elements);
                        break;
                    }
                case JTokenType.Null:
                    {
                        string elementParent = elementPath.Substring(0, elementPath.LastIndexOf('.'));
                        JObject obj = (JObject)jsonData.CustomSelectToken(elementParent);
                        obj.Property(elementName).Remove();
                        jsonString = jsonData.ToString();
                        AddMultipleElementsToJsonArray(ref jsonString, elementParent, elementName, elements);
                        break;
                    }
                case JTokenType.Array:
                    {
                        if (elements is List<string>)
                        {
                            foreach (var item in elements as List<string>)
                            {
                                string nodes = "\"" + item + "\"";
                                var objectToMerge = new JArray(JToken.Parse(nodes));
                                ((JArray)node).Merge(objectToMerge);
                            }
                        }
                        else if (elements is List<Dictionary<string, string>>)
                        {
                            foreach (var items in elements as List<Dictionary<string, string>>)
                            {
                                string nodes = "{";
                                int count = 1;
                                foreach (var dictItem in items)
                                {
                                    if (dictItem.Value == null)
                                    {
                                        nodes += "\"" + dictItem.Key + "\": null" + ((count != items.Count) ? "," : "");
                                    }
                                    else
                                    {
                                        nodes += "\"" + dictItem.Key + "\": \"" + dictItem.Value + ((count != items.Count) ? "\"," : "\"");
                                    }

                                    count++;
                                }
                                nodes += "}";
                                var objectToMerge = new JArray(JToken.Parse(nodes));
                                ((JArray)node).Merge(objectToMerge);
                            }
                        }

                        jsonString = jsonData.ToString();
                        break;
                    }
                default:
                    throw new NotSupportedException("A new element cannot be added to that json type: " + node.Type.ToString());
            }
        }

        /// <summary>
        /// Sets the values into json string.
        /// </summary>
        /// <param name="fieldsFromResponse">The fields from response.</param>
        /// <param name="jsonString">The json string.</param>
        /// <param name="force">if set to <c>true</c> [force]. Force the set even when the element is null</param>
        public static void SetValuesIntoJsonString(Dictionary<string, string> fieldsFromResponse, ref string jsonString, bool force = false)
        {
            foreach (var field in fieldsFromResponse)
            {
                JObject jsonData = JObject.Parse(jsonString);
                if (!force && jsonData.CustomSelectToken(field.Key) == null) continue;
                AddSingleElementToJsonString(ref jsonString, field.Key, field.Key.Split('.').Last(), field.Value);
            }
        }

        /// <summary>
        /// Sets the values into json file. If any of the values in the dictionary is not in the json to modify, the new value
        /// won't be added.
        /// </summary>
        /// <param name="fieldsFromResponse">The fields from response.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="force">if set to <c>true</c> [force]. Force the set even when the element is null</param>
        public static void SetValuesIntoJsonFile(Dictionary<string, string> fieldsFromResponse, string filePath, bool force = false)
        {
            string jsonString = BaseFileUtils.GetFileContent(filePath);
            SetValuesIntoJsonString(fieldsFromResponse, ref jsonString, force);
            File.WriteAllText(BaseFileUtils.CURRENT_PATH + Path.DirectorySeparatorChar + filePath, jsonString);
        }

        /// <summary>
        /// Gets the element values from json string.
        /// </summary>
        /// <param name="jsonString">The json string.</param>
        /// <param name="elementPath">The json path to the element.</param>
        /// <returns></returns>
        public static List<JToken> GetElementValuesFromJsonString(string jsonString, string elementPath)
        {
            if (IsValidJson(jsonString))
            {
                JObject jsonObject = JObject.Parse(jsonString);
                return jsonObject.CustomSelectTokens(elementPath).ToList();
            }
            else if (IsValidJsonArray(jsonString))
            {
                var jsonObjects = JsonConvert.DeserializeObject<JArray>(jsonString).ToObject<List<JObject>>();
                List<JToken> tokens = new List<JToken>();
                foreach (var obj in jsonObjects)
                {
                    if (obj.CustomSelectToken(elementPath) != null) tokens.Add(obj.CustomSelectToken(elementPath));
                }

                return tokens;
            }

            return null;
        }
    }
}
