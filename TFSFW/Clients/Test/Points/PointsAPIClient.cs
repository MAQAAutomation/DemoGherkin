using Demo.TFSAutomationFramework.Configuration;
using Demo.TFSAutomationFramework.Response.GetPoint;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;

namespace Demo.TFSAutomationFramework.Clients.Test.Points
{
    public class PointsAPIClient : AbstractAPIClient
    {
        /// <summary>
        /// Points - Get Point <see cref="https://docs.microsoft.com/en-us/rest/api/azure/devops/test/points/get%20point?view=azure-devops-rest-5.1"/>
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="suiteId"></param>
        /// <param name="testCaseId"></param>
        /// <returns>GetPointResponse</returns>
        public GetPointResponse GetPoint(int planId, int suiteId, int testCaseId)
        {
            var request = new RestRequest($"{TFSAutomationApiClientConfig.ProjectId}/_apis/test/plans/{planId}/suites/{suiteId}/points");
            request.AddParameter("testCaseId", testCaseId);
            var authenticator = new HttpBasicAuthenticator("", TFSAutomationApiClientConfig.Pat);
            authenticator.Authenticate(Client, request);

            IRestResponse response = Client.ExecuteAsGet(request, "GET");
            var getPointResponse = JsonConvert.DeserializeObject<GetPointResponse>(response.Content);

            return getPointResponse;
        }
    }
}
