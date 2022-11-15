using System.Collections.Generic;
using Demo.TFSAutomationFramework.Clients.Test.Points;
using Demo.TFSAutomationFramework.Configuration;
using Demo.TFSAutomationFramework.Request.Test.Results.Update;
using Demo.TFSAutomationFramework.Response.GetPoint;
using RestSharp;
using RestSharp.Authenticators;

namespace Demo.TFSAutomationFramework.Clients.Test.Results
{
    public class ResultsAPIClient : AbstractAPIClient
    {
        /// <summary>
        /// Update test result
        /// Please <see cref="https://docs.microsoft.com/en-us/rest/api/azure/devops/test/results/update?view=azure-devops-rest-5.1" />
        /// </summary>
        /// <param name="planId">The plan identifier.</param>
        /// <param name="suiteId">The suite identifier.</param>
        /// <param name="testCaseId">The test case identifier.</param>
        /// <param name="resultsRequest">The results request.</param>
        public void Update(int planId, int suiteId, int testCaseId, TestCaseResultRequest resultsRequest)
        {
            var pointsApiClient = new PointsAPIClient();
            Value pointResponse = pointsApiClient.GetPoint(planId, suiteId, testCaseId).Value[0];
            string lastRunId = pointResponse.LastTestRun.Id;
            string lastResultId = pointResponse.LastResult.Id;
            resultsRequest.Id = int.Parse(lastResultId);
            var request = new RestRequest(TFSAutomationApiClientConfig.BaseTFSUrl + TFSAutomationApiClientConfig.ProjectId + $"/_apis/test/Runs/{lastRunId}/results");
            var authenticator = new HttpBasicAuthenticator("", TFSAutomationApiClientConfig.Pat);
            authenticator.Authenticate(Client, request);
            request.AddJsonBody(new List<TestCaseResultRequest> { resultsRequest });
            request.AddQueryParameter("api-version", TFSAutomationApiClientConfig.ApiVersion);
            Client.Patch(request);
        }
    }
}
