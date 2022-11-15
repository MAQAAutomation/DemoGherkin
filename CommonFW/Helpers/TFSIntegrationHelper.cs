using System.Collections.Generic;
using System.IO;
using System.Linq;
using Demo.CommonFramework.Config;
using Demo.CommonFramework.Extensions;
using Demo.TestReport.Framework.Core;
using Demo.TFSAutomationFramework.Clients.Test.Points;
using Demo.TFSAutomationFramework.Clients.Test.Results;
using Demo.TFSAutomationFramework.Helpers;
using Demo.TFSAutomationFramework.Response.GetPoint;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using static NUnit.Framework.TestContext;

namespace Demo.CommonFramework.Helpers
{
    public class TfsIntegrationHelper
    {
        private string TestCaseId
        {
            get
            {
                return CurrentContext.Test.FullName.Substring(CurrentContext.Test.FullName.LastIndexOf('_') + 1, CurrentContext.Test.FullName.Length - CurrentContext.Test.FullName.LastIndexOf('_') - 1);
            }
        }

        /// <summary>
        /// Adds the test attachements to test result.
        /// </summary>
        /// <param name="testPlan">The test plan.</param>
        /// <param name="testSuite">The test suite.</param>
        /// <param name="testCaseId">The test case identifier.</param>
        internal void AddTestAttachmentsToTestResult(int testPlan, int testSuite, int testCaseId)
        {
            ICollection<TestAttachment> attachments = TestExecutionContext.CurrentContext.CurrentResult.TestAttachments;
            var testResultHelper = new TestResultHelper();
            foreach (TestAttachment attachment in attachments)
            {
                string filePath = attachment.FilePath;
                string fileName = Path.GetFileName(filePath);
                testResultHelper.AddAttachmentToTestResult(RunSettings.TestPlanId, RunSettings.TestSuiteId, testCaseId, filePath, fileName);
            }
        }

        /// <summary>
        /// Adds the test attachments to test run.
        /// </summary>
        /// <param name="testPlan">The test plan.</param>
        /// <param name="testSuite">The test suite.</param>
        /// <param name="testCaseId">The test case identifier.</param>
        /// <param name="zipFilePath">The zip file path.</param>
        internal void AddTestAttachmentsToTestRun(int testPlan, int testSuite, int testCaseId, string zipFilePath)
        {
            string filePath = zipFilePath;
            string fileName = Path.GetFileName(filePath);
            new TestResultHelper().AddAttachmentToTestRun(RunSettings.TestPlanId, RunSettings.TestSuiteId, testCaseId, filePath, fileName);
        }

        /// <summary>
        /// adds test result.
        /// </summary>
        /// <param name="testCaseId">The test case identifier.</param>
        /// <param name="testSuiteId">The test suite identifier.</param>
        /// <param name="status">The status.</param>
        public void AddTestResult(int testCaseId = 0, int testSuiteId = 0, TestStatus? status = null)
        {
            if (testCaseId > 0)
            {
                AddTestCaseResult(testCaseId, testSuiteId, status);
            }
            else if (TestCaseId.IsIntegerGreaterThanZero())
            {
                AddTestCaseResult(int.Parse(TestCaseId));
            }
        }

        /// <summary>
        /// Adds the test run attachment.
        /// </summary>
        /// <param name="zipFilePath">The zip file path.</param>
        public void AddTestRunAttachment(string zipFilePath)
        {
            if (TestCaseId.IsIntegerGreaterThanZero())
            {
                AddTestRunResult(int.Parse(TestCaseId), zipFilePath);
            }
        }

        /// <summary>
        /// Adds the test run result.
        /// </summary>
        /// <param name="testCaseId">The test case identifier.</param>
        /// <param name="zipFilePath">The zip file path.</param>
        internal void AddTestRunResult(int testCaseId, string zipFilePath)
        {
            if (TestCaseItemExistIntoTheTestSuite(RunSettings.TestPlanId, RunSettings.TestSuiteId, testCaseId))
            {
                AddTestAttachmentsToTestRun(RunSettings.TestPlanId, RunSettings.TestSuiteId, testCaseId, zipFilePath);
            }
        }

        /// <summary>
        /// adds test case result.
        /// </summary>
        /// <param name="testCaseId">The test case identifier.</param>
        /// <param name="testSuiteId">The test suite identifier.</param>
        /// <param name="status">The status.</param>
        internal void AddTestCaseResult(int testCaseId, int testSuiteId = 0, TestStatus? status = null)
        {
            testSuiteId = (testSuiteId > 0) ? testSuiteId : RunSettings.TestSuiteId;

            if (TestCaseItemExistIntoTheTestSuite(RunSettings.TestPlanId, testSuiteId, testCaseId))
            {
                var testManagementApi = new TestManagementAPIClient(RunSettings.ProjectId);
                TestStatus currentStatus = (status != null) ? (TestStatus)status : CurrentContext.Result.Outcome.Status;

                if (currentStatus == TestStatus.Inconclusive)
                {
                    currentStatus = TestStatus.Skipped;
                }

                testManagementApi.SetTestCaseResult(RunSettings.TestPlanId, testSuiteId, testCaseId, currentStatus);
                AddTestAttachmentsToTestResult(RunSettings.TestPlanId, testSuiteId, testCaseId);
            }
        }

        /// <summary>
        /// Tests the case item exist into the test suite.
        /// </summary>
        /// <param name="planId">The plan identifier.</param>
        /// <param name="suiteId">The suite identifier.</param>
        /// <param name="testCaseId">The test case identifier.</param>
        /// <returns></returns>
        public bool TestCaseItemExistIntoTheTestSuite(int planId, int suiteId, int testCaseId)
        {
            var pointsApiClient = new PointsAPIClient();
            GetPointResponse point = pointsApiClient.GetPoint(planId, suiteId, testCaseId);

            bool found = point.Value.Any();
            string foundString = found ? "found" : "not found";
            ExtentTestManager.LogDebug($"Test case {testCaseId} {foundString} into test suite {suiteId}");

            return found;
        }
    }
}
