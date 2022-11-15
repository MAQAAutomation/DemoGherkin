using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Text.RegularExpressions;
using Demo.CommonFramework.ExceptionHandler;
using Demo.CommonFramework.Helpers;

namespace Demo.CommonFramework
{
    public class BaseCustomWeb
    {
        /// <summary>
        /// Gets or sets the protocol.
        /// </summary>
        /// <value>
        /// The protocol.
        /// </value>
        public string Protocol { get; set; } = null;
        /// <summary>
        /// Gets or sets the server.
        /// </summary>
        /// <value>
        /// The server.
        /// </value>
        public string Server { get; set; } = null;
        /// <summary>
        /// Gets or sets the service.
        /// </summary>
        /// <value>
        /// The service.
        /// </value>
        public string Service { get; set; } = null;
        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        public string Result { get; set; } = null;
        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>
        /// The action.
        /// </value>
        public string Action { get; set; } = null;
        /// <summary>
        /// Gets or sets the end point.
        /// </summary>
        /// <value>
        /// The end point.
        /// </value>
        public string EndPoint { get; set; } = null;
        /// <summary>
        /// Gets or sets the header parameters.
        /// </summary>
        /// <value>
        /// The header parameters.
        /// </value>
        public WebHeaderCollection HeaderParams { get; set; } = null;
        /// <summary>
        /// Gets or sets the referer.
        /// </summary>
        /// <value>
        /// The referer.
        /// </value>
        public string Referer { get; set; } = null;

        public BaseCustomWeb() { }

        /// <summary>
        /// Checks if web service available.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="service">The service.</param>
        /// <param name="protocol">The protocol.</param>
        /// <param name="endPoint">The endPoint.</param>
        /// <returns></returns>
        public bool CheckIfWebServiceAvailable(string server, string service, string protocol, string endPoint)
        {
            if (string.IsNullOrEmpty(service) || string.IsNullOrEmpty(protocol)) throw new FrameworkException("At least service and protocol shall not be empty");

            Server = string.IsNullOrEmpty(server) ? Server : server;
            Service = service;
            Protocol = protocol;
            EndPoint = string.Empty;
            if (!string.IsNullOrEmpty(endPoint))
            {
                EndPoint = (endPoint.StartsWith("/") ? "" : "/") + endPoint;
            }

            var url = Protocol + "://" + (!string.IsNullOrEmpty(server) ? (server + "/") : string.Empty) + Service + EndPoint;
            HttpWebResponse response = null;
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(BaseHttpHelper.AcceptAllCertifications);

                var request = (HttpWebRequest)WebRequest.Create(url);
                if (HeaderParams != null)
                {
                    request.Headers = HeaderParams;
                }
                using (response = (HttpWebResponse)request.GetResponse())
                {
                    return response.StatusCode.Equals(HttpStatusCode.OK);
                }
            }
            catch (WebException e)
            {
                response = response ?? (HttpWebResponse)e.Response;
                //  not available at all, for some reason
                return (response != null) && (response.StatusCode.CompareTo(HttpStatusCode.InternalServerError) < 0);
            }
            catch (Exception e)
            {
                throw new FrameworkException(
                    string.Format("CheckIfWebServiceAvailable Error call the URL {0}. Messages: {1}", url, e.ToString()), e);
            }
        }

        /// <summary>
        /// Adds the parameter to the header.
        /// </summary>
        /// <param name="headerParams">The header parameters.</param>
        public void AddParameterToTheHeader(IEnumerable<string> headerParams)
        {
            WebHeaderCollection webHeaderCollection = new WebHeaderCollection();
            Regex rx = new Regex(@"(.+?):");

            foreach (string param in headerParams)
            {
                string pairKeyValue = (param.Contains("=") ? param.Replace('=', ':') : param);
                if (string.IsNullOrEmpty(pairKeyValue)) continue;
                Match key = rx.Match(pairKeyValue);

                if (!WebHeaderCollection.IsRestricted(key.Groups[1].Value))
                {
                    webHeaderCollection.Add(pairKeyValue);
                }
            }

            HeaderParams = webHeaderCollection;
        }
    }
}
