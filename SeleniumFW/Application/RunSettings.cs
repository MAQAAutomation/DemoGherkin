using System;
using System.IO;
using NUnit.Framework;

namespace Demo.UIAutomation.Framework.Application
{
    /// <summary>
    /// Parses the RunSettings file defined run test parameters to static properties.
    /// </summary>
    public class RunSettings : Demo.CommonFramework.Config.RunSettings
    {
        private const string MaxWebDriverSecondsWait = "5";
        private const string MaxTransitionsSecondsWait = "50";

        public new static string Browser => TestContext.Parameters.Get(RunSettingsKeys.Browser).ToLower();
        public new static string UseGrid => TestContext.Parameters.Get(RunSettingsKeys.UseGrid);
        public new static string GridRunningQA => TestContext.Parameters.Get(RunSettingsKeys.GridRunningQA);
        public new static string GridResolution => TestContext.Parameters.Get(RunSettingsKeys.GridResolution);

        private static string _predefinedEnvironment;
        public new static string PredefinedEnvironment
        {
            get => _predefinedEnvironment ?? TestContext.Parameters.Get(RunSettingsKeys.PredefinedEnvironmentName).ToLower();
            set => _predefinedEnvironment = value;
        }

        public new static int DefaultTransitionsTimeout => int.Parse(TestContext.Parameters.Get(RunSettingsKeys.DefaultTransitionsTimeout) ?? MaxTransitionsSecondsWait);
        public const string SelenoidDownloadFolder = "/home/selenium/Downloads/";
        public static string TestResultsDir
        {
            get
            {
                Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
                string testLogsDir = TestContext.Parameters.Get(RunSettingsKeys.TestResultsDir);
                string fullTestLogsDir = Path.GetFullPath(testLogsDir);
                Directory.CreateDirectory(fullTestLogsDir);
                fullTestLogsDir = fullTestLogsDir + Path.DirectorySeparatorChar + "TestResult" + DateTime.Now.ToString("_yyyyMMdd");
                Directory.CreateDirectory(fullTestLogsDir);
                string testName = RemoveInvalidCharsForFolderOrFilename(TestContext.CurrentContext.Test.Name);
                fullTestLogsDir = fullTestLogsDir + Path.DirectorySeparatorChar + "Test_" + testName;
                Directory.CreateDirectory(fullTestLogsDir);

                return fullTestLogsDir;
            }
        }

        public new static string GridHubUrl => TestContext.Parameters.Get(RunSettingsKeys.GridHubURL);

        private static string _user;


        private static string _password;


        /// <summary>
        /// Remove invalid folder or file name characters from a string
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string RemoveInvalidCharsForFolderOrFilename(string name)
        {
            return name.Replace(@"/", "")
                .Replace(@"\", "")
                .Replace("\"", "")
                .Replace(":", "");
        }
    }
}

