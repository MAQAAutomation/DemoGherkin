using System.Collections.Generic;

namespace Demo.RestServiceFramework.Security
{
    public class OAuth2Config
    {
        /// <summary>
        /// Gets the access URL token.
        /// </summary>
        /// <value>
        /// The access URL token.
        /// </value>
        public string AccessURLToken { get; private set; }
        /// <summary>
        /// Gets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        public string ClientId { get; private set; }
        /// <summary>
        /// Gets the client key.
        /// </summary>
        /// <value>
        /// The client key.
        /// </value>
        public string ClientKey { get; private set; }
        /// <summary>
        /// Gets the scopes.
        /// </summary>
        /// <value>
        /// The scopes.
        /// </value>
        public List<string> Scopes { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuth2Config"/> class.
        /// </summary>
        /// <param name="accessUrl">The access URL.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="clientKey">The client key.</param>
        /// <param name="scopes">The scopes.</param>
        public OAuth2Config(string accessUrl, string clientId, string clientKey, List<string> scopes)
        {
            AccessURLToken = accessUrl;
            ClientId = clientId;
            ClientKey = clientKey;
            Scopes = scopes;
        }

        /// <summary>
        /// Gets the scopes in a string with whitespaces between.
        /// </summary>
        /// <returns></returns>
        public string GetScopesString()
        {
            string scopeList = string.Empty;

            foreach(var scope in Scopes)
            {
                scopeList += scope + " ";
            }

            return scopeList;
        }
    }
}
