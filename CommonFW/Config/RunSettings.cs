using System;
using NUnit.Framework;

namespace Demo.CommonFramework.Config
{
    public class RunSettings
    {
        #region Common
        /// <summary>
        /// Gets the backend server.
        /// </summary>
        /// <value>
        /// The backend server.
        /// </value>
        public static string BackendServer => TestContext.Parameters.Get(RunSettingsKeys.BackendServer);

        /// <summary>
        /// Gets the name of the test file.
        /// </summary>
        /// <value>
        /// The name of the test file.
        /// </value>
        public static string TestFileName
        {
            get
            {
                return TestContext.CurrentContext.Test.Name + DateTime.Now.ToString("_yyyyMMdd_HHmmssfff") + ".html";
            }
        }

        /// <summary>
        /// Gets the test log level.
        /// </summary>
        /// <value>
        /// The test log level.
        /// </value>
        public static string TestLogLevel => TestContext.Parameters.Get(RunSettingsKeys.TestLogLevel);

        /// <summary>
        /// Gets a value indicating whether [test result format TFS].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [test result format TFS]; otherwise, <c>false</c>.
        /// </value>
        public static bool TestResultFormatTFS => "TFS".Equals(TestContext.Parameters.Get(RunSettingsKeys.TestResultFormat));

        /// <summary>
        /// Gets a value indicating whether [creation initial data universe].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [creation initial data universe]; otherwise, <c>false</c>.
        /// </value>
        public static bool CreationInitialData => bool.Parse(TestContext.Parameters.Get(RunSettingsKeys.CreationInitialData));

        /// <summary>
        /// Gets a value indicating whether [enviroment url].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [environment url]; otherwise, <c>false</c>.
        /// </value>
        public static string EnvironmentURL => TestContext.Parameters.Get(RunSettingsKeys.EnvironmentURL);
        /// <summary>
        /// Gets the WorkflowURL.
        /// </summary>
        /// <value>
        /// The WorkflowURL.
        /// </value>
        public static string WorkflowURL => TestContext.Parameters.Get(RunSettingsKeys.WorkflowURL) ?? EnvironmentURL;
        /// <summary>
        /// Get a value of sercurity server
        /// </summary>
        public static string SecurityServerURL => TestContext.Parameters.Get(RunSettingsKeys.SecurityServerURL);

        /// <summary>
        /// Get a value of predefined environment 
        /// </summary>
        private static string m_predefinedEnvironment;
        /// <summary>
        /// Get a value of predefined environment 
        /// </summary>
        public static string PredefinedEnvironment
        {
            get => m_predefinedEnvironment ?? TestContext.Parameters.Get(RunSettingsKeys.PredefinedEnvironmentName).ToLower();
            set => m_predefinedEnvironment = value;
        }

        /// <summary>
        /// Gets the authentication Endpoint.
        /// </summary>
        /// <value>
        /// The authentication URL.
        /// </value>
        public static string AuthServerEndpoint => TestContext.Parameters.Get(RunSettingsKeys.AuthServerEndpoint);

        #endregion

        #region RestServices
        private static readonly int m_defaultWebHookTimeout = 120;
        private static readonly int m_defaultPageSize = 200;

        /// <summary>
        /// Gets the web hook timeout seconds.
        /// </summary>
        /// <value>
        /// The web hook timeout seconds.
        /// </value>
        public static int WebHookTimeoutSeconds =>
            (
                !string.IsNullOrEmpty(TestContext.Parameters.Get(RunSettingsKeys.WebHookTimeoutSeconds))
                ? Convert.ToInt32(TestContext.Parameters.Get(RunSettingsKeys.WebHookTimeoutSeconds))
                : m_defaultWebHookTimeout
            );

        /// <summary>
        /// Gets the rest service server.
        /// </summary>
        /// <value>
        /// The rest service server.
        /// </value>
        public static string RestServiceUrl => TestContext.Parameters.Get(RunSettingsKeys.RestServiceUrl);

        /// <summary>
        /// Gets the default size of the page.
        /// </summary>
        /// <value>
        /// The default size of the page.
        /// </value>
        public static int DefaultPageSize => !string.IsNullOrEmpty(TestContext.Parameters.Get(RunSettingsKeys.DefaultPageSize))
                ? Convert.ToInt32(TestContext.Parameters.Get(RunSettingsKeys.DefaultPageSize))
                : m_defaultPageSize;
        #endregion

        #region Selenium
        private const string MaxWebDriverSecondsWait = "10";
        private const string MaxTransitionsSecondsWait = "60";
        private const string DefaultGridResolution = "1920x1200x24";

        /// <summary>
        /// Gets the browser.
        /// </summary>
        /// <value>
        /// The browser.
        /// </value>
        public static string Browser => TestContext.Parameters.Get(RunSettingsKeys.Browser).ToLower();
        /// <summary>
        /// Gets the use grid.
        /// </summary>
        /// <value>
        /// The use grid.
        /// </value>
        public static bool UseGrid => bool.Parse(TestContext.Parameters.Get(RunSettingsKeys.UseGrid));
        /// <summary>
        /// Gets the grid running qa.
        /// </summary>
        /// <value>
        /// The grid running qa.
        /// </value>
        public static string GridRunningQA => TestContext.Parameters.Get(RunSettingsKeys.GridRunningQA);

        /// <summary>
        /// Gets the environment configuration.
        /// </summary>
        /// <value>
        /// The environment configuration.
        /// </value>
        public static string EnvironmentConfiguration => TestContext.Parameters.Get(RunSettingsKeys.EnvironmentConfiguration).ToLower();
        /// <summary>
        /// Gets the default web driver timeout.
        /// </summary>
        /// <value>
        /// The default web driver timeout.
        /// </value>
        public static int DefaultWebDriverTimeout => int.Parse(TestContext.Parameters.Get(RunSettingsKeys.DefaultWebDriverTimeout) ?? MaxWebDriverSecondsWait);
        /// <summary>
        /// Gets the default transitions timeout.
        /// </summary>
        /// <value>
        /// The default transitions timeout.
        /// </value>
        public static int DefaultTransitionsTimeout => int.Parse(TestContext.Parameters.Get(RunSettingsKeys.DefaultTransitionsTimeout) ?? MaxTransitionsSecondsWait);


        /// <summary>
        /// Gets the grid resolution.
        /// </summary>
        /// <value>
        /// The grid resolution.
        /// </value>
        public static string GridResolution => TestContext.Parameters.Get(RunSettingsKeys.GridResolution) ?? DefaultGridResolution;

        /// <summary>
        /// Gets the grid hub URL.
        /// </summary>
        /// <value>
        /// The grid hub URL.
        /// </value>
        public static string GridHubUrl => TestContext.Parameters.Get(RunSettingsKeys.GridHubURL);

        /// <summary>
        /// The user
        /// </summary>
        private static string m_user;
        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        public static string User
        {
            get => m_user ?? TestContext.Parameters.Get(RunSettingsKeys.User);
            set => m_user = value;
        }

        /// <summary>
        /// The password
        /// </summary>
        private static string m_password;

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public static string Password
        {
            get => m_password ?? TestContext.Parameters.Get(RunSettingsKeys.Password);
            set => m_password = value;
        }

        /// <summary>
        /// Gets the maximum retry.
        /// </summary>
        /// <value>
        /// The maximum retry.
        /// </value>
        public static int MaxRetry => int.Parse(TestContext.Parameters.Get(RunSettingsKeys.MaxRetryFailedTest));
        #endregion

        #region TFS
        /// <summary>
        /// Gets a value indicating whether [integrate TFS].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [integrate TFS]; otherwise, <c>false</c>.
        /// </value>
        public static bool IntegrateTFS => TestContext.Parameters.Get(RunSettingsKeys.IntegrateTFS)?.ToLower().Equals("true") ?? false;

        /// <summary>
        /// Gets the project identifier.
        /// </summary>
        /// <value>
        /// The project identifier.
        /// </value>
        public static string ProjectId => TestContext.Parameters.Get(RunSettingsKeys.ProjectId);

        /// <summary>
        /// Gets the plan identifier.
        /// </summary>
        /// <value>
        /// The plan identifier.
        /// </value>
        public static int TestPlanId => int.Parse(TestContext.Parameters.Get(RunSettingsKeys.TestPlanId));

        /// <summary>
        /// Gets the suite identifier.
        /// </summary>
        /// <value>
        /// The suite identifier.
        /// </value>
        public static int TestSuiteId => int.Parse(TestContext.Parameters.Get(RunSettingsKeys.TestSuiteId));

        /// <summary>
        /// Gets the area path.
        /// </summary>
        /// <value>
        /// The area path.
        /// </value>
        public static string AreaPath => TestContext.Parameters.Get(RunSettingsKeys.AreaPath);

        /// <summary>
        /// Gets the team identifier.
        /// </summary>
        /// <value>
        /// The team identifier.
        /// </value>
        public static string TeamId => TestContext.Parameters.Get(RunSettingsKeys.TeamId);

        /// <summary>
        /// Gets the identifier dashboard.
        /// </summary>
        /// <value>
        /// The identifier dashboard.
        /// </value>
        public static string DashboardId => TestContext.Parameters.Get(RunSettingsKeys.DashboardId);

        /// <summary>
        /// Gets the name dashboard.
        /// </summary>
        /// <value>
        /// The name dashboard.
        /// </value>
        public static string DashboardName => TestContext.Parameters.Get(RunSettingsKeys.DashboardName);

        /// <summary>
        /// Gets the name of the dashboard.
        /// </summary>
        /// <value>
        /// The name of the dashboard.
        /// </value>
        public static string Token => TestContext.Parameters.Get(RunSettingsKeys.Token);
        /// <summary>
        /// Gets the template team identifier.
        /// </summary>
        /// <value>
        /// The template team identifier.
        /// </value>
        public static string TemplateTeamId => TestContext.Parameters.Get(RunSettingsKeys.TemplateTeamId);
        #endregion
    }
}
