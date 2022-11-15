using System;
using System.IO;
using Demo.TFSAutomationFramework.Clients.Test.Attachments;
using Demo.TFSAutomationFramework.Request.Test.Attachements;

namespace Demo.TFSAutomationFramework.Helpers
{
    /// <summary>
    /// Provides methods that produces updates into TFS test results using the TFS api support.
    /// </summary>
    public class TestResultHelper
    {
        /// <summary>
        /// Adds the attachment to test result.
        /// </summary>
        /// <param name="planId">The plan identifier.</param>
        /// <param name="suiteId">The suite identifier.</param>
        /// <param name="testCaseId">The test case identifier.</param>
        /// <param name="fileFullPath">Full path including file name that we would like to attach</param>
        /// <param name="fileName">Is the file name that we are attaching, can be different than the original one</param>
        /// <param name="comment">optional comment</param>
        public void AddAttachmentToTestResult(int planId, int suiteId, int testCaseId, string fileFullPath, string fileName, string comment = null)
        {
            var attachementRequest = CreateAttachment(fileFullPath, fileName, comment);
            var testAttachmentsApiClient = new TestAttachmentsAPIClient();
            testAttachmentsApiClient.CreateTestResultAttachment(planId, suiteId, testCaseId, attachementRequest);
        }

        /// <summary>
        /// Adds the attachement to test run.
        /// </summary>
        /// <param name="planId">The plan identifier.</param>
        /// <param name="suiteId">The suite identifier.</param>
        /// <param name="testCaseId">The test case identifier.</param>
        /// <param name="fileFullPath">The file full path.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="comment">The comment.</param>
        public void AddAttachmentToTestRun(int planId, int suiteId, int testCaseId, string fileFullPath, string fileName, string comment = null)
        {
            var attachementRequest = CreateAttachment(fileFullPath, fileName, comment);
            var testAttachmentsApiClient = new TestAttachmentsAPIClient();
            testAttachmentsApiClient.CreateTestRunAttachment(planId, suiteId, testCaseId, attachementRequest);
        }

        /// <summary>
        /// Creates the attachment.
        /// </summary>
        /// <param name="fileFullPath">The file full path.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="comment">The comment.</param>
        /// <returns></returns>
        private CreateTestResultAttachementRequest CreateAttachment(string fileFullPath, string fileName, string comment = null)
        {
            byte[] fileBytesArray = File.ReadAllBytes(fileFullPath);
            string fileStream = Convert.ToBase64String(fileBytesArray);
            var attachementRequest = new CreateTestResultAttachementRequest
            {
                Stream = fileStream,
                Comment = comment,
                FileName = fileName
            };

            return attachementRequest;
        }
    }
}
