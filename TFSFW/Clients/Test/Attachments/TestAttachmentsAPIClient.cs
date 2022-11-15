using Demo.TFSAutomationFramework.Clients.Test.Points;
using Demo.TFSAutomationFramework.Configuration;
using Demo.TFSAutomationFramework.Request.Test.Attachements;
using Demo.TFSAutomationFramework.Response.GetPoint;
using RestSharp;
using RestSharp.Authenticators;

namespace Demo.TFSAutomationFramework.Clients.Test.Attachments
{
    public class TestAttachmentsAPIClient : AbstractAPIClient
    {
        /// <summary>
        /// Create an attachement into a test result
        /// Please <see cref="https://docs.microsoft.com/en-us/rest/api/azure/devops/test/attachments/create%20test%20result%20attachment?view=azure-devops-rest-5.1" />
        /// </summary>
        /// <param name="planId">The plan identifier.</param>
        /// <param name="suiteId">The suite identifier.</param>
        /// <param name="testCaseId">The test case identifier.</param>
        /// <param name="attachementRequest">The attachement request.</param>
        public void CreateTestResultAttachment(int planId, int suiteId, int testCaseId, CreateTestResultAttachementRequest attachementRequest)
        {
            var pointsApiClient = new PointsAPIClient();
            Value pointResponse = pointsApiClient.GetPoint(planId, suiteId, testCaseId).Value[0];
            string lastRunId = pointResponse.LastTestRun.Id;
            string lastResultId = pointResponse.LastResult.Id;
            string httpUri = TFSAutomationApiClientConfig.BaseTFSUrl + TFSAutomationApiClientConfig.ProjectId + $"/_apis/test/Runs/{lastRunId}/results/{lastResultId}/attachments";
            CreateTestAttachment(attachementRequest, httpUri);
        }

        /// <summary>
        /// Creates the test run attachment.
        /// </summary>
        /// <param name="planId">The plan identifier.</param>
        /// <param name="suiteId">The suite identifier.</param>
        /// <param name="testCaseId">The test case identifier.</param>
        /// <param name="attachementRequest">The attachement request.</param>
        public void CreateTestRunAttachment(int planId, int suiteId, int testCaseId, CreateTestResultAttachementRequest attachementRequest)
        {
            var pointsApiClient = new PointsAPIClient();
            Value pointResponse = pointsApiClient.GetPoint(planId, suiteId, testCaseId).Value[0];
            string lastRunId = pointResponse.LastTestRun.Id;
            string httpUri = TFSAutomationApiClientConfig.BaseTFSUrl + TFSAutomationApiClientConfig.ProjectId + $"/_apis/test/Runs/{lastRunId}/attachments";
            CreateTestAttachment(attachementRequest, httpUri);
        }

        /// <summary>
        /// Creates the test attachment.
        /// </summary>
        /// <param name="attachementRequest">The attachement request.</param>
        /// <param name="httpUri">The HTTP URI.</param>
        private void CreateTestAttachment(CreateTestResultAttachementRequest attachementRequest, string httpUri)
        {
            var request = new RestRequest(httpUri);
            request.AddJsonBody(attachementRequest);
            var authenticator = new HttpBasicAuthenticator("", TFSAutomationApiClientConfig.Pat);
            authenticator.Authenticate(Client, request);
            request.AddQueryParameter("api-version", TFSAutomationApiClientConfig.ApiVersion + "-preview"); // TODO: remove "-preview" when we migrate to api 5.x
            Client.Post(request);
        }
    }
}
