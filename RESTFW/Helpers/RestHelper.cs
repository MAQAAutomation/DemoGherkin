using System;
using System.Collections.Generic;
using System.Net;
using Demo.CommonFramework.Config;
using Demo.CommonFramework.Helpers;

namespace Demo.RestServiceFramework.Helpers
{
    public static class RestHelper
    {
        /// <summary>
        /// Gets or sets the current token.
        /// </summary>
        /// <value>
        /// The current token.
        /// </value>
        private static string CurrentToken { get; set; } = null;

        /// <summary>
        /// Gets or sets the current cookies.
        /// </summary>
        /// <value>
        /// The current cookies.
        /// </value>
        private static CookieCollection CurrentCookies { get; set; } = null;

        /// <summary>
        /// Performs the request.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="referer">The referer.</param>
        /// <param name="method">The method.</param>
        /// <param name="webApiClient">The web API client.</param>
        /// <param name="jsonString">The json string.</param>
        /// <param name="pathParams">The path parameters.</param>
        /// <param name="queryParams">The query parameters.</param>
        /// <param name="reUseAuthorization">if set to <c>true</c> [re use authorization].</param>
        /// <param name="fullFilePath">The full file path.</param>
        public static void PerformRequest(string endpoint
                                        , IEnumerable<string> headers
                                        , string referer
                                        , BaseHttpHelper.EHttpMethod method
                                        , out CustomWebApiClient webApiClient
                                        , string jsonString = null
                                        , IEnumerable<string> pathParams = null
                                        , IEnumerable<string> queryParams = null
                                        , bool reUseAuthorization = true
                                        )
        {
            webApiClient = new CustomWebApiClient
            {
                LoginServerURL = RunSettings.EnvironmentURL,
                LoginAuthURL = RunSettings.SecurityServerURL,
                Server = RunSettings.PredefinedEnvironment,
                Service = RunSettings.PredefinedEnvironment,
                Protocol = "HTTPS",
                Cookies = CurrentCookies,
                Token = CurrentToken
            };

            webApiClient.AddParameterToTheHeader(headers);
            webApiClient.Referer = referer;
            webApiClient.Action = method.ToString();
            webApiClient.EndPoint = endpoint;
            webApiClient.PathParam = BaseUtils.GetParameterListAsString(pathParams, BaseUtils.EParamType.PathParam);
            webApiClient.QueryParam = BaseUtils.GetParameterListAsString(queryParams, BaseUtils.EParamType.QueryParam);

            if (!method.Equals(BaseHttpHelper.EHttpMethod.GET))
            {
                webApiClient.JsonRequest = jsonString;
            }
            try
            {
                webApiClient.CallWebService();
                if (reUseAuthorization)
                {
                    CurrentCookies = webApiClient.Cookies;
                    CurrentToken = webApiClient.Token;
                }
            }
            catch (Exception e)
            {
                if (!e.Message.Equals("The given key was not present in the dictionary.")) throw e;
            }
        }
    }
}
