using Demo.DataUniverseFramework.Helpers;
using Demo.DataUniverseFramework.SQL;
using Demo.TestReport.Framework.Core;

namespace Demo.DataUniverseFramework
{
    public class DataBaseManager
    {
        public SqlConfig SQLConfigInstance { get; internal set; }
        public ISqlAccess SQLAccess { get; internal set; }
        public string SqlPath { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataBaseManager"/> class.
        /// </summary>
        public DataBaseManager() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataBaseManager" /> class.
        /// </summary>
        /// <param name="server">The server.</param>
        public DataBaseManager(string server)
        {
            SQLConfigInstance = new SqlConfig(server);
            SQLAccess = new SqlAccess(SQLConfigInstance);
        }

        /// <summary>
        /// Runs the statement.
        /// </summary>
        /// <returns></returns>
        public bool RunStatements()
        {
            string[] listStatement = SqlHelper.GetStatementFromFile(SqlPath);
            bool runStatement = true;
            foreach (string statement in listStatement)
            {
                ExtentTestManager.LogInfo("Check the data base with the following statement: " + statement);
                if (SqlHelper.RunStatementIntoDataBase(statement, SQLAccess) < 1)
                {
                    runStatement = false;
                    ExtentTestManager.LogError("the following statement does not do any change: " + statement);
                }
            }
            return runStatement;
        }         
    }
}
