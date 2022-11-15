namespace Demo.RestServiceFramework.Security
{
    public class LoginConfig
    {
        /// <summary>
        /// Gets the server URL.
        /// </summary>
        /// <value>
        /// The server URL.
        /// </value>
        public string ServerUrl { get; private set; }
        /// <summary>
        /// Gets the authentication URL.
        /// </summary>
        /// <value>
        /// The authentication URL.
        /// </value>
        public string AuthUrl { get; private set; }
        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        public string User { get; private set; }
        /// <summary>
        /// Gets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; private set; }

        /// <summary>
        /// Gets the main service.
        /// </summary>
        /// <value>
        /// The main service.
        /// </value>
        public string MainService { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginConfig" /> class.
        /// </summary>
        /// <param name="serverUrl">The server URL.</param>
        /// <param name="user">The user.</param>
        /// <param name="password">The password.</param>
        ///  /// <param name="password">The password.</param>
        public LoginConfig(string serverUrl, string authUrl, string user, string password, string mainService)
        {
            ServerUrl = serverUrl;
            AuthUrl = authUrl;
            User = user;
            Password = password;
            MainService = mainService;
        }
    }
}
