using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Demo.CommonFramework.Helpers;
using Demo.RestServiceFramework.Helpers;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

namespace Demo.RestServiceFramework.Security
{
    public static class SecurityManager
    {
        public enum EAuthorization
        {
            None,
            Basic,
            OAuth2,
            Login,
            LoginSPA,
            RefreshToken
        }

        /// <summary>
        /// Gets the authentication configured.
        /// </summary>
        /// <param name="auth">The authentication.</param>
        /// <param name="pathFile">The path file.</param>
        /// <returns></returns>
        public static EAuthorization GetAuthorizationConfigured(ref object auth, string pathFile = null)
        {
            string path = pathFile ?? "Authorization.config";
            return XmlHelper.LoadAuthenticationConfig(path, ref auth);
        }

        /// <summary>
        /// Gets the o auth2 token by client credentials.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="secret">The secret.</param>
        /// <param name="scope">The scope.</param>
        /// <returns></returns>
        public static string GetOAuth2TokenByClientCredentials(string url, string clientId, string secret, string scope = null)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = "POST";

            string data = "grant_type=client_credentials&client_id=" + clientId + "&client_secret=" + secret;
            data += !string.IsNullOrEmpty(scope) ? "&scope=" + scope : "";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            byte[] dataStream = Encoding.UTF8.GetBytes(data);
            httpWebRequest.ContentLength = dataStream.Length;
            Stream newStream = httpWebRequest.GetRequestStream();

            newStream.Write(dataStream, 0, dataStream.Length);
            newStream.Close();

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            string response;
            OAuth2AccessToken authAccessToken;
            JObject tokenJson;

            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                response = streamReader.ReadToEnd();
                tokenJson = JObject.Parse(response);
            }

            authAccessToken = new OAuth2AccessToken(tokenJson);
            return authAccessToken.AccessToken;
        }



        /// <summary>
        /// Gets the SPAAuth token by client credentials.
        /// </summary>
        /// <param name="mainServerUrl">The main server URL.</param>
        /// <param name="authUrl">The authentication URL.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public static string GetLoginSPACookies(string mainServerUrl, string authUrl, string username, string password)
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(BaseHttpHelper.AcceptAllCertifications);
            var cookieContainer = new CookieContainer();

            //Call de public settings 
            string jsonReponse = GetSystemConfigurationPublicSettings(mainServerUrl);
            string clientId = BaseJsonHelper.GetElementValueFromJsonString(jsonReponse, "$.clientId");
            string scope = BaseJsonHelper.GetElementValueFromJsonString(jsonReponse, "$.scopes");
            string authenticationServerUrl = BaseJsonHelper.GetElementValueFromJsonString(jsonReponse, "$.authenticationServerUrl");

            //Call connect authorize
            string data = "?client_id=" + clientId +
                          "&redirect_uri=" + WebUtility.UrlEncode(mainServerUrl) +
                          "&response_type=code" +
                          "&scope=" + WebUtility.UrlEncode(scope) +
                          "&nonce=" + CalculateNonce() +
                          "&state=" + "a1e958f3811a76511ec07be83f4e67ae90P2A7AYa" +
                          "&code_challenge=" + "9edDLRBf63zzkioknIl2tXRwax7cRq8FrwB_ivOGc-8" +
                          "&code_challenge_method=S256";

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(authenticationServerUrl + "/connect/authorize" + data);
            httpWebRequest.Method = BaseHttpHelper.EHttpMethod.GET.ToString();
            httpWebRequest.CookieContainer = cookieContainer;
            httpWebRequest.AllowAutoRedirect = true;
            httpWebRequest.ServicePoint.ConnectionLeaseTimeout = 5000;
            HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            var htmlBody = GetHtmlDocFromResponse(httpResponse)?.DocumentNode.SelectSingleNode("//body");
            string returnUrl = WebUtility.HtmlDecode(htmlBody?.SelectSingleNode("//input[@name='ReturnUrl']").GetAttributeValue("value", string.Empty));
            var verificationToken = htmlBody?.SelectSingleNode("//input[@name='__RequestVerificationToken']").GetAttributeValue("value", string.Empty);
            httpResponse.Close();

            SetUserAndPasswordSPA(authUrl, returnUrl, username, password, verificationToken, cookieContainer, out HttpWebResponse httpResponseUserPass);

            Uri uriLocation = httpResponseUserPass.ResponseUri;
            string code = HttpUtility.ParseQueryString(uriLocation.Query).Get("code");
            scope = HttpUtility.ParseQueryString(uriLocation.Query).Get("scope");
            string state = HttpUtility.ParseQueryString(uriLocation.Query).Get("state");
            string sessionState = HttpUtility.ParseQueryString(uriLocation.Query).Get("session_state");
            httpResponseUserPass.Close();

            return GetSPAToken(authenticationServerUrl, clientId, mainServerUrl, code, scope, state, sessionState, cookieContainer);
        }


        /// <summary>Gets the refresh token.</summary>
        /// <param name="serverUrl">The server URL.</param>
        /// <param name="refreshToken">The refresh token.</param>
        /// <param name="environment">The environment.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public static string GetRefreshToken(string serverUrl, string refreshToken, string environment)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(serverUrl + "/sync/" + environment + "bucket/_oidc_refresh?refresh_token=" + refreshToken);
            httpWebRequest.Method = BaseHttpHelper.EHttpMethod.GET.ToString();
            httpWebRequest.AllowAutoRedirect = true;
            httpWebRequest.ServicePoint.ConnectionLeaseTimeout = 5000;
            HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            string jsonReponse = string.Empty;

            using (WebResponse response = httpWebRequest.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    jsonReponse = reader.ReadToEnd();
                    reader.Close();
                }
                response.Close();
            }
            return jsonReponse;
        }

        /// <summary>
        /// Gets the login cookies.
        /// </summary>
        /// <param name="mainServerUrl">The main server URL.</param>
        /// <param name="authUrl">The authentication URL.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public static CookieCollection GetLoginCookies(string mainServerUrl, string authUrl, string username, string password, out string token)
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(BaseHttpHelper.AcceptAllCertifications);

            var cookieContainer = new CookieContainer();

            OpenFOH(mainServerUrl, cookieContainer, out HttpWebResponse httpResponse);

            var htmlBody = GetHtmlDocFromResponse(httpResponse)?.DocumentNode.SelectSingleNode("//body");
            var returnUrl = WebUtility.HtmlDecode(htmlBody?.SelectSingleNode("//input[@name='ReturnUrl']").GetAttributeValue("value", string.Empty));
            var verificationToken = htmlBody?.SelectSingleNode("//input[@name='__RequestVerificationToken']").GetAttributeValue("value", string.Empty);

            SetUserAndPassword(authUrl, returnUrl, username, password, verificationToken, cookieContainer, out HttpWebResponse httpResponseUserPass);
            token = GetToken(httpResponseUserPass, authUrl, returnUrl, cookieContainer, out string action, out string code, out string scope, out string state,
                out string sessionState, out string accessToken, out string expiresIn, out string tokenType);

            CookieCollection cookieCollection = GetCookies(httpResponseUserPass, authUrl, cookieContainer, action, token, scope, code, state, sessionState, accessToken, expiresIn, tokenType);
            cookieCollection.Add(cookieContainer.GetCookies(new Uri(mainServerUrl)));

            return cookieCollection;
        }

        /// <summary>
        /// Gets the cookies.
        /// </summary>
        /// <param name="httpResponseUserPass">The HTTP response user pass.</param>
        /// <param name="authUrl">The authentication URL.</param>
        /// <param name="cookieContainer">The cookie container.</param>
        /// <param name="action">The action.</param>
        /// <param name="token">The token.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="code">The code.</param>
        /// <param name="state">The state.</param>
        /// <param name="sessionState">State of the session.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="expiresIn">The expires in.</param>
        /// <param name="tokenType">Type of the token.</param>
        /// <returns></returns>
        private static CookieCollection GetCookies(HttpWebResponse httpResponseUserPass, string authUrl, CookieContainer cookieContainer
            , string action, string token, string scope, string code, string state, string sessionState, string accessToken, string expiresIn, string tokenType)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(action);
            httpWebRequest.Method = BaseHttpHelper.EHttpMethod.POST.ToString();
            httpWebRequest.AllowAutoRedirect = false;
            httpWebRequest.Referer = authUrl + httpResponseUserPass.Headers["Location"];
            httpWebRequest.CookieContainer = cookieContainer;
            httpWebRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            string data = "code=" + code +
                          "&id_token=" + token +
                          "&access_token=" + accessToken +
                          "&token_type=" + tokenType +
                          "&expires_in=" + expiresIn +
                          "&scope=" + scope +
                          "&state=" + state +
                          "&session_state=" + sessionState;

            byte[] dataStream = Encoding.UTF8.GetBytes(data);
            httpWebRequest.ContentLength = dataStream.Length;
            Stream newStream = httpWebRequest.GetRequestStream();
            newStream.Write(dataStream, 0, dataStream.Length);
            newStream.Close();
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            return httpResponse.Cookies;
        }

        /// <summary>
        /// Gets the token.
        /// </summary>
        /// <param name="httpResponseUserPass">The HTTP response user pass.</param>
        /// <param name="authUrl">The authentication URL.</param>
        /// <param name="returnUrl">The return URL.</param>
        /// <param name="cookieContainer">The cookie container.</param>
        /// <param name="action">The action.</param>
        /// <param name="code">The code.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="state">The state.</param>
        /// <param name="sessionState">State of the session.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="expiresIn">The expires in.</param>
        /// <param name="tokenType">Type of the token.</param>
        /// <returns></returns>
        private static string GetToken(HttpWebResponse httpResponseUserPass, string authUrl, string returnUrl, CookieContainer cookieContainer
            , out string action, out string code, out string scope, out string state, out string sessionState, out string accessToken, out string expiresIn, out string tokenType)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(authUrl + httpResponseUserPass.Headers["Location"]);
            httpWebRequest.Method = BaseHttpHelper.EHttpMethod.GET.ToString();
            httpWebRequest.AllowAutoRedirect = false;
            httpWebRequest.Referer = authUrl + "/Authentication/Login/?returnUrl=" + returnUrl;
            httpWebRequest.CookieContainer = cookieContainer;
            httpWebRequest.KeepAlive = true;
            httpWebRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3";
            httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/77.0.3865.90 Safari/537.36";
            httpWebRequest.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate, br");
            httpWebRequest.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-GB,en-US;q=0.9,en;q=0.8");
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            var htmlBody = GetHtmlDocFromResponse(httpResponse)?.DocumentNode.SelectSingleNode("//body");
            action = htmlBody?.SelectSingleNode("//form[@method='post']").GetAttributeValue("action", string.Empty);
            var token = htmlBody?.SelectSingleNode("//input[@name='id_token']").GetAttributeValue("value", string.Empty);
            code = htmlBody?.SelectSingleNode("//input[@name='code']").GetAttributeValue("value", string.Empty);
            scope = htmlBody?.SelectSingleNode("//input[@name='scope']").GetAttributeValue("value", string.Empty);
            state = htmlBody?.SelectSingleNode("//input[@name='state']").GetAttributeValue("value", string.Empty);
            sessionState = htmlBody?.SelectSingleNode("//input[@name='session_state']").GetAttributeValue("value", string.Empty);

            accessToken = htmlBody?.SelectSingleNode("//input[@name='access_token']")?.GetAttributeValue("value", string.Empty);
            expiresIn = htmlBody?.SelectSingleNode("//input[@name='expires_in']")?.GetAttributeValue("value", string.Empty);
            tokenType = htmlBody?.SelectSingleNode("//input[@name='token_type']")?.GetAttributeValue("value", string.Empty);


            return token;
        }

        /// <summary>
        /// Gets the HTML document from response.
        /// </summary>
        /// <param name="httpResponse">The HTTP response.</param>
        /// <returns></returns>
        private static HtmlDocument GetHtmlDocFromResponse(HttpWebResponse httpResponse)
        {
            var htmlDoc = new HtmlDocument();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                string response = streamReader.ReadToEnd();
                htmlDoc.LoadHtml(response);
            }

            return htmlDoc;
        }

        /// <summary>
        /// Sets the user and password.
        /// </summary>
        /// <param name="authUrl">The authentication URL.</param>
        /// <param name="returnUrl">The return URL.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="verificationToken">The verification token.</param>
        /// <param name="cookieContainer">The cookie container.</param>
        /// <param name="httpResponse">The HTTP response.</param>
        private static void SetUserAndPassword(string authUrl, string returnUrl, string username, string password, string verificationToken,
            CookieContainer cookieContainer, out HttpWebResponse httpResponse)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(authUrl + "/Authentication/Login/?returnUrl=" + WebUtility.UrlEncode(returnUrl));
            httpWebRequest.Method = BaseHttpHelper.EHttpMethod.POST.ToString();
            httpWebRequest.AllowAutoRedirect = false;
            httpWebRequest.Referer = authUrl + "/Authentication/Login?returnUrl=" + WebUtility.UrlEncode(returnUrl);
            httpWebRequest.CookieContainer = cookieContainer;
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/77.0.3865.90 Safari/537.36";
            httpWebRequest.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate, br");
            httpWebRequest.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-GB,en-US;q=0.9,en;q=0.8");
            httpWebRequest.Headers.Add("Origin", authUrl);

            string data = "ReturnUrl=" + WebUtility.UrlEncode(returnUrl) +
                          "&Username=" + username +
                          "&Password=" + WebUtility.UrlEncode(password) +
                          "&button=login&__RequestVerificationToken=" + verificationToken;
            byte[] dataStream = Encoding.UTF8.GetBytes(data);
            httpWebRequest.ContentLength = dataStream.Length;
            Stream newStream = httpWebRequest.GetRequestStream();
            newStream.Write(dataStream, 0, dataStream.Length);
            newStream.Close();
            httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        }

        /// <summary>
        /// Sets the user and password.
        /// </summary>
        /// <param name="authUrl">The authentication URL.</param>
        /// <param name="returnUrl">The return URL.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="verificationToken">The verification token.</param>
        /// <param name="cookieContainer">The cookie container.</param>
        /// <param name="httpResponse">The HTTP response.</param>
        private static void SetUserAndPasswordSPA(string authUrl, string returnUrl, string username, string password, string verificationToken,
            CookieContainer cookieContainer, out HttpWebResponse httpResponse)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(authUrl + "/Authentication?returnUrl=" + WebUtility.UrlEncode(returnUrl));
            httpWebRequest.Method = BaseHttpHelper.EHttpMethod.POST.ToString();
            httpWebRequest.AllowAutoRedirect = true;
            httpWebRequest.Referer = authUrl + "/Authentication?returnUrl=" + WebUtility.UrlEncode(returnUrl);
            httpWebRequest.CookieContainer = cookieContainer;
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/77.0.3865.90 Safari/537.36";
            httpWebRequest.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate, br");
            httpWebRequest.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-GB,en-US;q=0.9,en;q=0.8");
            httpWebRequest.Headers.Add("Origin", authUrl);
            httpWebRequest.ServicePoint.ConnectionLeaseTimeout = 5000;

            string data = "ReturnUrl=" + WebUtility.UrlEncode(returnUrl) +
                          "&Username=" + username +
                          "&Password=" + WebUtility.UrlEncode(password) +
                          "&button=login&__RequestVerificationToken=" + verificationToken;
            byte[] dataStream = Encoding.UTF8.GetBytes(data);
            httpWebRequest.ContentLength = dataStream.Length;
            Stream newStream = httpWebRequest.GetRequestStream();
            newStream.Write(dataStream, 0, dataStream.Length);
            newStream.Close();
            httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        }

        /// <summary>
        /// Opens the foh.
        /// </summary>
        /// <param name="mainServerUrl">The main server URL.</param>
        /// <param name="cookieContainer">The cookie container.</param>
        /// <param name="httpResponse">The HTTP response.</param>
        private static void OpenFOH(string mainServerUrl, CookieContainer cookieContainer, out HttpWebResponse httpResponse)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(mainServerUrl);
            httpWebRequest.Method = BaseHttpHelper.EHttpMethod.GET.ToString();
            httpWebRequest.CookieContainer = cookieContainer;
            httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        }

        /// <summary>Gets the spa token.</summary>
        /// <param name="authUrl">The authentication URL.</param>
        /// <param name="clientID">The client identifier.</param>
        /// <param name="redirectUri">The redirect URI.</param>
        /// <param name="code">The code.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="state">The state.</param>
        /// <param name="sessionState">State of the session.</param>
        /// <param name="cookieContainer">The cookie container.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        private static string GetSPAToken(string authUrl, string clientID, string redirectUri, string code, string scope, string state, string sessionState, CookieContainer cookieContainer)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(authUrl + "/connect/token");
            httpWebRequest.Method = BaseHttpHelper.EHttpMethod.POST.ToString();
            httpWebRequest.AllowAutoRedirect = true;
            httpWebRequest.CookieContainer = cookieContainer;
            httpWebRequest.KeepAlive = true;
            httpWebRequest.Accept = "application/json, text/plain, */*";
            httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/77.0.3865.90 Safari/537.36";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate, br");
            httpWebRequest.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-GB,en-US;q=0.9,en;q=0.8");
            httpWebRequest.Headers.Add("code", code);
            httpWebRequest.Headers.Add("scope", scope);
            httpWebRequest.Headers.Add("state", state);
            httpWebRequest.Headers.Add("session_state", sessionState);
            httpWebRequest.CookieContainer = cookieContainer;
            httpWebRequest.ServicePoint.ConnectionLeaseTimeout = 5000;
            string data = "grant_type=authorization_code" +
                          "&client_id=" + clientID +
                          "&code_verifier=4471b50453dd2c510fa37078e95a9ae0084ff66dc3762754dfc9634e94fckdVf7Z7" +
                          "&code=" + code +
                          "&redirect_uri=" + WebUtility.UrlEncode(redirectUri);

            Byte[] dataStream = Encoding.UTF8.GetBytes(data);
            httpWebRequest.ContentLength = dataStream.Length;
            Stream newStream = httpWebRequest.GetRequestStream();
            newStream.Write(dataStream, 0, dataStream.Length);
            newStream.Close();
            HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            string jsonReponse = string.Empty;

            using (WebResponse response = httpWebRequest.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    jsonReponse = reader.ReadToEnd();
                    reader.Close();
                }
            }

            string token = BaseJsonHelper.GetElementValueFromJsonString(jsonReponse, "$.access_token");
            httpResponse.Close();
            return token;
        }

        /// <summary>
        /// Get SystemConfiguration public sttings.
        /// </summary>
        /// <param name="mainServerUrl">The main server URL.</param>
        private static string GetSystemConfigurationPublicSettings(string mainServerUrl)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(mainServerUrl + "/api/SystemConfiguration/PublicSettings");
            httpWebRequest.Method = BaseHttpHelper.EHttpMethod.GET.ToString();
            httpWebRequest.Referer = mainServerUrl;
            httpWebRequest.ContentType = "application/json; charset=UTF-8";
            httpWebRequest.Accept = "application/json";
            httpWebRequest.KeepAlive = false;
            httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/77.0.3865.90 Safari/537.36";
            httpWebRequest.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate, br");
            httpWebRequest.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-GB,en-US;q=0.9,en;q=0.8");
            httpWebRequest.ServicePoint.ConnectionLeaseTimeout = 5000;


            string jsonReponse = string.Empty;

            using (WebResponse response = httpWebRequest.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    jsonReponse = reader.ReadToEnd();
                    reader.Close();
                }
                response.Close();

            }

            return jsonReponse;
        }

        /// <summary>Calculates the nonce.</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        private static string CalculateNonce()
        {
            //Allocate a buffer
            var ByteArray = new byte[20];
            //Generate a cryptographically random set of bytes
            using (var Rnd = RandomNumberGenerator.Create())
            {
                Rnd.GetBytes(ByteArray);
            }
            //Base64 encode and then return
            return Convert.ToBase64String(ByteArray);
        }
    }
}
