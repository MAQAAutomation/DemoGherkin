using System.Net;
using Demo.TFSAutomationFramework.Configuration;
using RestSharp;

namespace Demo.TFSAutomationFramework.Clients
{
    public abstract class AbstractAPIClient
    {
        protected string Area { get; set; }
        protected RestClient Client { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractAPIClient"/> class.
        /// </summary>
        protected AbstractAPIClient()
        {
            Client = new RestClient(TFSAutomationApiClientConfig.BaseTFSUrl)
            {
                CookieContainer = new CookieContainer()
            };
        }
    }
}
