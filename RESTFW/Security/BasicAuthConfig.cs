namespace Demo.RestServiceFramework.Security
{
    public class BasicAuthConfig
    {
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
        /// Initializes a new instance of the <see cref="BasicAuthConfig"/> class.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="password">The password.</param>
        public BasicAuthConfig(string user, string password)
        {
            User = user;
            Password = password;
        }
    }
}
