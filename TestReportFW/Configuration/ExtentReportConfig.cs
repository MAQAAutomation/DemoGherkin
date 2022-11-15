using Demo.TestReport.Framework.Core;

namespace Demo.TestReport.Framework.Configuration
{
    /// <summary>
    /// Contains the config of the report, if there is not any value defined into a property it will take the a default one or the one defined
    /// into html-config.xml file
    /// </summary>
    public static class ExtentReportConfig
    {
        public static string DocumentTitle { get; set; }
        public static string ReportName { get; set; }
        public static string ReportConfigFilePath { get; set; }
        public static LogStatus LogLevel { get; set; }
        public static bool TFS { get; set; } = false;
    }
}
