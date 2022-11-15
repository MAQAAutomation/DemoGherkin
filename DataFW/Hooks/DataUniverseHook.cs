using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Demo.CommonFramework.Helpers;
using Demo.TestReport.Framework.Core;
using TechTalk.SpecFlow;
using static Demo.CommonFramework.Helpers.BaseFileUtils;

namespace Demo.DataUniverseFramework.Hooks
{
    [Binding]
    public sealed class DataUniverseHook : Steps.CommonSteps
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataUniverseHook"/> class.
        /// </summary>
        /// <param name="objectContainer">The object container.</param>
        public DataUniverseHook(RequestProducer requestProducer) : base(requestProducer)
        {
            requestProducerInstance = requestProducer;
        }

        /// <summary>
        /// Befores the scenario.
        /// </summary>
        [BeforeScenario(Order = 5)]
        public void BeforeScenario()
        {
            string connectionStringRequest = requestProducerInstance.ConnectionStringRequest;
            string connectionStringResponse = requestProducerInstance.ConnectionStringResponse;

            string INTERNAL_ENV_FRONTEND = Regex.Match(connectionStringRequest, @"Initial Catalog=(.+?);").Groups[0].Value.Replace("_Origin;", "").Replace("Initial Catalog=", "");
            string INTERNAL_ENV_BACKEND = Regex.Match(connectionStringResponse, @"database=(.+?);").Groups[0].Value.Replace("_Eclipse;", "").Replace("database=", "");

            requestProducerInstance.VariablesDict.Add("$INTERNAL_ENV_BACKEND$", INTERNAL_ENV_BACKEND);
            requestProducerInstance.VariablesDict.Add("$INTERNAL_ENV_FRONTEND$", INTERNAL_ENV_FRONTEND); 
        }
        /// <summary>
        /// Afters the step.
        /// </summary>
        [AfterStep(Order = 1)]
        public void AfterStep()
        {
            //TODO Because a possible race condition error we put a try-catch  to avoid the test would be stopped because a log error
            //Review this try catch with the PBI http://tfslive:8080/tfs/MIG01/Quality%20Management/_workitems/edit/320772 will be done
            try
            {
                string filePath = ExtentTestManager.TestResultResourcesFilesPath;
                
                foreach (var sqlFile in requestProducerInstance.SqlFileList)
                {
                    if (!Path.GetExtension(sqlFile).Equals(BaseUtils.GetEnumDescription(EFileExtension.SQL))) continue;

                    string sqlFileName = Path.GetFileNameWithoutExtension(sqlFile) + ".sql";
                        if (!File.Exists(filePath + sqlFileName))
                        {
                            string fileContent = File.ReadAllText(sqlFile);
                            Dictionary<string, string> variables = requestProducerInstance.VariablesDict;
                            Common.ReplaceParametersByFunctions(ref variables, ref fileContent,null, null);
                            ExtentTestManager.LogInfoWithAttach(StepContext.StepInfo.Text, filePath, sqlFileName, fileContent);
                        }
                }
            }
            catch (Exception e)
            {
                ExtentTestManager.LogWarning("Data Universe, The test skipped an Error happened writting the logs in the test execution please take a look: " + e.Message);
            }
        }
    }
}
