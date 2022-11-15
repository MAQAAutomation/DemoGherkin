using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using Demo.CommonFramework.ExceptionHandler;
using Demo.CommonFramework.Helpers;

namespace Demo.RestServiceFramework
{
    /// <summary>
    /// 
    /// </summary>
    public class WebHookManager
    {
        private WebHookListener listener;
        private const string WEB_HOOK_URL = "http://<IP>:<PORT>/test/";
        private const string WEB_HOOK_FIELD = "webHookUrl";
        private string m_url = null;
        private int m_localPort = 0;

        public int Timeout { get; private set; }

        /// <summary>
        /// Gets the response.
        /// </summary>
        /// <value>
        /// The response.
        /// </value>
        public string Response { get; private set; } = null;

        /// <summary>
        /// Gets the local port.
        /// </summary>
        /// <value>
        /// The local port.
        /// </value>
        private int LocalPort
        {
            get
            {
                if (m_localPort == 0)
                {
                    m_localPort = BaseUtils.GetLocalPortAvailable();
                }
                return m_localPort;
            }
        }

        /// <summary>
        /// Gets the URL template.
        /// </summary>
        /// <value>
        /// The URL template.
        /// </value>
        private string UrlTemplate
        {
            get
            {
                if (m_url == null)
                {
                    m_url = WEB_HOOK_URL + BaseUtils.RandomizeMessagesID().Replace("-", "") + "/";
                }
                return m_url;
            }
        }

        /// <summary>
        /// Gets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        public string Url
        {
            get
            {
                return UrlTemplate.Replace("<IP>", BaseUtils.GetLocalIPAddress()).Replace("<PORT>", LocalPort.ToString());
            }
        }

        /// <summary>
        /// Starts the server.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <exception cref="InconclusiveFrameworkException">Unable to start listener! Error: " + hlex.Message</exception>
        public void StartServer(int timeout = 150000)
        {
            Timeout = timeout;
            var httpListener = new HttpListener();
            listener = new WebHookListener(httpListener, Url.Replace(BaseUtils.GetLocalIPAddress(), "*"), ProcessResponse);
            try
            {
                listener.Start();
            }
            catch (HttpListenerException hlex)
            {
                throw new InconclusiveFrameworkException("Unable to start listener! Error: " + hlex.Message);
            }
        }

        /// <summary>
        /// Determines whether this instance is started.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is started; otherwise, <c>false</c>.
        /// </returns>
        public bool IsStarted()
        {
            return listener != null && listener.IsActive();
        }

        /// <summary>
        /// Processes the response.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns></returns>
        public byte[] ProcessResponse(string response)
        {
            Response = response;
            BaseFileUtils.AttachFileToLog(Response, "WebHook_" + Url.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Last(), BaseFileUtils.EStreamType.Response, BaseFileUtils.EFileExtension.JSON, "Response received into the webhook");
            return Encoding.ASCII.GetBytes(response);
        }

        /// <summary>
        /// Waits for response.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        /// <exception cref="FrameworkException">Timeout: There are no messages received in the webhook: " + Url</exception>
        internal bool WaitForResponse(string result = null)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            string reqId = BaseJsonHelper.GetElementValueFromJsonString(result, "requestId");

            while (string.IsNullOrEmpty(Response) || (Response != null && !Response.Contains(reqId)))
            {
                if (sw.ElapsedMilliseconds > Timeout) throw new FrameworkException("Timeout: There are no messages received in the webhook: " + Url);
            }

            return true;
        }

        /// <summary>
        /// Adds the web hook element to json request.
        /// </summary>
        /// <param name="jsonRequest">The json request.</param>
        internal void AddWebHookElementToJsonRequest(ref string jsonRequest)
        {
            BaseJsonHelper.AddSingleElementToJsonString(ref jsonRequest, "", WEB_HOOK_FIELD, Url);
        }
    }
}
