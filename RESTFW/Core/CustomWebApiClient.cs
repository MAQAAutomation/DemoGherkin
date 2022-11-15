using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Text;
using Demo.CommonFramework;
using Demo.CommonFramework.ExceptionHandler;
using Demo.CommonFramework.Helpers;
using Demo.RestServiceFramework.Security;

namespace Demo.RestServiceFramework
{
    public class CustomWebApiClient : BaseCustomWeb
    {
        /// <summary>
        /// Gets or sets the json request.
        /// </summary>
        /// <value>
        /// The json request.
        /// </value>
        public string JsonRequest { get; set; } = null;
        /// <summary>
        /// Gets or sets the query parameter.
        /// </summary>
        /// <value>
        /// The query parameter.
        /// </value>
        public string QueryParam { get; set; } = null;
        /// <summary>
        /// Gets or sets the path parameter.
        /// </summary>
        /// <value>
        /// The path parameter.
        /// </value>
        public string PathParam { get; set; } = null;
        /// <summary>
        /// Gets or sets the HTTP response status.
        /// </summary>
        /// <value>
        /// The HTTP response status.
        /// </value>
        public HttpStatusCode HttpResponseStatus { get; set; }
        /// <summary>
        /// Gets or sets the authorization file path.
        /// </summary>
        /// <value>
        /// The authorization file path.
        /// </value>
        public string AuthorizationFilePath { get; set; } = null;

        /// <summary>
        /// Gets a value indicating whether [use expired token].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use expired token]; otherwise, <c>false</c>.
        /// </value>
        public bool UseExpiredToken { get; internal set; } = false;

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        /// <value>
        /// The token.
        /// </value>
        internal string Token { get; set; }

        /// <summary>
        /// Gets or sets the cookies.
        /// </summary>
        /// <value>
        /// The cookies.
        /// </value>
        internal CookieCollection Cookies { get; set; }

        /// <summary>
        /// Gets or sets the login authentication URL.
        /// </summary>
        /// <value>
        /// The login authentication URL.
        /// </value>
        public string LoginAuthURL { get; set; }

        /// <summary>
        /// Gets or sets the login server URL.
        /// </summary>
        /// <value>
        /// The login server URL.
        /// </value>
        public string LoginServerURL { get; set; }

        /// <summary>
        /// Gets or sets the form data.
        /// </summary>
        /// <value>
        /// The form data.
        /// </value>
        public byte[] FormData { get; set; }

        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        /// <value>
        /// The type of the content.
        /// </value>
        public string ContentType { get; set; }

        /// <summary>
        /// Send a request to the Rest Web Services and get the response of this call
        /// </summary>
        /// <exception cref="FrameworkException"></exception>
        public void CallWebService()
        {
            var url = Protocol.ToLower() + "://" + Service + EndPoint + PathParam + QueryParam;
            try
            {
                BaseHttpHelper.EHttpMethod httpMethod = (BaseHttpHelper.EHttpMethod)Enum.Parse(typeof(BaseHttpHelper.EHttpMethod), Action);
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(BaseHttpHelper.AcceptAllCertifications);

                HttpWebRequest webRequest = CreateWebApiRequest(url, httpMethod);

                if (!httpMethod.Equals(BaseHttpHelper.EHttpMethod.GET))
                {
                    if (FormData != null && FormData.Length > 0)
                    {
                        InsertFormIntoWebRequest(FormData, webRequest);
                    }
                    else
                    {
                        InsertJsonIntoWebRequest(JsonRequest, webRequest);
                    }
                }

                using (WebResponse webResponse = webRequest.GetResponse())
                {
                    SetResult((HttpWebResponse)webResponse);
                }
            }
            catch (WebException e)
            {
                SetResult((HttpWebResponse)e.Response);
            }
            catch (Exception e)
            {
                throw new FrameworkException(string.Format("Exception in the CallWebService method call to {0} . Messages:{1} ", url, e.ToString()), e);
            }
            finally
            {
                BaseFileUtils.AttachFileToLog(JsonRequest, BaseUtils.GetServiceName(Service)
                                        + EndPoint.Replace("/", ""), BaseFileUtils.EStreamType.Request
                                        , BaseFileUtils.EFileExtension.JSON, "Request sent");

                if (!string.IsNullOrEmpty(Result))
                {
                    BaseFileUtils.AttachFileToLog(Result, BaseUtils.GetServiceName(Service)
                        , BaseFileUtils.EStreamType.Response, BaseFileUtils.EFileExtension.JSON, "Response received");
                }
            }
        }

        /// <summary>
        /// Sets the result.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <exception cref="FrameworkException">There is no response from the server</exception>
        private void SetResult(HttpWebResponse response)
        {
            if (response == null) throw new FrameworkException("There is no response from the server");

            HttpWebResponse httpResponse = response;
            HttpResponseStatus = httpResponse.StatusCode;
            using (StreamReader rd = new StreamReader(response.GetResponseStream()))
            {
                Result = rd.ReadToEnd();
            }
        }

        /// <summary>
        /// Create a Rest Api Request
        /// </summary>
        /// <param name="url">To the Web Services</param>
        /// <param name="method">http rest verb</param>
        /// <returns></returns>
        /// <exception cref="FrameworkException">Failed to get the authorization: " + e.Message</exception>
        private HttpWebRequest CreateWebApiRequest(string url, BaseHttpHelper.EHttpMethod method)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.ServerCertificateValidationCallback = ServicePointManager.ServerCertificateValidationCallback;

            if (HeaderParams != null)
            {
                webRequest.Headers = HeaderParams;
            }

            webRequest.Method = method.ToString();
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.AuthenticationLevel = AuthenticationLevel.None;
            webRequest.Accept = @"application/json, text/plain, */*";
            webRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            webRequest.KeepAlive = true;

            if (!string.IsNullOrEmpty(Referer))
            {
                webRequest.Referer = Referer;
            }

            if (!string.IsNullOrEmpty(ContentType))
            {
                webRequest.ContentType = ContentType;
            }
            else if (JsonRequest != null)
            {
                if (BaseJsonHelper.IsValidJson(JsonRequest) || BaseJsonHelper.IsValidJsonArray(JsonRequest))
                {
                    webRequest.ContentType = "application/json";
                }
                else
                {
                    webRequest.ContentType = "application/x-www-form-urlencoded";
                }
            }
            else
            {
                webRequest.ContentLength = 0;
            }

            try
            {
                object authConfig = null;
                SecurityManager.EAuthorization authorization = SecurityManager.GetAuthorizationConfigured(ref authConfig, AuthorizationFilePath);
                switch (authorization)
                {
                    case SecurityManager.EAuthorization.OAuth2:
                        {
                            OAuth2Config oAuth2Config = (OAuth2Config)authConfig;
                            Token = SecurityManager.GetOAuth2TokenByClientCredentials(oAuth2Config.AccessURLToken, oAuth2Config.ClientId,
                                oAuth2Config.ClientKey, oAuth2Config.GetScopesString());

                            if (UseExpiredToken)
                            {
                                SecurityManager.GetOAuth2TokenByClientCredentials(oAuth2Config.AccessURLToken, oAuth2Config.ClientId,
                                oAuth2Config.ClientKey, oAuth2Config.GetScopesString());
                            }

                            webRequest.Headers.Add("Authorization", "Bearer " + Token);

                            break;
                        }
                    case SecurityManager.EAuthorization.Basic:
                        {
                            BasicAuthConfig basicAuthConfig = (BasicAuthConfig)authConfig;
                            Token = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(basicAuthConfig.User + ":" + basicAuthConfig.Password));
                            webRequest.Headers.Add("Authorization", "Basic " + Token);
                            break;
                        }
                    case SecurityManager.EAuthorization.Login:
                        {
                            string token = string.Empty;
                            LoginConfig loginConfig = (LoginConfig)authConfig;

                            LoginServerURL = string.IsNullOrEmpty(LoginServerURL) ? GetLoginServicesURL(loginConfig) : LoginServerURL;
                            LoginAuthURL = string.IsNullOrEmpty(LoginAuthURL) ? loginConfig.AuthUrl : LoginAuthURL;
                            Cookies = Cookies ?? SecurityManager.GetLoginCookies(LoginServerURL, LoginAuthURL, loginConfig.User,
                                loginConfig.Password, out token);

                            Token = Token ?? token;
                            webRequest.Headers.Add("Authorization", "Bearer " + Token);
                            webRequest.CookieContainer = new CookieContainer();
                            webRequest.CookieContainer.Add(Cookies);
                            break;
                        }
                    case SecurityManager.EAuthorization.LoginSPA:
                        {
                            LoginConfig loginConfig = (LoginConfig)authConfig;
                            LoginServerURL = string.IsNullOrEmpty(LoginServerURL) ? GetLoginServicesURL(loginConfig) : LoginServerURL;
                            LoginAuthURL = string.IsNullOrEmpty(LoginAuthURL) ? loginConfig.AuthUrl : LoginAuthURL;

                            Token = SecurityManager.GetLoginSPACookies(LoginServerURL, LoginAuthURL, loginConfig.User, loginConfig.Password);
                            webRequest.Headers.Add("Authorization", "Bearer " + Token);
                            break;
                        }
                    case SecurityManager.EAuthorization.RefreshToken:
                        {
                            RefreshTokenConfig refreshTokenConfig = (RefreshTokenConfig)authConfig;
                            Token = SecurityManager.GetRefreshToken(LoginServerURL, refreshTokenConfig.RefreshToken, refreshTokenConfig.Environment);
                            webRequest.Headers.Add("Authorization", "Bearer " + Token);
                            break;
                        }
                    case SecurityManager.EAuthorization.None:
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                throw new FrameworkException("Failed to get the authorization: " + e.Message);
            }

            return webRequest;
        }

        /// <summary>
        /// Gets the login services URL.
        /// </summary>
        /// <param name="loginConfig">The login configuration.</param>
        /// <returns></returns>
        private string GetLoginServicesURL(LoginConfig loginConfig)
        {
            //TODO modify this removed contains when the change will be change to adding MainService 
            string slash = loginConfig.ServerUrl.EndsWith("/") || (!string.IsNullOrEmpty(loginConfig.MainService) && loginConfig.MainService.StartsWith("/")) ? "" : "/";

            return !string.IsNullOrEmpty(loginConfig.MainService) && !loginConfig.ServerUrl.Contains(loginConfig.MainService)
                ? loginConfig.ServerUrl + slash + loginConfig.MainService : loginConfig.ServerUrl;
        }

        /// <summary>
        /// Insert the Json in paramaters into the Rest request
        /// </summary>
        /// <param name="json">the information in Json format for the body of the request </param>
        /// <param name="webRequest">The web request</param>
        private void InsertJsonIntoWebRequest(string json, HttpWebRequest webRequest)
        {
            using (Stream stream = webRequest.GetRequestStream())
            {
                stream.Write(Encoding.UTF8.GetBytes(json), 0, json.Length);
            }
        }

        /// <summary>
        /// Inserts the form into web request.
        /// </summary>
        /// <param name="formData">The form data.</param>
        /// <param name="webRequest">The web request.</param>
        private void InsertFormIntoWebRequest(Byte[] formData, HttpWebRequest webRequest)
        {
            using (Stream stream = webRequest.GetRequestStream())
            {
                stream.Write(formData, 0, formData.Length);
            }
        }
    }
}
