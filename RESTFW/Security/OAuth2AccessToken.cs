using Newtonsoft.Json.Linq;

namespace Demo.RestServiceFramework.Security
{
    public class OAuth2AccessToken
    {
        private const string ACCESS_TOKEN = "access_token";
        private const string EXPIRE_IN = "expire_in";
        private const string TOKEN_TYPE = "token_type";

        /// <summary>
        /// Gets the access token.
        /// </summary>
        /// <value>
        /// The access token.
        /// </value>
        public string AccessToken { get; private set; }
        /// <summary>
        /// Gets the expire in.
        /// </summary>
        /// <value>
        /// The expire in.
        /// </value>
        public int ExpireIn { get; private set; }
        /// <summary>
        /// Gets the type of the token.
        /// </summary>
        /// <value>
        /// The type of the token.
        /// </value>
        public string TokenType { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuth2AccessToken"/> class.
        /// </summary>
        /// <param name="tokenJson">The token json.</param>
        public OAuth2AccessToken(JObject tokenJson)
        {
            AccessToken = tokenJson.Value<string>(ACCESS_TOKEN);
            ExpireIn = tokenJson.Value<int>(EXPIRE_IN);
            TokenType = tokenJson.Value<string>(TOKEN_TYPE);
        }
    }
}
