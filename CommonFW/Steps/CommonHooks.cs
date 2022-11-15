using System.Collections.Generic;
using System.Text;
using Demo.CommonFramework.Config;
using Demo.CommonFramework.ExceptionHandler;
using Demo.CommonFramework.Helpers;
using Demo.TestReport.Framework.Configuration;
using Demo.TestReport.Framework.Core;
using Demo.TestReport.Framework.Helpers.Markup;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace Sequel.QA.CommonFramework.Steps
{
    [Binding]
    public sealed class CommonHooks : TechTalk.SpecFlow.Steps
    {
        /// <summary>
        /// Tests the case has never been executed into that test plan.
        /// </summary>
        /// <returns></returns>
        private bool TestCaseHasNeverBeenExecutedIntoThatTestPlan => TestContext.CurrentContext.Result.Message == null;

        /// <summary>
        /// Tests the do not passed before and is true rerun only failed tests TFS parameter.
        /// </summary>
        /// <returns></returns>
        private bool IsTFSIgnored => !TestContext.CurrentContext.Result.Message.Contains("TFS Ignored");

        /// <summary>
        /// Starts the logging.
        /// </summary>
        [BeforeScenario(Order = 0)]
        public void StartLogging()
        {
            ExtentReportConfig.TFS = RunSettings.TestResultFormatTFS;
            ExtentTestManager.CreateTest(TestContext.CurrentContext.Test.Name, RunSettings.TestLogLevel);
            ExtentTestManager.AddCategory(ScenarioContext.ScenarioInfo.Tags);


        }

        /// <summary>
        /// Logs the step.
        /// </summary>
        [BeforeStep]
        public void LogStep()
        {
            ExtentTestManager.LogInfo(" --> Starting", "Step '" + ScenarioContext.StepContext.StepInfo.StepDefinitionType.ToString()
                                                        + " " + ScenarioContext.StepContext.StepInfo.Text + "'");
        }

        /// <summary>
        /// Inconclusives the exception handler.
        /// </summary>
        [AfterStep]
        public void InconclusiveExceptionHandler()
        {
            string stepName = ScenarioContext.StepContext.StepInfo.StepDefinitionType.ToString() + " " + ScenarioContext.StepContext.StepInfo.Text;

            if (ScenarioContext.TestError != null)
            {
                if (ScenarioContext.TestError.GetType().Equals(typeof(InconclusiveFrameworkException)))
                {
                    ExtentTestManager.LogWarning(ScenarioContext.TestError.Message, stepName);
                    Assert.Inconclusive(ScenarioContext.TestError.Message);
                }
                else
                {
                    if (stepName.Length > 155) stepName = stepName.Substring(0, 150) + "[...]";
                    ExtentTestManager.LogFail(ScenarioContext.TestError.Message, stepName);
                    StringBuilder exceptionMessage = new StringBuilder();
                    var exception = ScenarioContext.TestError;
                    exceptionMessage.AppendLine(exception.Message);
                    while (exception.InnerException != null)
                    {
                        exception = exception.InnerException;
                        exceptionMessage.AppendLine(exception.Message);
                    }
                    ExtentTestManager.Log(LogStatus.DEBUG, Markup.CreateExceptionMessage(exceptionMessage.ToString()));
                }
            }
            else
            {
                ExtentTestManager.LogPass(string.Empty, stepName);
            }
        }

        /// <summary>
        /// Removes the flow ids.
        /// </summary>
        [AfterScenario]
        public void AfterScenario()
        {
            var status = TestContext.CurrentContext.Result.Outcome.Status;
            var stacktrace = string.IsNullOrEmpty(TestContext.CurrentContext.Result.StackTrace)
                    ? ""
                    : string.Format("<pre>{0}</pre>", TestContext.CurrentContext.Result.StackTrace);
            ExtentTestManager.LogInfo("Test ended with " + status);
            if (!string.IsNullOrEmpty(stacktrace))
            {
                ExtentTestManager.Log(LogStatus.DEBUG, Markup.CreateStackTrace(stacktrace));
            }
            ExtentTestManager.EndTest();

            if (!RunSettings.TestResultFormatTFS)
            {
                if (RunSettings.IntegrateTFS
                    && (TestCaseHasNeverBeenExecutedIntoThatTestPlan || IsTFSIgnored))
                {
                    var tfsIntegrationHelper = new TfsIntegrationHelper();
                    tfsIntegrationHelper.AddTestResult();
                    tfsIntegrationHelper.AddTestRunAttachment(ExtentTestManager.ZipTest());
                }

                List<string> reports = BaseFileUtils.GetFilesByExtension(ExtentTestManager.TestResultHtmlFilesPath, ".html");
                foreach (string file in reports)
                {
                    TestContext.AddTestAttachment(file);
                }
            }
            else
            {
                ExtentTestManager.ZipTest();
            }
        }
    }
}
