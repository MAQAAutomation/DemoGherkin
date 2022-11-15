namespace Demo.RestServiceFramework.Security
{
    public class RefreshTokenConfig
    {
        /// <summary>
        /// Gets the server URL.
        /// </summary>
        /// <value>
        /// The server URL.
        /// </value>
        public string ServerUrl { get; private set; }
        /// <summary>
        /// Gets the refresh token.
        /// </summary>
        /// <value>
        /// The refresh Token
        /// </value>
        public string RefreshToken { get; private set; }

        /// <summary>Gets the environment.</summary>
        /// <value>The environment.</value>
        public string Environment { get; private set; }


        /// <summary>Initializes a new instance of the <see cref="RefreshTokenConfig" /> class.</summary>
        /// <param name="serverUrl">The server URL.</param>
        /// <param name="refreshToken">The refresh token.</param>
        /// <param name="environment">The environment.</param>
        public RefreshTokenConfig(string serverUrl, string refreshToken, string environment)
        {
            ServerUrl = serverUrl;
            RefreshToken = refreshToken;
            Environment = environment;
        }
    }
}
