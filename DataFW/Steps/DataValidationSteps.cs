using System;
using System.Collections.Generic;
using System.Linq;
using Demo.CommonFramework.ExceptionHandler;
using Demo.CommonFramework.Extensions;
using Demo.CommonFramework.Helpers;
using Demo.CommonFramework.Steps;
using Demo.DataUniverseFramework.SQL;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace Demo.DataUniverseFramework.Steps
{
    [Binding]
    public class DataValidationSteps : BaseCommonSteps
    {
        protected RequestProducer requestProducerInstance;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataValidationSteps"/> class.
        /// </summary>
        /// <param name="producer">The producer.</param>
        public DataValidationSteps(RequestProducer producer)
        {
            requestProducerInstance = producer;
        }

        /// <summary>
        /// Thens the values shall match with SQL.
        /// </summary>
        /// <param name="comparisonType">Type of the comparison.</param>
        /// <param name="sqlPath">The SQL path.</param>
        /// <param name="databaseType">Type of the database.</param>
        /// <param name="tableFieldsValues">The table fields values.</param>
        [When(@"the values indicated shall (.*) the column of the result of the SQL file located at (.*) executed in the (.*) database")]
        [Then(@"the values indicated shall (.*) the column of the result of the SQL file located at (.*) executed in the (.*) database")]
        public void ThenTheValueShallMatchWithSQL(string comparisonType, string sqlPath, string databaseType, Table tableFieldsValues)
        {
            requestProducerInstance.WaitUntilDBIsUpdated(sqlPath, TimeSpan.FromSeconds(60), new SqlAccess(requestProducerInstance.GetSqlConfig(databaseType.ToLower().Equals("frontend"))));
            BaseUtils.TableToIEnumerable(tableFieldsValues, out string[] fieldsName, out string[] values);
            requestProducerInstance.GenerateEnvironmentParamRequest(values, sqlPath, new SqlAccess(requestProducerInstance.GetSqlConfig(databaseType.ToLower().Equals("frontend"))));
            Dictionary<string, string> dict = BaseUtils.BuildDictionary(fieldsName, requestProducerInstance.RequestResponseProvider.Param);
            Assert.Multiple(() =>
            {
                foreach (var item in dict)
                {
                    List<string> sqlValues = requestProducerInstance.GetValuesFromSQLFileColumn(item.Key, sqlPath, new SqlAccess(requestProducerInstance.GetSqlConfig(databaseType.ToLower().Equals("frontend"))));
                    List<string> tableValues = ListOfParametersTransform(item.Value).ToList();
                    bool equals = !comparisonType.ToLower().Equals(BaseUtils.EMatchType.Contain.ToString().ToLower())
                                    ? tableValues.SequenceEqual(sqlValues)
                                    : sqlValues.All(x => tableValues.Any(y => y.Equals(x)));
                    Assert.IsTrue(equals, item.Key + " does not match the expected. Received: " + sqlValues.ToStringList() + "; Expected: " + tableValues.ToStringList());
                }
            });
        }

        /// <summary>
        /// Thens the compare variables.
        /// </summary>
        /// <param name="variable1">The variable1.</param>
        /// <param name="comparison">The comparison.</param>
        /// <param name="variable2">The variable2.</param>
        [Then(@"the (.*) variable is (.*) the (.*) variable")]
        public void ThenCompareVariables(string variableName1, string comparison, string variableName2)
        {
            requestProducerInstance.GenerateEnvironmentParamRequest(new List<string> { variableName1, variableName2 }, null, new SqlAccess(requestProducerInstance.GetSqlConfig()));
            string variable1 = requestProducerInstance.RequestResponseProvider.Param[0];
            string variable2 = requestProducerInstance.RequestResponseProvider.Param[1];
            Enum.TryParse(comparison.Replace(" ", ""), ignoreCase: true, out BaseUtils.EMatchType matchType);
            switch (matchType)
            {
                case BaseUtils.EMatchType.EqualTo:
                    Assert.AreEqual(variable1, variable2, "The variable " + variableName1 + "=" + variable1 + " is not equal to " + variableName2 + "=" + variable2);
                    break;
                case BaseUtils.EMatchType.GreaterOrEqualThan:
                    Assert.GreaterOrEqual(variable1, variable2, "The variable " + variableName1 + "=" + variable1 + " is not greater or equal than " + variableName2 + "=" + variable2);
                    break;
                case BaseUtils.EMatchType.GreaterThan:
                    Assert.Greater(variable1, variable2, "The variable " + variableName1 + "=" + variable1 + " is not greater than " + variableName2 + "=" + variable2);
                    break;
                case BaseUtils.EMatchType.LowerOrEqualThan:
                    Assert.LessOrEqual(variable1, variable2, "The variable " + variableName1 + "=" + variable1 + " is not lower or equal than " + variableName2 + "=" + variable2);
                    break;
                case BaseUtils.EMatchType.LowerThan:
                    Assert.Less(variable1, variable2, "The variable " + variableName1 + "=" + variable1 + " is not lower than " + variableName2 + "=" + variable2);
                    break;
                default:
                    Assert.Fail("'" + comparison + "' not supported. Please try one of the following: " + Enum.GetNames(typeof(BaseUtils.EMatchType)).ToList().ToStringList());
                    break;
            }
        }

        /// <summary>
        /// Thens the check columns from SQL.
        /// </summary>
        /// <param name="columnName1">The column name1.</param>
        /// <param name="sqlPath1">The SQL path1.</param>
        /// <param name="appLayer1">The application layer1.</param>
        /// <param name="comparisonType">Type of the comparison.</param>
        /// <param name="columnName2">The column name2.</param>
        /// <param name="sqlPath2">The SQL path2.</param>
        /// <param name="appLayer2">The application layer2.</param>
        [Then(@"the column (.*) of the result of the SQL file located at (.*) executed in the (.*) database shall (.*) the column (.*) of the result of the other SQL file located at (.*) executed in the (.*) database")]
        public void ThenCheckColumnsFromSQL(string columnName1, string sqlPath1, string appLayer1, string comparisonType, string columnName2, string sqlPath2, string appLayer2)
        {
            List<string> sqlValues1 = requestProducerInstance.GetValuesFromSQLFileColumn(columnName1, sqlPath1, new SqlAccess(requestProducerInstance.GetSqlConfig(appLayer1.ToLower().Equals("frontend"))));
            List<string> sqlValues2 = requestProducerInstance.GetValuesFromSQLFileColumn(columnName2, sqlPath2, new SqlAccess(requestProducerInstance.GetSqlConfig(appLayer2.ToLower().Equals("frontend"))));

            Enum.TryParse(comparisonType, out BaseUtils.EMatchType enumComparison);
            bool result = false;

            switch (enumComparison)
            {
                case BaseUtils.EMatchType.Contain:
                    result = sqlValues1.All(x => sqlValues2.Any(y => y.Equals(x)));
                    break;
                case BaseUtils.EMatchType.Differ:
                    result = !sqlValues1.SequenceEqual(sqlValues2);
                    break;
                case BaseUtils.EMatchType.Match:
                    result = sqlValues1.SequenceEqual(sqlValues2);
                    break;
            }

            Assert.IsTrue(result, columnName1 + " does not " + comparisonType + " the expected. Values1: " + sqlValues1.ToStringList() + "; Values2: " + sqlValues2.ToStringList());
        }

        /// <summary>
        /// Thens the check tables from SQL.
        /// </summary>
        /// <param name="sqlPath1">The SQL path1.</param>
        /// <param name="appLayer1">The application layer1.</param>
        /// <param name="comparisonType">Type of the comparison.</param>
        /// <param name="sqlPath2">The SQL path2.</param>
        /// <param name="appLayer2">The application layer2.</param>
        [Then(@"the table of the SQL file located at (.*) executed in the (.*) database shall (.*) the table of the other SQL file located at (.*) executed in the (.*) database")]
        public void ThenCheckTablesFromSQL(string sqlPath1, string appLayer1, string comparisonType, string sqlPath2, string appLayer2)
        {
            requestProducerInstance.WaitUntilDBIsUpdated(sqlPath1, TimeSpan.FromSeconds(60), new SqlAccess(requestProducerInstance.GetSqlConfig(appLayer1.ToLower().Equals("frontend"))));
            requestProducerInstance.WaitUntilDBIsUpdated(sqlPath2, TimeSpan.FromSeconds(60), new SqlAccess(requestProducerInstance.GetSqlConfig(appLayer2.ToLower().Equals("frontend"))));
            List<string> columNames = new List<string>();
            try
            {
                columNames = requestProducerInstance.GetColumnsFromSQLFile(sqlPath1, new SqlAccess(requestProducerInstance.GetSqlConfig(appLayer1.ToLower().Equals("frontend"))));
            }
            catch (InconclusiveFrameworkException e)
            {
                Assert.Fail(e.Message);
            }
            Assert.Multiple(() =>
            {
                foreach (var column in columNames)
                {
                    ThenCheckColumnsFromSQL(column, sqlPath1, appLayer1, comparisonType, column, sqlPath2, appLayer2);
                }
            });
        }
    }
}
