using System;
using System.Net;
using System.Text.RegularExpressions;
using Demo.TFSAutomationFramework.Clients.Test.Points;
using Demo.TFSAutomationFramework.Configuration;
using Demo.TFSAutomationFramework.Constants;
using NUnit.Framework.Interfaces;
using RestSharp;
using RestSharp.Authenticators;


namespace Demo.TFSAutomationFramework.Clients.Test.Results
{
    //TODO remove totally this class and implement 'create test result API' specification
    public class TestManagementAPIClient : AbstractAPIClient
    {
        /// <summary>
        /// Is part of the url when we perform the bulk update, e.g 'Claims'
        /// </summary>
        private string Team { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestManagementAPIClient"/> class.
        /// </summary>
        /// <param name="team">The team.</param>
        public TestManagementAPIClient(string team)
        {
            Client = new RestClient(TFSAutomationApiClientConfig.BaseTFSUrl)
            {
                CookieContainer = new CookieContainer()
            };
            Team = team;
            Area = "_testManagement";
        }

        /// <summary>
        /// Set a test case run result
        /// </summary>
        /// <param name="planId">Plan id number where test case belong</param>
        /// <param name="suiteId">Suite id number where test belong</param>
        /// <param name="testCaseId">Test case id number</param>
        /// <returns></returns>
        public IRestResponse SetTestCaseResult(int planId, int suiteId, int testCaseId, TestStatus status)
        {
            TestResult result;
            switch (status)
            {
                case TestStatus.Passed:
                    result = TestResult.PASS;
                    break;
                case TestStatus.Failed:
                    result = TestResult.FAIL;
                    break;
                case TestStatus.Skipped:
                    result = TestResult.NON_APPLICABLE;
                    break;
                case TestStatus.Inconclusive:
                    result = TestResult.BLOCK;
                    break;
                default:
                    throw new ArgumentException($"status: {status} invalid argument value");
            }

            string resource = "BulkMarkTestPoints";
            string requestVerificationToken = GetRequestVerificationToken(planId, suiteId);
            int testPointId = new PointsAPIClient().GetPoint(planId, suiteId, testCaseId).Value[0].Id;
            var request = new RestRequest($"{TFSAutomationApiClientConfig.ProjectId}/_api/{Area}/{resource}");
            request
                .AddParameter("__RequestVerificationToken", requestVerificationToken)
                .AddParameter("testPointIds", testPointId)
                .AddParameter("planId", planId)
                .AddParameter("suiteId", suiteId)
                .AddParameter("outcome", (int)result)
                .AddQueryParameter("__v", TFSAutomationApiClientConfig.ApiVersion);

            var authenticator = new HttpBasicAuthenticator("", TFSAutomationApiClientConfig.Pat);
            authenticator.Authenticate(Client, request);

            return Client.Post(request);
        }

        /// <summary>
        /// Get the request verification token that is a form parameter that it is needed by POST api calls
        /// also when we get this page it sets the authentication tokens as cookies which are needed for POST api calls. 
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="suiteId"></param>
        /// <returns></returns>
        public string GetRequestVerificationToken(int planId, int suiteId)
        {
            var request = new RestRequest($"/{Team}/_testManagement");
            var authenticator = new HttpBasicAuthenticator("", TFSAutomationApiClientConfig.Pat);
            request.AddParameter("plainId", planId);
            request.AddParameter("suiteId", suiteId);
            authenticator.Authenticate(Client, request);

            IRestResponse response = Client.Get(request);
            string responseContent = response.Content;

            return new Regex("__RequestVerificationToken\" type=\"hidden\" value=\"([ _.=A-Za-z0-9\\-]*)").Match(responseContent).Groups[1].Value;
        }
    }
}
