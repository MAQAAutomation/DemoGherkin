using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using Demo.CommonFramework.Config;
using Demo.CommonFramework.ExceptionHandler;
using Demo.CommonFramework.Helpers;
using Demo.DataUniverseFramework.SQL;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace Demo.DataUniverseFramework.Helpers
{
    public static class SqlHelper
    {
        private static List<uint> m_flowIdsInUse = new List<uint>();

        /// <summary>
        /// Gets the parameters from data base.
        /// </summary>
        /// <param name="functionsDict">The functions dictionary.</param>
        /// <param name="pathFile">The path file.</param>
        /// <param name="sqlAccess">The SQL access.</param>
        /// <param name="result">The result.</param>
        /// <exception cref="InconclusiveFrameworkException"></exception>
        public static void GetParametersFromDataBase(ref Dictionary<string,string> functionsDict, string pathFile, ISqlAccess sqlAccess, out DataTable result)
        {
            string sqlQuery = BaseFileUtils.GetSQLFileContent(pathFile);
            result = new DataTable();
            if (!string.IsNullOrEmpty(sqlQuery))
            {
                Common.ReplaceParametersByFunctions(ref functionsDict, ref sqlQuery, sqlAccess.Config, null);
                result = sqlAccess.ExecuteQuery(sqlAccess.GetDataAdapter(sqlAccess.CreateConnection(sqlQuery)));

                if (result.Rows.Count == 0)
                {
                    throw new InconclusiveFrameworkException(string.Format("The environment to test doesn't have minimal data into the data Base to finish the Test. " +
                    "The following query, whose result is mandatory to finish the test, returns NO DATA. SQL: {0}", sqlQuery));
                }
            }
        }

        /// <summary>
        /// Gets the query from data base.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="sqlAccess">The SQL access.</param>
        /// <param name="mandatoryResult">if set to <c>true</c> [mandatory result].</param>
        /// <returns></returns>
        /// <exception cref="InconclusiveFrameworkException"></exception>
        public static string GetQueryFromDataBase(string query, ISqlAccess sqlAccess, bool mandatoryResult = false)
        {
            string result = string.IsNullOrEmpty(query) ? string.Empty : sqlAccess.ExecuteQueryStringResult(sqlAccess.CreateConnection(query));

            if (string.IsNullOrEmpty(result) && mandatoryResult)
            {
                throw new InconclusiveFrameworkException(string.Format("The environment to test doesn't have minimal data into the data Base to finish the Test. " +
                    "The following query, whose result is mandatory to finish the test, return NO DATA. SQL:{0}", query));
            }

            return result;
        }

        /// <summary>
        /// Runs the statement into data base.
        /// </summary>
        /// <param name="statement">The statement.</param>
        /// <param name="sqlAccess">The SQL access.</param>
        /// <returns></returns>
        public static int RunStatementIntoDataBase(string statement, ISqlAccess sqlAccess)
        {
            return sqlAccess.ExecuteStatement(sqlAccess.CreateConnection(statement));
        }

        /// <summary>
        /// Gets the message bus main table.
        /// </summary>
        /// <param name="sqlAccess">The SQL access.</param>
        /// <returns></returns>
        private static string GetMessageBusMainTable(ISqlAccess sqlAccess)
        {
            string mainTable = "messagebus";
            try
            {
                string sqlQuery = "SELECT * FROM " + mainTable + ".[ExternalSystem]";
                sqlAccess.ExecuteQuery(sqlAccess.GetDataAdapter(sqlAccess.CreateConnection(sqlQuery)));
            }
            catch
            {
                mainTable = "dbo";
            }

            return mainTable;
        }

        /// <summary>
        /// Gets the policy flow identifier.
        /// </summary>
        /// <param name="sqlAccess">The SQL access.</param>
        /// <param name="sum">The sum.</param>
        /// <returns></returns>
        public static uint GetPolicyFlowID(ISqlAccess sqlAccess, uint sum = 0)
        {
            if (sqlAccess == null || sqlAccess.Config == null) return 0;

            string mainTable = GetMessageBusMainTable(sqlAccess);

            string query = "SELECT TOP 1 CAST(esm.ExternalRecordId AS INT) AS ExternalRecordId"
                            + " FROM " + mainTable + ".[ExternalSystemMapping] esm"
                            + " INNER JOIN " + mainTable + ".[EclipseRecordType] ert ON esm.EclipseRecordTypeId = ert.EclipseRecordTypeId AND ert.SysValue = 'Policy' AND esm.Del = 0 AND ert.Del = 0"
                            + " INNER JOIN " + mainTable + ".[ExternalSystem] es ON esm.ExternalSystemId = es.ExternalSystemId"
                            + " order by ExternalRecordId desc";

            int randSum = BaseUtils.RandomizeInt(1, 9999);
            var flowId = uint.Parse(sqlAccess.ExecuteQueryStringResult(sqlAccess.CreateConnection(query))) + randSum + sum;
            uint result = (uint)((m_flowIdsInUse.Count == 0) ? flowId : Math.Max(flowId, m_flowIdsInUse.Max() + randSum + sum));
            m_flowIdsInUse.Add(result);

            return result;
        }

        /// <summary>
        /// Removes the entries by source system.
        /// </summary>
        /// <param name="sqlAccess">The SQL access.</param>
        /// <param name="sourceSystem">The source system.</param>
        /// <returns></returns>
        public static bool RemoveFlowIdEntriesBySourceSystem(ISqlAccess sqlAccess, string sourceSystem)
        {
            // If the sourceSystem is not for testing purposes we are not going to delete any row
            if (!sourceSystem.ToLower().Contains("test")) return false;

            string mainTable = GetMessageBusMainTable(sqlAccess);
            string query = "SELECT es.SysValue AS OriginatingSourceSystem, esm.EclipseRecordId AS EclipseRecordId, esm.ExternalRecordId AS ExternalRecordId"
                            + " FROM " + mainTable + ".[ExternalSystemMapping] esm"
                            + " INNER JOIN " + mainTable + ".[EclipseRecordType] ert ON esm.EclipseRecordTypeId = ert.EclipseRecordTypeId AND esm.Del = 0 AND ert.Del = 0"
                            + " INNER JOIN " + mainTable + ".[ExternalSystem] es ON esm.ExternalSystemId = es.ExternalSystemId"
                            + " WHERE es.SysValue = '" + sourceSystem + "'";

            DataTable sourceSystemDataTable = sqlAccess.ExecuteQuery(sqlAccess.GetDataAdapter(sqlAccess.CreateConnection(query)));

            List<string> flowIdList = (from DataRow dr in sourceSystemDataTable.Rows
                                       select (string)dr["ExternalRecordId"]).ToList();

            // Delete all flowId from the list into the [ExternalSystemMapping] table
            if (flowIdList.Count > 0)
            {
                string deleteQuery = "BEGIN";
                foreach (var flowId in flowIdList)
                {
                    deleteQuery += " DELETE FROM " + mainTable + ".[ExternalSystemMapping]"
                                        + " WHERE ExternalRecordId = '" + flowId + "'";


                }
                deleteQuery += " END";

                return sqlAccess.ExecuteStatement(sqlAccess.CreateConnection(deleteQuery)) > 0;
            }

            return true;
        }

        /// <summary>
        /// Inserts the new source system.
        /// </summary>
        /// <param name="sqlAccess">The SQL access.</param>
        /// <param name="sourceSystem">The source system.</param>
        /// <returns></returns>
        /// <exception cref="FrameworkException">The sql configuration is not well defined and the source system cannot be inserted!</exception>
        public static bool InsertNewSourceSystem(ISqlAccess sqlAccess, string sourceSystem)
        {
            if (sqlAccess == null || sqlAccess.Config == null) throw new FrameworkException("The sql configuration is not well defined and the source system cannot be inserted!");

            string mainTable = GetMessageBusMainTable(sqlAccess);

            string insertQuery = string.Empty;

            switch (RunSettings.EnvironmentConfiguration.ToLower().Replace("_", "").Replace("lloyds", ""))
            {
                case "vanillauw":
                case "impact":
                case "riskpoint":
                    insertQuery = "INSERT INTO " + mainTable + ".[ExternalSystem](SysValue, Description, UpdDate, CreatedDate)"
                                 + " VALUES('" + sourceSystem + "', 'Test System', GETDATE(), GETDATE())";
                    break;
                case "vanillamga":
                    insertQuery = "INSERT INTO " + mainTable + ".[ExternalSystem](Value, Description, UpdateUser, UpdateDate, CreatedDate)"
                                 + " VALUES('" + sourceSystem + "', 'Test System', 'Test User', GETDATE(), GETDATE())";
                    break;
                default:
                    throw new FrameworkException(RunSettings.EnvironmentConfiguration + " Configuration is not supported");
            }

            try
            {
                return sqlAccess.ExecuteStatement(sqlAccess.CreateConnection(insertQuery)) > 0;
            }
            catch(Exception e)
            {
                if (!e.Message.ToLower().Contains("duplicate key")) throw e;
                // Do nothing if it is already inserted
                return true;
            }
        }

        /// <summary>
        /// Deletes the source system.
        /// </summary>
        /// <param name="sqlAccess">The SQL access.</param>
        /// <param name="sourceSystem">The source system.</param>
        /// <returns></returns>
        /// <exception cref="FrameworkException">The sql configuration is not well defined and the source system cannot be deleted!</exception>
        public static bool DeleteSourceSystem(ISqlAccess sqlAccess, string sourceSystem)
        {
            if (sqlAccess == null || sqlAccess.Config == null) throw new FrameworkException("The sql configuration is not well defined and the source system cannot be deleted!");

            string mainTable = GetMessageBusMainTable(sqlAccess);

            string deleteQuery = "DELETE FROM " + mainTable + ".[ExternalSystem] WHERE SysValue = '" + sourceSystem + "'";
            return sqlAccess.ExecuteStatement(sqlAccess.CreateConnection(deleteQuery)) > 0;
        }

        /// <summary>
        /// Gets the statement from file.
        /// </summary>
        /// <param name="sqlPathFile">The SQL path file.</param>
        /// <returns></returns>
        [MethodImpl(Synchronized)]
        public static string[] GetStatementFromFile(string sqlPathFile)
        {
            string allStatement = BaseFileUtils.GetFileContent(sqlPathFile);
            return allStatement.Split(new string[] { "GO\r\n" }, StringSplitOptions.None);
        }
    }
}
