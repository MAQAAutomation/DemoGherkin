using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Demo.CommonFramework.Config;
using Demo.CommonFramework.Helpers;
using Demo.CommonFramework.Steps;
using Demo.TestReport.Framework.Core;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace Demo.RestServiceFramework.Steps
{
    public class CommonSteps : BaseCommonSteps
    {
        private readonly WebHookManager webHook = new WebHookManager();
        private readonly CustomWebApiClient webApiClient = new CustomWebApiClient();

        private string m_response = null;
        private int m_pageSize = RunSettings.DefaultPageSize;
        private int m_pageNumber = 1;
        private Dictionary<string, string> fieldsFromResponse = null;

        /// <summary>
        /// Gets the response.
        /// </summary>
        /// <value>
        /// The response.
        /// </value>
        private string Response
        {
            get
            {
                if (m_response == null)
                {
                    m_response = webApiClient.Result;
                    if (webHook.IsStarted() && webHook.WaitForResponse(webApiClient.Result))
                    {
                        m_response = webHook.Response;
                    }
                }
                return m_response;
            }
        }

        /// <summary>
        /// Givens the configured rest service available.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        [Given("the rest service available with the (.*) endpoint"),
        When("the rest service available with the (.*) endpoint"),
        Then("the rest service available with the (.*) endpoint")]
        public void GivenTheConfiguredRestServiceAvailable(string endpoint)
        {
            GivenTheWebServiceAvailableWithProtocolAndEndPoint(endpoint);
        }

        /// <summary>
        /// Givens the configured service available with header.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="headerParams">The header parameters.</param>
        [Given(@"the rest service available with the (.*) endpoint and with the header ((?:.,\.+)*(?:.*))"),
        When(@"the rest service available with the (.*) endpoint and with the header ((?:.,\.+)*(?:.*))"),
        Then(@"the rest service available with the (.*) endpoint and with the header ((?:.,\.+)*(?:.*))")]
        public void GivenTheConfiguredServiceAvailableWithHeader(string endpoint, IEnumerable<string> headerParams = null)
        {
            GivenTheWebServiceAvailableWithProtocolWithEndPointAndHeader(endpoint, headerParams);
        }

        /// <summary>
        /// Givens the web service available with protocol and end point.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        private void GivenTheWebServiceAvailableWithProtocolAndEndPoint(string endpoint = "")
        {
            webApiClient.Server = RunSettings.PredefinedEnvironment;
            webApiClient.QueryParam = string.Empty;
            webApiClient.PathParam = string.Empty;
            webApiClient.JsonRequest = string.Empty;
            string protocol = RunSettings.RestServiceUrl.Split(':')[0];
            string serviceName = RunSettings.RestServiceUrl;
            serviceName = serviceName.Replace(protocol + "://", "");

            Assert.IsTrue(webApiClient.CheckIfWebServiceAvailable("", serviceName, protocol, endpoint), "The " + serviceName + endpoint + " service is NOT available!");
        }

        /// <summary>
        /// Givens the web service available with protocol with end point andh header.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="headerParams">The header parameters.</param>
        private void GivenTheWebServiceAvailableWithProtocolWithEndPointAndHeader(string endpoint, IEnumerable<string> headerParams = null)
        {
            if (headerParams != null)
            {
                webApiClient.AddParameterToTheHeader(headerParams);
            }
            GivenTheWebServiceAvailableWithProtocolAndEndPoint(endpoint);
        }

        /// <summary>
        /// Givens a collection of path parameters.
        /// </summary>
        /// <param name="pathParams">The path parameters.</param>
        [Given(@"a collection of path params ((?:.,\.+)*(?:.*)) content"),
        When(@"a collection of path params ((?:.,\.+)*(?:.*)) content"),
        Then(@"a collection of path params ((?:.,\.+)*(?:.*)) content")]
        public void GivenACollectionOfPathParams(IEnumerable<string> pathParams)
        {
            webApiClient.PathParam = BaseUtils.GetParameterListAsString(pathParams, BaseUtils.EParamType.PathParam);
            ExtentTestManager.LogInfo("Path Params: " + webApiClient.PathParam);
        }

        /// <summary>
        /// Givens a collection of query parameters.
        /// </summary>
        /// <param name="queryParams">The query parameters.</param>
        [Given(@"a collection of query params ((?:.,\.+)*(?:.*)) content"),
        When(@"a collection of query params ((?:.,\.+)*(?:.*)) content"),
        Then(@"a collection of query params ((?:.,\.+)*(?:.*)) content")]
        public void GivenACollectionOfQueryParams(IEnumerable<string> queryParams)
        {
            webApiClient.QueryParam = BaseUtils.GetParameterListAsString(queryParams, BaseUtils.EParamType.QueryParam);
            ExtentTestManager.LogInfo("Query Params: " + webApiClient.QueryParam);
            UpdatePagination(webApiClient.QueryParam);
        }

        /// <summary>
        /// Updates the pagination.
        /// </summary>
        /// <param name="queryParamString">The query parameter string.</param>
        private void UpdatePagination(string queryParamString)
        {
            Dictionary<string, List<string>> dict = BaseUtils.GetQueryParamDictionary(queryParamString);
            string pageNumString = dict.ContainsKey("pageNumber") ? dict.FirstOrDefault(x => string.Equals(x.Key, "pageNumber", StringComparison.OrdinalIgnoreCase)).Value[0] : string.Empty;
            string pageSizeString = dict.ContainsKey("pageSize") ? dict.FirstOrDefault(x => string.Equals(x.Key, "pageSize", StringComparison.OrdinalIgnoreCase)).Value[0] : string.Empty;
            if (int.TryParse(pageNumString, out int pageNum))
            {
                m_pageNumber = pageNum;
            }
            if (int.TryParse(pageSizeString, out int pageSize))
            {
                m_pageSize = pageSize;
            }
        }

        /// <summary>
        /// I use the security configuration from file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        [Given(@"I use the security configuration from (.*) file"),
        When(@"I use the security configuration from (.*) file"),
        Then(@"I use the security configuration from (.*) file")]
        public void IUseTheSecurityConfigFromFile(string filePath)
        {
            webApiClient.AuthorizationFilePath = filePath;
        }

        /// <summary>
        /// Uses the webhook.
        /// </summary>
        [Given(@"I listen the response using the webhook"),
        When(@"I listen the response using the webhook"),
        Then(@"I listen the response using the webhook")]
        public void UseWebhook()
        {
            webHook.StartServer(RunSettings.WebHookTimeoutSeconds * 1000);
        }

        /// <summary>
        /// Whens the i perform the action.
        /// </summary>
        /// <param name="action">The action.</param>
        [Given(@"I perform the (.*) action"),
        When(@"I perform the (.*) action"),
        Then(@"I perform the (.*) action")]
        public void WhenIPerformTheAction(string action)
        {
            webApiClient.Action = action;
        }

        /// <summary>
        /// is the send a json request.
        /// </summary>
        [Given(@"I send the request"),
        When(@"I send the request"),
        Then(@"I send the request")]
        public void ISendRequest()
        {
            m_response = null;
            webApiClient.CallWebService();
        }

        /// <summary>
        /// is the send a json request.
        /// </summary>
        /// <param name="jsonRequestPath">The json request path.</param>
        [Given(@"I send the (.*) json request content"),
        When(@"I send the (.*) json request content"),
        Then(@"I send the (.*) json request content")]
        public void ISendAJsonRequest(string jsonRequestPath)
        {
            string jsonRequest = BaseFileUtils.GetFileContent(jsonRequestPath);
            if (webHook.IsStarted())
            {
                webHook.AddWebHookElementToJsonRequest(ref jsonRequest);
            }

            webApiClient.JsonRequest = jsonRequest;
            ISendRequest();
        }

        /// <summary>
        /// Whens the i take value from response message.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="path">The path.</param>
        [Given(@"I set ((?:.,\.+)*(?:.*)) value from response message into the file located at (.*) path"),
        Given(@"I set ((?:.,\.+)*(?:.*)) values from response message into the file located at (.*) path"),
        When(@"I set ((?:.,\.+)*(?:.*)) value from response message into the file located at (.*) path"),
        When(@"I set ((?:.,\.+)*(?:.*)) values from response message into the file located at (.*) path"),
        Then(@"I set ((?:.,\.+)*(?:.*)) value from response message into the file located at (.*) path"),
        Then(@"I set ((?:.,\.+)*(?:.*)) values from response message into the file located at (.*) path")]
        public void WhenITakeValueFromResponseMessage(IEnumerable<string> values, string path)
        {
            fieldsFromResponse = new Dictionary<string, string>();
            foreach (var value in values)
            {
                fieldsFromResponse[value] = BaseJsonHelper.GetElementValueFromJsonString(Response, "$.." + value);
                ExtentTestManager.LogInfo(value + " = " + fieldsFromResponse[value]);
            }
            BaseJsonHelper.SetValuesIntoJsonFile(fieldsFromResponse, path);
        }

        /// <summary>
        /// Whens the i take json path value from response message.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="path">The path.</param>
        [Given(@"I set value using ((?:.,\.+)*(?:.*)) JsonPath from response message into the file located at (.*) path"),
         Given(@"I set values using ((?:.,\.+)*(?:.*)) JsonPaths from response message into the file located at (.*) path"),
         When(@"I set value using ((?:.,\.+)*(?:.*)) JsonPath from response message into the file located at (.*) path"),
         When(@"I set values using ((?:.,\.+)*(?:.*)) JsonPaths from response message into the file located at (.*) path"),
         Then(@"I set value using ((?:.,\.+)*(?:.*)) JsonPath from response message into the file located at (.*) path"),
         Then(@"I set values using ((?:.,\.+)*(?:.*)) JsonPaths from response message into the file located at (.*) path")]
        public void WhenITakeJsonPathValueFromResponseMessage(IEnumerable<string> values, string path)
        {
            fieldsFromResponse = new Dictionary<string, string>();
            foreach (var value in values)
            {
                fieldsFromResponse[value] = BaseJsonHelper.GetElementValueFromJsonString(Response, value);
                ExtentTestManager.LogInfo(value + " = " + fieldsFromResponse[value]);
            }
            BaseJsonHelper.SetValuesIntoJsonFile(fieldsFromResponse, path);
        }

        /// <summary>
        /// Thens the HTTP response should be.
        /// </summary>
        /// <param name="httpCode">The HTTP code.</param>
        [Given(@"the HTTP response should be (.*)"),
        When(@"the HTTP response should be (.*)"),
        Then(@"the HTTP response should be (.*)")]
        public void ThenTheHTTPResponseShouldBe(string httpCode)
        {
            HttpStatusCode httpStatusExpected = (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), httpCode);
            HttpStatusCode httpStatusReceived = webApiClient.HttpResponseStatus;
            Assert.IsTrue(httpStatusReceived.Equals(httpStatusExpected), "HTTP Status Received: " + httpStatusReceived + "; Expected: " + httpStatusExpected);
        }

        /// <summary>
        /// Thens the result should match the expected.
        /// </summary>
        /// <param name="expected">The expected.</param>
        [Given(@"the result should match the expected (.*) response"),
        When(@"the result should match the expected (.*) response"),
        Then(@"the result should match the expected (.*) response")]
        public void ThenTheResultShouldMatchTheExpected(string expected)
        {
            string error = string.Empty;

            string expectedResponse = BaseFileUtils.GetFileContent(expected);
            string fileName = Path.GetFileNameWithoutExtension(expected).Replace("Template", "");
            BaseFileUtils.AttachFileToLog(expectedResponse, fileName, BaseFileUtils.EStreamType.Expected, BaseUtils.GetFileExtension(BaseUtils.EFormat.JSON), "Expected response");

            Assert.IsTrue(Common.CompareResponse(Response, expectedResponse, out error), "The JSON message received does not match the expected: " + error);
        }
    }
}
