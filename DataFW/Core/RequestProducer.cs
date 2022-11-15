using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using Demo.CommonFramework.ExceptionHandler;
using Demo.CommonFramework.Helpers;
using Demo.DataUniverseFramework.Helpers;
using Demo.DataUniverseFramework.SQL;
using Demo.TestReport.Framework.Core;
using Newtonsoft.Json.Linq;
using TechTalk.SpecFlow;

namespace Demo.DataUniverseFramework
{
    public class RequestProducer
    {
        private readonly string m_ServerRequest;
        private readonly string m_ServerResponse;
        private Dictionary<string, string> m_queryValuesDict = null;
        private DataTable m_ResultParameters;
        private SqlConfig m_instanceRequest;
        private SqlConfig m_instanceResponse;
        public string ConnectionStringResponse => InstanceSQLConfigResponse.ConnectionString;
        public string ConnectionStringRequest => InstanceSQLConfigRequest.ConnectionString;
        public List<string> SqlFileList { get; private set; } = new List<string>();
        public ScenarioContext ScenarioContext { get; set; }
        public Dictionary<string, string> VariablesDict
        {
            get
            {
                return m_queryValuesDict ?? (m_queryValuesDict = new Dictionary<string, string>());
            }
            set
            {
                m_queryValuesDict = value;
            }
        }

        /// <summary>
        /// Gets or sets the request response provider.
        /// </summary>
        /// <value>
        /// The request response provider.
        /// </value>
        public EnvironmentRequest RequestResponseProvider { get; set; }

        /// <summary>
        /// Gets or sets the name of the action.
        /// </summary>
        /// <value>
        /// The name of the action.
        /// </value>
        public string ActionName { get; set; }

        /// <summary>
        /// Gets the instance SQL configuration request.
        /// </summary>
        /// <value>
        /// The instance SQL configuration request.
        /// </value>
        private SqlConfig InstanceSQLConfigRequest
        {
            get
            {
                m_instanceRequest = m_instanceRequest ?? new SqlConfig(m_ServerRequest);
                return m_instanceRequest;
            }
        }

        /// <summary>
        /// SQLs the configuration instance.
        /// </summary>
        /// <param name="isRequestServerConfig">if set to <c>true</c> [is request server configuration].</param>
        /// <returns></returns>
        public SqlConfig GetSqlConfig(bool isRequestServerConfig = true) => isRequestServerConfig ? InstanceSQLConfigRequest : InstanceSQLConfigResponse;

        /// <summary>
        /// Gets the instance SQL configuration response.
        /// </summary>
        /// <value>
        /// The instance SQL configuration response.
        /// </value>
        private SqlConfig InstanceSQLConfigResponse
        {
            get
            {
                if (string.IsNullOrEmpty(m_ServerResponse))
                {
                    m_instanceResponse = m_instanceRequest;
                }
                else
                {
                    m_instanceResponse = m_instanceResponse ?? new SqlConfig(m_ServerResponse);
                }

                return m_instanceResponse;
            }
        }

        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="service">name of the services to Test</param>
        /// <param name="serverRequest">The server request.</param>
        /// <param name="serverResponse">The server response.</param>
        public RequestProducer(string service, string serverRequest, string serverResponse = null)
        {
            RequestResponseProvider = new EnvironmentRequest();
            m_ServerRequest = serverRequest;
            m_ServerResponse = serverResponse;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestProducer"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="service">The service.</param>
        /// <param name="serverRequest">The server request.</param>
        /// <param name="serverResponse">The server response.</param>
        public RequestProducer(ScenarioContext context, string service, string serverRequest, string serverResponse = null)
        {
            RequestResponseProvider = new EnvironmentRequest();
            m_ServerRequest = serverRequest;
            m_ServerResponse = serverResponse;
            ScenarioContext = context;
        }

        /// <summary>
        /// manage the requests (SOAP) the send to the environment
        /// </summary>
        /// <param name="pathFile">The path file.</param>
        /// <param name="sqlAccess">The SQL access.</param>
        /// <param name="sqlPath">The SQL path.</param>
        public void GenerateEnvironmentSoapRequest(string pathFile, ISqlAccess sqlAccess, string sqlPath = null)
        {
            RequestResponseProvider.XmlRequest = GenerateEnvironmentRequest(pathFile, BaseUtils.EFormat.XML, sqlAccess, sqlPath: sqlPath);
        }

        /// <summary>
        /// Generates the enviroment parameter request.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="sqlPath">The SQL path.</param>
        /// <param name="sqlAccess">The SQL access.</param>
        public void GenerateEnvironmentParamRequest(IEnumerable<string> param, string sqlPath, ISqlAccess sqlAccess)
        {
            RequestResponseProvider.Param = GenerateEnvironmentRequest(sqlPath, BaseUtils.EFormat.OTHERS, sqlAccess
                , param, BaseUtils.EParamType.Param).Split(Constants.StepTildeVariablesIndicatorChar).ToList();
        }

        /// <summary>
        /// Modifies the environment date.
        /// </summary>
        /// <param name="keyValue">The key value.</param>
        /// <param name="sqlPath">The SQL path.</param>
        /// <param name="sqlAccess">The SQL access.</param>
        /// <returns></returns>
        public bool ModifyEnvironmentData(Dictionary<string, string> keyValue, string sqlPath, ISqlAccess sqlAccess, bool isStatement = true)
        {
            if (isStatement)
            {
                string sqlquery = BaseFileUtils.GetSQLFileContent(sqlPath);
                Common.ReplaceParametersByFunctions(ref keyValue, ref sqlquery, sqlAccess.Config, null);

                return SqlHelper.RunStatementIntoDataBase(sqlquery, sqlAccess) > 0;
            }
            else
            {
                UpdateParameters(sqlPath, sqlAccess);
                UpdateVariablesDict(BaseUtils.EFormat.OTHERS);
                return true;
            }
        }

        /// <summary>
        /// Modifies the environment data.
        /// </summary>
        /// <param name="sqlPath">The SQL path.</param>
        /// <param name="sqlAccess">The SQL access.</param>
        /// <returns></returns>
        public bool ModifyEnvironmentData(string sqlPath, ISqlAccess sqlAccess, bool isStatement = true)
        {
            MergeWithScenarioContext();
            return ModifyEnvironmentData(m_queryValuesDict, sqlPath, sqlAccess, isStatement);
        }

        /// <summary>
        /// Merges the with scenario context.
        /// </summary>
        /// <param name="scenarioContext">The scenario context.</param>
        /// <param name="variablesDict">The variables dictionary.</param>
        private void MergeWithScenarioContext()
        {
            Dictionary<string, string> contextVariables = ScenarioContext?.Where(item => item.Key.StartsWith(Constants.StepVariablesIndicatorChar.ToString()))
                                                                          .ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.ToString()) ?? new Dictionary<string, string>();
            VariablesDict = VariablesDict.Where(item => item.Key.StartsWith(Constants.StepVariablesIndicatorChar.ToString()) || item.Key.StartsWith(Constants.StepHashVariablesIndicatorChar.ToString()))
                                                                          .ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.ToString()) ?? new Dictionary<string, string>();
            VariablesDict = VariablesDict.Union(contextVariables)
                               .GroupBy(i => i.Key).ToDictionary(group => group.Key, group => group.First().Value);
        }

        /// <summary>
        /// manage the requests (QueryParam) the send to the environment
        /// </summary>
        /// <param name="queryParams">The query parameters.</param>
        /// <param name="sqlPath">The SQL path.</param>
        /// <param name="sqlAccess">The SQL access.</param>
        public void GenerateEnvironmentQueryParamRequest(IEnumerable<string> queryParams, string sqlPath, ISqlAccess sqlAccess)
        {
            RequestResponseProvider.QueryParam = GenerateEnvironmentRequest(sqlPath, BaseUtils.EFormat.JSON, sqlAccess
                , queryParams, BaseUtils.EParamType.QueryParam);
            RequestResponseProvider.QueryParam = BaseUtils.GetParameterListAsString(queryParams, BaseUtils.EParamType.QueryParam);
        }

        /// <summary>
        /// manage the requests (pathParam) the send to the environment
        /// </summary>
        /// <param name="pathParams">The path parameters.</param>
        /// <param name="sqlPath">The SQL path.</param>
        /// <param name="sqlAccess">The SQL access.</param>
        public void GenerateEnvironmentPathParamRequest(IEnumerable<string> pathParams, string sqlPath, ISqlAccess sqlAccess)
        {
            RequestResponseProvider.PathParam = GenerateEnvironmentRequest(sqlPath, BaseUtils.EFormat.JSON, sqlAccess
                , pathParams, BaseUtils.EParamType.PathParam);
        }

        /// <summary>
        /// manage the requests (JSON) the send to the environment
        /// </summary>
        /// <param name="pathFile">The path file.</param>
        /// <param name="sqlAccess">The SQL access.</param>
        /// <param name="isExpected">if set to <c>true</c> [is expected].</param>
        public void GenerateEnvironmentJsonRequest(string pathFile, ISqlAccess sqlAccess, bool isExpected = false)
        {
            RequestResponseProvider.JsonRequest = GenerateEnvironmentRequest(pathFile, BaseUtils.EFormat.JSON, sqlAccess);
            if (isExpected)
            {
                BaseFileUtils.AttachFileToLog(RequestResponseProvider.JsonRequest, Path.GetFileNameWithoutExtension(pathFile)
                                              , BaseFileUtils.EStreamType.Expected, BaseFileUtils.EFileExtension.JSON, "JSON Request");
            }
        }

        /// <summary>
        /// manage the response the send to the environment
        /// </summary>
        /// <param name="pathFile">The path file.</param>
        /// <param name="sqlAccess">The SQL access.</param>
        /// <param name="sqlPath">The SQL path.</param>
        public void GenerateEnvironmentSoapResponse(string pathFile, ISqlAccess sqlAccess, string sqlPath = null)
        {
            GenerateEnvironmentResponse(pathFile, BaseUtils.EFormat.XML, sqlAccess, sqlPath);
        }

        /// <summary>
        /// manage the response the send to the environment
        /// </summary>
        /// <param name="pathFile">The path file.</param>
        /// <param name="sqlAccess">The SQL access.</param>
        /// <param name="sqlPath">The SQL path.</param>
        public void GenerateEnvironmentJsonResponse(string pathFile, ISqlAccess sqlAccess, string sqlPath = null)
        {
            GenerateEnvironmentResponse(pathFile, BaseUtils.EFormat.JSON, sqlAccess, sqlPath);
        }

        /// <summary>
        /// Updates the parameters.
        /// </summary>
        /// <param name="sqlPath">The path file.</param>
        /// <param name="sqlAccess">The SQL access.</param>
        private void UpdateParameters(string sqlPath, ISqlAccess sqlAccess)
        {
            if (!string.IsNullOrEmpty(sqlPath) && m_ResultParameters == null || (!SqlFileList.Contains(sqlPath)))
            {
                MergeWithScenarioContext();
                SqlHelper.GetParametersFromDataBase(ref m_queryValuesDict, sqlPath, sqlAccess, out m_ResultParameters);
                if (!string.IsNullOrEmpty(sqlPath))
                {
                    SqlFileList.Add(sqlPath);
                }
            }
        }

        /// <summary>
        /// Generates the environment request.
        /// </summary>
        /// <param name="pathFile">The path file.</param>
        /// <param name="format">The format.</param>
        /// <param name="sqlAccess">The SQL access.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="param">The parameter.</param>
        /// <param name="sqlPath">The SQL path.</param>
        /// <returns></returns>
        private string GenerateEnvironmentRequest(string pathFile, BaseUtils.EFormat format, ISqlAccess sqlAccess, IEnumerable<string> parameters = null
            , BaseUtils.EParamType param = BaseUtils.EParamType.None, string sqlPath = null)
        {
            sqlPath = string.IsNullOrEmpty(sqlPath) ? pathFile : sqlPath;

            UpdateParameters(sqlPath, sqlAccess);

            return (parameters == null) ? ReplaceTemplatesWithParameters(BaseFileUtils.GetFileContent(pathFile), format, sqlAccess)
                                        : ReplaceTemplatesWithParameters(BaseUtils.GetParameterListAsString(parameters, param), format, sqlAccess);
        }

        /// <summary>
        /// Generates the environment response.
        /// </summary>
        /// <param name="pathFile">The path file.</param>
        /// <param name="format">The format.</param>
        /// <param name="sqlAccess">The SQL access.</param>
        /// <param name="sqlPath">The SQL path.</param>
        private void GenerateEnvironmentResponse(string pathFile, BaseUtils.EFormat format, ISqlAccess sqlAccess, string sqlPath = null)
        {
            sqlPath = string.IsNullOrEmpty(sqlPath) ? pathFile : sqlPath;
            UpdateParameters(sqlPath, sqlAccess);
            RequestResponseProvider.Response = ReplaceTemplatesWithParameters(BaseFileUtils.GetFileContent(pathFile), format, sqlAccess);

            string fileName = Path.GetFileNameWithoutExtension(pathFile).Replace("Template", "");
            BaseFileUtils.AttachFileToLog(RequestResponseProvider.Response, fileName, BaseFileUtils.EStreamType.Expected, BaseUtils.GetFileExtension(format), "Expected response");
        }

        /// <summary>
        /// Gets the parameterized queries from template.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="sqlAccess">The SQL access.</param>
        /// <exception cref="InconclusiveFrameworkException">The template doesn’t have a pretty standard format please review the QUERY TABS and blank space or tabs. Templates: " + template</exception>
        private void GetParameterizedQueriesFromTemplate(ref string template, ISqlAccess sqlAccess)
        {
            if (!Regex.IsMatch(template, Constants.QUERY + @"(\d)*")) return;

            Dictionary<string, string> listQueries = new Dictionary<string, string>();

            if (BaseXmlHelper.IsValidXml(template))
            {
                GetParamQueriesFromXmlTemplate(ref listQueries, ref template, sqlAccess);
            }
            else if (BaseJsonHelper.IsValidJson(template))
            {
                GetParamQueriesFromJsonTemplate(ref listQueries, ref template, sqlAccess);
            }
            else
            {
                ExtentTestManager.LogWarning("Sending a file that is not a valid XML/JSON file");
            }

            Common.ReplaceParametersByValues(listQueries, ref template);
            //Review no more queries in the templates
            if (Regex.IsMatch(template, Constants.QUERY + @"(\d)*"))
            {
                throw new InconclusiveFrameworkException("The template doesn’t have a pretty standard format please review the QUERY TABS and blank space or tabs. Templates: " + template);
            }
        }

        /// <summary>
        /// Gets the parameter queries from json template.
        /// </summary>
        /// <param name="listQueries">The list queries.</param>
        /// <param name="template">The template.</param>
        /// <param name="sqlAccess">The SQL access.</param>
        /// <exception cref="InconclusiveFrameworkException">The following Query is mandatory and doesn't have results: " + token.Value<string>()</exception>
        private void GetParamQueriesFromJsonTemplate(ref Dictionary<string, string> listQueries, ref string template, ISqlAccess sqlAccess)
        {
            JObject jObject = JObject.Parse(template);
            List<JToken> tokenList = new List<JToken>();

            var matchCollection = Regex.Matches(template, Constants.QUERY + @"(" + Constants.MANDATORY + @")*(\d)*").OfType<Match>().Select(m => m.Value).Distinct();

            foreach (string match in matchCollection)
            {
                tokenList.AddRange(jObject.SelectTokens("$.." + match).ToList());
            }

            foreach (JToken token in tokenList)
            {
                string queryResult = SqlHelper.GetQueryFromDataBase(token.Value<string>(), sqlAccess);
                string key = token.Parent.ToString();
                bool mandatory = key.Contains(Constants.QUERY + Constants.MANDATORY);

                if (string.IsNullOrEmpty(queryResult) && mandatory)
                {
                    throw new InconclusiveFrameworkException("The following Query is mandatory and doesn't have results: " + token.Value<string>());
                }
                else if (string.IsNullOrEmpty(queryResult))
                {
                    listQueries[key] = string.Empty;
                }
                else
                {
                    XmlDocument resultXml = new XmlDocument();

                    if (!BaseXmlHelper.IsValidXml(queryResult))
                    {
                        queryResult = BaseXmlHelper.InsertRootElement(queryResult);
                    }

                    resultXml.LoadXml(queryResult);
                    listQueries[key] = BaseJsonHelper.GetJSONPortionToInsert(BaseJsonHelper.XMLtoJsonWithOutRootElement(resultXml));
                }
            }
        }

        /// <summary>
        /// Gets the parameter queries from XML template.
        /// </summary>
        /// <param name="listQueries">The list queries.</param>
        /// <param name="template">The template.</param>
        private void GetParamQueriesFromXmlTemplate(ref Dictionary<string, string> listQueries, ref string template, ISqlAccess sqlAccess)
        {
            XmlNodeList nodeListQueries = BaseXmlHelper.GetElementsByTagName(Constants.QUERY, template);
            foreach (XmlNode node in nodeListQueries)
            {
                bool mandatoryResult = false;
                XmlNode mandatoryAttr = node.Attributes.GetNamedItem(XmlHelper.ATT_MANDATORY_RESULT);
                if (mandatoryAttr != null && mandatoryAttr.HasChildNodes)
                {
                    bool.TryParse(mandatoryAttr.FirstChild.Value, out mandatoryResult);
                }

                string key = XmlHelper.QUERY_TAG_START + node.InnerText + XmlHelper.QUERY_TAG_END;
                listQueries[key] = SqlHelper.GetQueryFromDataBase(node.InnerText, sqlAccess, mandatoryResult);
            }
            BaseXmlHelper.RemoveAttributes(Constants.QUERY, ref template);
        }

        /// <summary>
        /// Gets the values from SQL file column.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="pathFile">The path file.</param>
        /// <param name="sqlAccess">The SQL access.</param>
        /// <returns></returns>
        /// <exception cref="FrameworkException">The column " + columnName + " has not been found in the query of the SQL '" + pathFile + "'</exception>
        public List<string> GetValuesFromSQLFileColumn(string columnName, string pathFile, ISqlAccess sqlAccess)
        {
            GetDataTable(pathFile, sqlAccess, out DataTable result);
            int columnIndex = result.Columns.IndexOf(columnName);

            if (columnIndex < 0)
            {
                throw new FrameworkException("The column " + columnName + " has not been found in the query of the SQL '" + pathFile + "'");
            }

            List<string> resultList = new List<string>();

            foreach (DataRow row in result.Rows)
            {
                resultList.Add(row[columnIndex].ToString());
            }

            return resultList;
        }

        /// <summary>
        /// Gets the columns from SQL file.
        /// </summary>
        /// <param name="sqlPath">The SQL path.</param>
        /// <param name="sqlAccess">The SQL access.</param>
        /// <returns></returns>
        public List<string> GetColumnsFromSQLFile(string sqlPath, ISqlAccess sqlAccess)
        {
            GetDataTable(sqlPath, sqlAccess, out DataTable result);
            List<string> columns = new List<string>();

            foreach (DataColumn column in result.Columns)
            {
                columns.Add(column.ColumnName);
            }

            return columns;
        }

        /// <summary>
        /// Gets the data table.
        /// </summary>
        /// <param name="sqlPath">The SQL path.</param>
        /// <param name="sqlAccess">The SQL access.</param>
        /// <param name="result">The result.</param>
        private void GetDataTable(string sqlPath, ISqlAccess sqlAccess, out DataTable result)
        {
            MergeWithScenarioContext();
            SqlHelper.GetParametersFromDataBase(ref m_queryValuesDict, sqlPath, sqlAccess, out result);
            SqlFileList.Add(sqlPath);
        }

        /// <summary>
        /// Replaces the templates with parameters.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="fileOutPutFormat">The file out put format.</param>
        /// <param name="sqlAccess">The SQL access.</param>
        /// <returns></returns>
        private string ReplaceTemplatesWithParameters(string template, BaseUtils.EFormat fileOutPutFormat, ISqlAccess sqlAccess)
        {
            string parameterizedTemplate = template;
            MergeWithScenarioContext();

            UpdateVariablesDict(fileOutPutFormat);

            Common.ReplaceParametersByValues(VariablesDict, ref parameterizedTemplate);
            Common.ReplaceParametersByFunctions(ref m_queryValuesDict, ref parameterizedTemplate, sqlAccess.Config, InstanceSQLConfigResponse);
            GetParameterizedQueriesFromTemplate(ref parameterizedTemplate, sqlAccess);

            if (fileOutPutFormat == BaseUtils.EFormat.JSON)
            {
                XmlDocument doc = new XmlDocument();
                if (BaseXmlHelper.IsValidXml(parameterizedTemplate))
                {
                    doc.LoadXml(parameterizedTemplate);
                    parameterizedTemplate = BaseJsonHelper.XMLtoJson(doc);
                }
            }

            return parameterizedTemplate;
        }

        /// <summary>
        /// Updates the variables dictionary.
        /// </summary>
        private void UpdateVariablesDict(BaseUtils.EFormat fileOutPutFormat)
        {
            foreach (DataRow row in m_ResultParameters.Rows)
            {
                foreach (DataColumn column in m_ResultParameters.Columns)
                {
                    if (!VariablesDict.ContainsKey(column.ColumnName))
                    {
                        if (fileOutPutFormat == BaseUtils.EFormat.XML)
                        {
                            VariablesDict.Add(column.ColumnName, row[column].ToString().Replace("&", "&amp;"));
                        }
                        else if (fileOutPutFormat == BaseUtils.EFormat.JSON)
                        {
                            VariablesDict.Add(column.ColumnName, (row[column] is DBNull) ? null : row[column].ToString());
                        }
                        else
                        {
                            VariablesDict.Add(column.ColumnName, row[column].ToString());
                        }
                        if (ScenarioContext != null && !ScenarioContext.ContainsKey(column.ColumnName))
                        {
                            ScenarioContext?.Add(column.ColumnName, VariablesDict[column.ColumnName]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds the ordered query values.
        /// </summary>
        /// <param name="variables">The variables.</param>
        /// <param name="keyValues">The key values.</param>
        /// <returns>the query values dictionary</returns>
        /// <exception cref="FrameworkException">The number of variables does not match with the numbers of values requested!</exception>
        public Dictionary<string, string> AddOrderedQueryValues(IEnumerable<string> variables, IDictionary<string, string> keyValues)
        {
            if (variables.ToList().Count != keyValues.Values.Count)
            {
                throw new FrameworkException("The number of variables does not match with the numbers of values requested!");
            }

            int i = 0;
            foreach (var item in variables)
            {
                VariablesDict[item] = keyValues.Values.ToList()[i++];
                if (ScenarioContext != null)
                {
                    ScenarioContext[item] = VariablesDict[item];
                }
            }

            return VariablesDict;
        }

        /// <summary>
        /// Deletes the source system.
        /// </summary>
        /// <param name="sourceSystem">The source system.</param>
        /// <param name="sqlAccess">The SQL access.</param>
        /// <returns></returns>
        public bool DeleteSourceSystem(string sourceSystem, ISqlAccess sqlAccess = null)
        {
            return SqlHelper.DeleteSourceSystem(sqlAccess ?? new SqlAccess(InstanceSQLConfigResponse), sourceSystem);
        }

        /// <summary>
        /// Inserts the new source system.
        /// </summary>
        /// <param name="sourceSystem">The source system.</param>
        public bool InsertNewSourceSystem(string sourceSystem, ISqlAccess sqlAccess = null)
        {
            return SqlHelper.InsertNewSourceSystem(sqlAccess ?? new SqlAccess(InstanceSQLConfigResponse), sourceSystem);
        }

        /// <summary>
        /// Removes the flow identifier entries by source system.
        /// </summary>
        /// <param name="sourceSystem">The source system.</param>
        /// <param name="sqlAccess">The SQL access.</param>
        public bool RemoveFlowIdEntriesBySourceSystem(string sourceSystem, ISqlAccess sqlAccess = null)
        {
            return SqlHelper.RemoveFlowIdEntriesBySourceSystem(sqlAccess ?? new SqlAccess(InstanceSQLConfigResponse), sourceSystem);
        }

        /// <summary>
        /// Waits the until database is updated.
        /// </summary>
        /// <param name="sqlPath">The SQL path.</param>
        /// <param name="timeSpan">The time span.</param>
        /// <param name="sqlAccess">The SQL access.</param>
        public bool WaitUntilDBIsUpdated(string sqlPath, TimeSpan timeSpan, ISqlAccess sqlAccess)
        {
            DataTable result = null;
            Dictionary<string, string> tempDict = new Dictionary<string, string>();
            MergeWithScenarioContext();
            foreach (var item in VariablesDict)
            {
                tempDict[item.Key] = item.Value;
            }
            string sqlquery = BaseFileUtils.GetSQLFileContent(sqlPath);
            if (string.IsNullOrEmpty(sqlquery)) return true;
            Common.ReplaceParametersByFunctions(ref tempDict, ref sqlquery, sqlAccess.Config, null);

            Stopwatch watch = new Stopwatch();
            watch.Start();
            do
            {
                result = sqlAccess.ExecuteQuery(sqlAccess.GetDataAdapter(sqlAccess.CreateConnection(sqlquery)));
            }
            while (result.Rows.Count == 0 && watch.ElapsedMilliseconds <= timeSpan.TotalMilliseconds);
            watch.Stop();

            return result.Rows.Count > 0;
        }
    }
}
