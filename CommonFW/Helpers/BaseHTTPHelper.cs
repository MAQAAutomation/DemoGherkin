using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Demo.CommonFramework.Helpers
{
    public class BaseHttpHelper
    {
        protected BaseHttpHelper() { }

        public enum EHttpMethod
        {
            /// <summary>
            /// The HTTP GET method is used to **read** (or retrieve) a representation of a resource
            /// </summary>
            GET,
            /// <summary>
            /// The POST verb is most-often utilized to **create** new resources
            /// </summary>
            POST,
            /// <summary>
            /// PUT is most-often utilized for **update** capabilities
            /// </summary>
            PUT,
            /// <summary>
            /// PATCH is used for **modify** capabilities.The PATCH request only needs to contain the changes to the resource, not the complete resource
            /// </summary>
            PATCH,
            /// <summary>
            /// DELETE is pretty easy to understand. It is used to **delete** a resource identified by a URI.
            /// </summary>
            DELETE
        };


        /// <summary>
        /// Accepts all certifications.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="certification">The certification.</param>
        /// <param name="chain">The chain.</param>
        /// <param name="sslPolicyErrors">The SSL policy errors.</param>
        /// <returns></returns>
        public static bool AcceptAllCertifications(object sender, X509Certificate certification, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
