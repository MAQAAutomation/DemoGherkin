using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Demo.CommonFramework.Helpers;
using Demo.DataUniverseFramework.Helpers;
using Demo.DataUniverseFramework.SQL;

namespace Demo.DataUniverseFramework
{
    public static class Common
    {
        private static readonly List<string> m_functionsReplace = new List<string>
        {
            Constants.CURRENT_DATE
            , Constants.RANDOM_STRING
            , Constants.PAST_DATE
            , Constants.FUTURE_DATE
            , Constants.RANDOM_NUMBER
            , Constants.NEW_MESSAGE_ID
            , Constants.NEW_CORRELATION_ID
            , Constants.NEW_CONVERSATION_ID
            , Constants.RANDOM_ALPHA_NUMERIC
            , Constants.NEW_FLOW_ID
            , Constants.NEW_FLOW_ID_SECTION
            , Constants.NEW_COMMAND_ID
            , Constants.CAMELCASE_PROPERTIES_CONTRACT
        };

        /// <summary>
        /// Replace List of keys
        /// </summary>
        /// <param name="keyValueList">List key-value </param>
        /// <param name="templatesRequest">String with XML template</param>
        public static void ReplaceParametersByValues(Dictionary <string, string> keyValueList, ref string templatesRequest)
        {
            foreach (KeyValuePair<string, string> value in keyValueList)
            {
                if (templatesRequest.Contains(value.Key) && !Regex.IsMatch(templatesRequest, @"[aA]+[sS]+[ ]*[']*" + value.Key.Replace("$", "\\$")))
                {
                    if(value.Value == null)
                    {
                        templatesRequest = templatesRequest.Replace(value.Key, "#isNull");
                    }
                    else
                    {
                        templatesRequest = templatesRequest.Replace(value.Key, value.Value);
                    }
                }
                else if (value.Key.Equals(Constants.CAMELCASE_PROPERTIES_CONTRACT))
                {
                    templatesRequest = Regex.Replace(templatesRequest, Constants.CAMELCASE_PROPERTIES_CONTRACT, "");
                    templatesRequest = templatesRequest.Replace("#CAMELCASEPROPERTIESCONTRACT##", value.Value);
                }
            }
        }

        /// <summary>
        /// Replace method call to values
        /// </summary>
        /// <param name="templatesRequest">String with XML template</param>
        /// <param name="sqlConfig">The SQL configuration.</param>
        /// <param name="sqlConfigFlowID">The SQL configuration flow identifier.</param>
        public static void ReplaceParametersByFunctions(ref Dictionary<string, string> keyValueList, ref string templatesRequest, SqlConfig sqlConfig, SqlConfig sqlConfigFlowID)
        {
            foreach (string value in m_functionsReplace)
            {
                if (Regex.IsMatch(templatesRequest, value, RegexOptions.IgnoreCase ) && !keyValueList.ContainsKey(value))
                {
                    switch (value)
                    {
                        case Constants.CURRENT_DATE:
                            keyValueList.Add(Constants.CURRENT_DATE, BaseUtils.CurrentDate().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
                            break;
                        case Constants.RANDOM_STRING:
                            foreach (Match match in Regex.Matches(templatesRequest, value))
                            {
                                if (!keyValueList.ContainsKey(match.Value))
                                { 
                                    keyValueList.Add(match.Value, BaseUtils.RandomizeString(int.Parse(Regex.Match(match.Value, @"\d+").Value)).ToString());
                                }
                            }
                            break;
                        case Constants.PAST_DATE:
                            keyValueList.Add(Constants.PAST_DATE, BaseUtils.RandomizeDate(false).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
                            break;
                        case Constants.FUTURE_DATE:
                            keyValueList.Add(Constants.FUTURE_DATE, BaseUtils.RandomizeDate(true).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
                            break;
                        case Constants.RANDOM_NUMBER:
                            foreach (Match match in Regex.Matches(templatesRequest, value))
                            {
                                if (!keyValueList.ContainsKey(match.Value))
                                {
                                    keyValueList.Add(match.Value, BaseUtils.RandomizeInt(1, int.Parse(Regex.Match(match.Value, @"\d+").Value)).ToString());
                                }
                            }
                            break;
                        case Constants.NEW_MESSAGE_ID:
                        case Constants.NEW_CORRELATION_ID:
                        case Constants.NEW_CONVERSATION_ID:
                        case Constants.NEW_COMMAND_ID:
                            keyValueList.Add(value, BaseUtils.RandomizeMessagesID());
                            break;
                        case Constants.RANDOM_ALPHA_NUMERIC:
                            foreach (Match match in Regex.Matches(templatesRequest, value))
                            {
                                if (!keyValueList.ContainsKey(match.Value))
                                { 
                                    keyValueList.Add(match.Value, BaseUtils.RandomizeAlfanumericString(int.Parse(Regex.Match(match.Value, @"\d+").Value)).ToString());
                                }
                            }
                            break;
                        case Constants.NEW_FLOW_ID:
                            keyValueList.Add(Constants.NEW_FLOW_ID, SqlHelper.GetPolicyFlowID(new SqlAccess(sqlConfigFlowID)).ToString());
                            break;
                        case Constants.NEW_FLOW_ID_SECTION:
                            foreach (Match match in Regex.Matches(templatesRequest, value))
                            {
                                if (!keyValueList.ContainsKey(match.Value))
                                {
                                    keyValueList.Add(match.Value, SqlHelper.GetPolicyFlowID(new SqlAccess(sqlConfigFlowID), uint.Parse(Regex.Match(match.Value, @"\d+").Value)).ToString());
                                }
                            }
                            break;
                        case Constants.CAMELCASE_PROPERTIES_CONTRACT:
                             foreach (Match match in Regex.Matches(templatesRequest, value))
                            {
                                if (!keyValueList.ContainsKey(match.Value))
                                {
                                    keyValueList.Add(Constants.CAMELCASE_PROPERTIES_CONTRACT, BaseJsonHelper.CamelCasePropertyNamesContract(match.Value).ToString());
                                }
                            }
                            break;
                    }
                }
            }

            ReplaceParametersByValues(keyValueList, ref templatesRequest);
        }
    }
}
