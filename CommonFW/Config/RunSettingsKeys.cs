namespace Demo.CommonFramework.Config
{
    public class RunSettingsKeys
    {
        #region Common
        public const string BackendServer = "BackendServer";
        public const string TestLogLevel = "TestLogLevel";
        public const string TestResultFormat = "TestResultFormat";
        public const string CreationInitialData = "CreationInitialData";
        public const string EnvironmentURL = "EnvironmentURL";
        public const string WorkflowURL = "WorkflowURL";
        public const string SecurityServerURL = "SecurityServerURL";
        public const string PredefinedEnvironmentName = "PredefinedEnvironmentName";
        public const string AuthServerEndpoint = "AuthServerEndpoint";
        public const string ApplicationHierarchy = "ApplicationHierarchy";

        #endregion

        #region RestServices
        public const string RestServiceUrl = "RestServiceUrl";
        public const string WebHookTimeoutSeconds = "WebHookTimeoutSeconds";
        public const string DefaultPageSize = "DefaultPageSize";
        #endregion

        #region Selenium
        public const string User = "User";
        public const string Password = "Password";
        public const string Browser = "TestBrowser";
        public const string MaxRetryFailedTest = "MaxRetryFailedTest";
        public const string EnvironmentConfiguration = "EnvironmentConfiguration";
        public const string TestResultsDir = "TestResultsDir";
        public const string UseGrid = "UseGrid";
        public const string GridHubURL = "GridHubURL";
        public const string GridRunningQA = "GridRunningQA";
        public const string DefaultWebDriverTimeout = "DefaultWebDriverTimeout";
        public const string DefaultTransitionsTimeout = "DefaultTransitionsTimeout";
        public const string GridResolution = "GridResolution";
        #endregion

        #region TFS
        public const string IntegrateTFS = "IntegrateTFS";
        public const string ProjectId = "ProjectId";
        public const string TestPlanId = "TestPlanId";
        public const string TestSuiteId = "TestSuiteId";
        public const string AreaPath = "AreaPath";
        public const string DashboardId = "DashboardId";
        public const string DashboardName = "DashboardName";
        public const string TeamId = "TeamId";
        public const string Token = "Token";
        public const string TemplateTeamId = "TemplateTeamId";
        #endregion
    }
}
