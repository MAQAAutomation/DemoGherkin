using Demo.CommonFramework.Config;

namespace Demo.WebFW.Framework.Utils
{
    public static class UserService
    {
        /// <summary>
        /// Gets the test user.
        /// </summary>
        /// <returns></returns>
        public static User GetTestUser()
        {
            return new User()
            {
                UserName = RunSettings.User,
                Password = RunSettings.Password
            };
        }
    }
    public class User
    {
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName { get; set; }
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; set; }
    }
}
