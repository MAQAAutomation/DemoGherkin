using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Demo.CommonFramework.ExceptionHandler;
using Demo.CommonFramework.Extensions;

namespace Demo.CommonFramework.Helpers
{
    public class BaseUtils
    {
        private static Random rand = new Random();

        public enum EComparisonType
        {
            [Description("Equals to")]
            EqualsTo,
            [Description("Greater than")]
            GreaterThan,
            [Description("Greater or equal than")]
            GreaterOrEqualThan,
            [Description("Lower than")]
            LowerThan,
            [Description("Lower or equal than")]
            LowerOrEqualThan,
            [Description("Different to")]
            Different
        }

        /// <summary>
        /// Compares the objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="received">The received.</param>
        /// <param name="expected">The expected.</param>
        /// <param name="type">The type.</param>
        /// <param name="error">The error.</param>
        /// <returns></returns>
        public static bool CompareObjects<T>(T received, T expected, Type type, out string error)
        {
            var errorBuilder = new StringBuilder();
            error = string.Empty;

            if (expected == null) return true;

            string errorAux = string.Empty;
            bool equalObjects = true;

            if (type.BaseType.Name == typeof(List<>).Name)
            {
                bool? temp = CheckIfEmptyListOrDifferent(received, expected, type, out errorAux);
                error += errorAux;
                if (temp != null) return (bool)temp;
            }
            else if (type.BaseType.Name == typeof(Array).Name)
            {
                bool? temp = CheckIfEmptyArrayOrDifferent(received, expected, type, out errorAux);
                error += errorAux;
                if (temp != null) return (bool)temp;
            }

            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (property.Name.Contains("ExtensionData")
                    || property.PropertyType.IsPrimitive
                    || property.PropertyType.BaseType == null
                    || property.Name.ToUpper().EndsWith("ID")
                    || (property.Name.Contains("Policy") && property.Name.Contains("Ref"))
                    || property.Name.Equals("PolicyType")) continue;

                if (received.GetType().BaseType.Name == typeof(List<>).Name)
                {
                    IList resultList = received as IList;
                    IList expectedList = expected as IList;

                    equalObjects &= FindElementInList(resultList, expectedList, out errorAux);
                    errorBuilder.Append(errorAux);
                }
                else if (property.PropertyType.BaseType.Name == typeof(List<>).Name)
                {
                    equalObjects &= CompareObjects(property.GetValue(received), property.GetValue(expected), property.PropertyType, out errorAux);
                    errorBuilder.Append(errorAux);
                }
                else
                {
                    object valueReceived = property.GetValue(received);
                    object valueExpected = property.GetValue(expected);
                    equalObjects &= CompareSimpleObjects(valueReceived, valueExpected, property.Name, out errorAux);
                    errorBuilder.Append(errorAux);
                }
            }

            error = errorBuilder.ToString();

            return equalObjects;
        }

        /// <summary>
        /// Gets the local port available.
        /// </summary>
        /// <returns></returns>
        public static int GetLocalPortAvailable()
        {
            TcpListener listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        /// <summary>
        /// Gets the local ip.
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new FrameworkException("No network adapters with an IPv4 address in the system!");
        }

        /// <summary>
        /// Removes the numbers at the end.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        internal static string RemoveNumbersAtTheEnd(string name)
        {
            string pattern = @"\d+$";
            string replacement = "";
            Regex rgx = new Regex(pattern);
            return rgx.Replace(name, replacement);
        }

        /// <summary>
        /// Finds the element in list.
        /// </summary>
        /// <param name="resultList">The result list.</param>
        /// <param name="expectedList">The expected list.</param>
        /// <param name="error">The error.</param>
        /// <returns></returns>
        private static bool FindElementInList(IList resultList, IList expectedList, out string error)
        {
            bool found = true;
            bool tempComparison = false;
            string errorAux = string.Empty;
            var accumError = new StringBuilder();
            var errorString = new StringBuilder();
            error = string.Empty;

            foreach (var resultItem in resultList)
            {
                int count = 0;
                foreach (var expectedItem in expectedList)
                {
                    count++;
                    tempComparison = CompareObjects(resultItem, expectedItem, resultItem.GetType(), out errorAux);
                    if (tempComparison)
                    {
                        accumError = new StringBuilder();
                        break;
                    }
                    else
                    {
                        accumError.Append("\n**List item " + count + ":");
                    }
                    accumError.Append(errorAux);
                }

                found &= tempComparison;
                errorString.Append(accumError);
            }

            error = errorString.ToString();

            return found;
        }

        /// <summary>
        /// Compares the simple objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="received">The received.</param>
        /// <param name="expected">The expected.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="error">The error.</param>
        /// <returns></returns>
        public static bool CompareSimpleObjects<T>(T received, T expected, string propertyName, out string error, EComparisonType comparisonType = EComparisonType.EqualsTo)
        {
            error = string.Empty;
            // We only test those that is different to null
            if (expected == null) return true;

            string stringReceived = received?.ToString().ToLower().Replace("\r", "").Replace("\n", " ");
            string stringExpected = expected.ToString().ToLower().Replace("\r", "").Replace("\n", " ");

            bool equalObjects = CompareAlphanumericStrings(stringReceived, stringExpected, comparisonType);

            if (!equalObjects)
            {
                error = "The value of " + propertyName + " does not match the expected! Comparison type: " + GetEnumDescription(comparisonType)
                    + ". Expected: " + expected.ToString() + ", Received: " + received?.ToString();
            }

            return equalObjects;
        }

        /// <summary>
        /// Compares the alphanumeric strings.
        /// </summary>
        /// <param name="leftString">The left string.</param>
        /// <param name="rightString">The right string.</param>
        /// <param name="comparisonType">Type of the comparison.</param>
        /// <returns></returns>
        public static bool CompareAlphanumericStrings(string leftString, string rightString, EComparisonType comparisonType)
        {
            bool result = false;
            switch (comparisonType)
            {
                case EComparisonType.EqualsTo:
                    result = leftString == rightString;
                    break;
                case EComparisonType.Different:
                    result = leftString != rightString;
                    break;
                case EComparisonType.GreaterOrEqualThan:
                    result = leftString == rightString || CompareAlphanumericStrings(leftString, rightString, EComparisonType.GreaterThan);
                    break;
                case EComparisonType.GreaterThan:
                    result = IsGreaterOrLower(leftString, rightString, EComparisonType.GreaterThan);
                    break;
                case EComparisonType.LowerOrEqualThan:
                    result = leftString == rightString || CompareAlphanumericStrings(leftString, rightString, EComparisonType.LowerThan);
                    break;
                case EComparisonType.LowerThan:
                    result = IsGreaterOrLower(leftString, rightString, EComparisonType.LowerThan);
                    break;
            }

            return result;
        }

        /// <summary>
        /// Determines whether [is greater or lower] [the specified left string].
        /// </summary>
        /// <param name="leftString">The left string.</param>
        /// <param name="rightString">The right string.</param>
        /// <param name="lowerThan">The lower than.</param>
        /// <returns>
        ///   <c>true</c> if [is greater or lower] [the specified left string]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsGreaterOrLower(string leftString, string rightString, EComparisonType comparisonType)
        {
            if (leftString == null || rightString == null)
            {
                throw new FrameworkException("Values to be compared cannot be null! Left string = " + leftString + ", Right string = " + rightString);
            }

            var customAlphanumericOrder = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            int maxLength = Math.Max(leftString.Length, rightString.Length);

            leftString = leftString.PadRight(maxLength, '0').ToUpperInvariant();
            rightString = rightString.PadRight(maxLength, '0').ToUpperInvariant();

            for (int index = 0; index < maxLength; index++)
            {
                int leftOrderPosition = customAlphanumericOrder.IndexOf(leftString[index]);
                int rightOrderPosition = customAlphanumericOrder.IndexOf(rightString[index]);

                if (leftOrderPosition > rightOrderPosition)
                {
                    return comparisonType.Equals(EComparisonType.GreaterThan);
                }
                if (leftOrderPosition < rightOrderPosition)
                {
                    return comparisonType.Equals(EComparisonType.LowerThan);
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if empty list or different.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="received">The received.</param>
        /// <param name="expected">The expected.</param>
        /// <param name="type">The type.</param>
        /// <param name="error">The error.</param>
        /// <returns></returns>
        private static bool? CheckIfEmptyListOrDifferent<T>(T received, T expected, Type type, out string error)
        {
            error = string.Empty;
            IList resultList = received as IList;
            IList expectedList = expected as IList;

            if ((expected != null && received == null) || (resultList.Count != expectedList.Count))
            {
                error += "\nThe " + type.Name + " number of elements (" + resultList?.Count + ") does not match the expected (" + expectedList.Count + ")";
                return false;
            }
            if (resultList.Count == 0)
            {
                return true;
            }

            return null;
        }

        /// <summary>
        /// Checks if empty array or different.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="received">The received.</param>
        /// <param name="expected">The expected.</param>
        /// <param name="type">The type.</param>
        /// <param name="error">The error.</param>
        /// <returns></returns>
        private static bool? CheckIfEmptyArrayOrDifferent<T>(T received, T expected, Type type, out string error)
        {
            error = string.Empty;
            Array resultList = received as Array;
            Array expectedList = expected as Array;

            if ((expected != null && received == null) || (resultList.Length != expectedList.Length))
            {
                error += "\nThe " + type.Name + " number of elements (" + resultList?.Length + ") does not match the expected (" + expectedList.Length + ")";
                return false;
            }
            if (resultList.Length == 0)
            {
                return true;
            }

            return null;
        }

        /// <summary>
        /// Return a random int between two values
        /// </summary>
        /// <param name="min">the minimin value</param>
        /// <param name="max">the maximum value</param>
        /// <returns></returns>
        public static int RandomizeInt(int min, int max)
        {
            return rand.Next(min, max);
        }

        /// <summary>
        /// Randomizes the messages identifier.
        /// </summary>
        /// <returns></returns>
        public static string RandomizeMessagesID()
        {
            //Guid should contain 32 digits with 4 dashes (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx).
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Gets the enum description.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerationValue">The enumeration value.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">EnumerationValue must be of Enum type - enumerationValue</exception>
        public static string GetEnumDescription<T>(T enumerationValue)
        {
            Type type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
            }

            //Tries to find a DescriptionAttribute for a potential friendly name
            //for the enum
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    //Pull out the description value
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            //If we have no description attribute, just return the ToString of the enum
            return enumerationValue.ToString();
        }

        /// <summary>
        /////// Gets the name of the service.
        /////////// </summary>
        /// <param name="service">The service.</param>
        /// <returns></returns>
        public static string GetServiceName(string serviceUrl)
        {
            string[] serviceSplit = serviceUrl.Split('/');
            return serviceSplit[serviceSplit.Length - 1];
        }

        /// <summary>
        /// Determines whether this instance is any.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">The data.</param>
        /// <returns>
        ///   <c>true</c> if the specified data is any; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAny<T>(IEnumerable<T> data)
        {
            return data != null && data.Any();
        }

        /// <summary>
        /// Checks the equality or range.
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <param name="received">The received.</param>
        /// <returns></returns>
        public static bool CheckEqualityOrInRange(string expected, string received, string fieldName, out string error)
        {
            if (expected.IsRange())
            {
                return CheckRange(expected, received, fieldName, out error);
            }
            else
            {
                return CompareSimpleObjects(received, expected, fieldName, out error);
            }
        }

        /// <summary>
        /// Checks the range.
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <param name="received">The received.</param>
        /// <param name="error">The error.</param>
        /// <returns></returns>
        private static bool CheckRange(string expected, string received, string fieldName, out string error)
        {
            bool result = false;
            error = string.Empty;

            string minRange = Regex.Split(expected, "[,]")[0].Trim();
            string maxRange = Regex.Split(expected, "[,]")[1].Trim();

            bool minIncluded = minRange.Contains("[");
            bool maxIncluded = maxRange.Contains("]");

            string min = minRange.Substring(1);
            string max = maxRange.Substring(0, maxRange.Length - 1);

            if (min.IsDate())
            {
                DateTime minDateTime = DateTime.Parse(min);
                DateTime maxDateTime = DateTime.Parse(max);
                DateTime receivedDateTime = DateTime.Parse(received);

                result = minIncluded ? receivedDateTime >= minDateTime : receivedDateTime > minDateTime;
                result &= maxIncluded ? receivedDateTime <= maxDateTime : receivedDateTime < maxDateTime;
            }
            else
            {
                int minInt = int.Parse(min);
                int maxInt = int.Parse(max);
                int receivedInt = int.Parse(received);

                result = minIncluded ? receivedInt >= minInt : receivedInt > minInt;
                result &= maxIncluded ? receivedInt <= maxInt : receivedInt < maxInt;
            }

            if (!result)
            {
                error = "The received " + fieldName + " value: " + received + " is not in the range: " + expected;
            }

            return result;
        }

        public enum EParamType
        {
            None,
            QueryParam,
            PathParam,
            Param
        }

        public enum EFormat
        {
            XML,
            JSON,
            OTHERS
        }

        /// <summary>
        /// Gets the file extension.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public static BaseFileUtils.EFileExtension GetFileExtension(EFormat format)
        {
            BaseFileUtils.EFileExtension fileExtension = BaseFileUtils.EFileExtension.SQL;
            switch (format)
            {
                case EFormat.JSON:
                    fileExtension = BaseFileUtils.EFileExtension.JSON;
                    break;
                case EFormat.XML:
                    fileExtension = BaseFileUtils.EFileExtension.XML;
                    break;
                case EFormat.OTHERS:
                    fileExtension = BaseFileUtils.EFileExtension.TXT;
                    break;
            }

            return fileExtension;
        }

        /// <summary>
        /// Gets the parameter list as string.
        /// </summary>
        /// <param name="paramList">The parameter list.</param>
        /// <returns></returns>
        public static string GetParameterListAsString(IEnumerable<string> paramList, EParamType paramType)
        {
            if (!IsAny(paramList)) return string.Empty;

            var listAsString = new StringBuilder();
            bool isQueryParam = paramType.Equals(EParamType.QueryParam);
            string separator = string.Empty;
            if (!paramType.Equals(EParamType.Param))
            {
                separator = isQueryParam ? "?" : "/";
            }

            foreach (string param in paramList)
            {
                if (param == null) continue;
                separator = (!isQueryParam && param.StartsWith("/")) ? string.Empty : separator;
                listAsString.Append(separator + param);
                separator = isQueryParam ? "&" : "/";
                separator = paramType.Equals(EParamType.Param) ? Constants.StepTildeVariablesIndicatorChar.ToString() : separator;
            }
            return listAsString.ToString();
        }

        /// <summary>
        /// Gets the query parameter dictionary.
        /// </summary>
        /// <param name="queryParamString">The query parameter string.</param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> GetQueryParamDictionary(string queryParamString)
        {
            Dictionary<string, List<string>> queryParamDict = new Dictionary<string, List<string>>();

            if (!string.IsNullOrEmpty(queryParamString))
            {
                if (!queryParamString.StartsWith("?")) queryParamString = "?" + queryParamString;
                foreach (var item in queryParamString.Substring(1).Split('&').ToList())
                {
                    if (item.Contains("="))
                    {
                        string key = item.Split('=')[0];
                        string value = item.Split('=')[1];
                        if (queryParamDict.ContainsKey(key))
                        {
                            List<string> valueList = queryParamDict[key];
                            valueList.Add(value);
                            queryParamDict[key] = valueList;
                        }
                        else
                        {
                            queryParamDict.Add(key, new List<string>() { value });
                        }
                    }
                }
            }

            return queryParamDict;
        }
    }
}
