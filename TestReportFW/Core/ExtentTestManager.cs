using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using AventStack.ExtentReports;
using AventStack.ExtentReports.MarkupUtils;
using Demo.TestReport.Framework.Configuration;
using Demo.TestReport.Framework.Helpers;
using NUnit.Framework;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace Demo.TestReport.Framework.Core
{
    public static class ExtentTestManager
    {
        private static ConcurrentDictionary<string, ExtentWrapper> m_listExtentWrappers = new ConcurrentDictionary<string, ExtentWrapper>();
        public static string TestResultHtmlFilesPath => ExtentWrapper?.TestResultHtmlFilesPath;
        public static string TestResultResourcesFilesPath => ExtentWrapper?.TestResultResourcesFilesPath;
        public static ExtentWrapper ExtentWrapper
        {
            get
            {
                string key = m_listExtentWrappers.ContainsKey(ALL) ? ALL : TestReportName;
                return m_listExtentWrappers.ContainsKey(key) ? m_listExtentWrappers[key] : null;
            }
        }
        private const string ALL = "ALL";
        private static string TestReportName => TestContext.CurrentContext.Test.Name;

        /// <summary>
        /// Create a new Test
        /// </summary>
        /// <param name="testReportName"></param>
        /// <param name="logLevel">The log level.</param>
        /// <param name="description">The description.</param>
        [MethodImpl(Synchronized)]
        public static ExtentTest CreateTest(string testReportName, string logLevel, string description = null)
        {
            string key = ExtentReportConfig.TFS ? testReportName : ALL;

            if (!m_listExtentWrappers.ContainsKey(key))
            {
                ExtentWrapper extentWrapper = new ExtentWrapper();
                m_listExtentWrappers.AddOrUpdate(key, (x) => extentWrapper, (x, old) => extentWrapper);
            }
            return m_listExtentWrappers[key].CreateTest(testReportName, logLevel, description);
        }

        /// <summary>
        /// Create a Log line into the report
        /// </summary>
        /// <param name="status"></param>
        /// <param name="details"></param>
        /// <param name="stepName"></param>
        /// <param name="testReportName"></param>
        [MethodImpl(Synchronized)]
        public static void Log(LogStatus status, string details, string stepName = null, string testReportName = null)
        {
            ExtentWrapper?.Log(status, details, stepName, testReportName);
        }

        /// <summary>
        /// Create a markup log into the report
        /// </summary>
        /// <param name="status"></param>
        /// <param name="markup"></param>
        /// <param name="testReportName"></param>
        [MethodImpl(Synchronized)]
        public static void Log(LogStatus status, IMarkup markup, string testReportName = null)
        {
            ExtentWrapper?.Log(status, markup, testReportName);
        }

        /// <summary>
        /// Adds the information log.
        /// </summary>
        /// <param name="stepName">Name of the step.</param>
        /// <param name="details">The details.</param>
        [MethodImpl(Synchronized)]
        public static void LogInfo(string details, string stepName = null)
        {
            Log(LogStatus.INFO, details, stepName);
        }

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="stepName">Name of the step.</param>
        /// <param name="details">The details.</param>
        [MethodImpl(Synchronized)]
        public static void LogError(string details, string stepName = null)
        {
            Log(LogStatus.ERROR, details, $"{ServiceHelper.GetPageServiceName()} - {stepName}");
        }

        /// <summary>
        /// Logs the warning.
        /// </summary>
        /// <param name="stepName">Name of the step.</param>
        /// <param name="details">The details.</param>
        [MethodImpl(Synchronized)]
        public static void LogWarning(string details, string stepName = null)
        {
            Log(LogStatus.WARNING, details, stepName);
        }

        /// <summary>
        /// Logs the pass.
        /// </summary>
        /// <param name="stepName">Name of the step.</param>
        /// <param name="details">The details.</param>
        [MethodImpl(Synchronized)]
        public static void LogPass(string details, string stepName = null)
        {
            Log(LogStatus.PASS, details, stepName);
        }

        /// <summary>
        /// Logs the fail.
        /// </summary>
        /// <param name="stepName">Name of the step.</param>
        /// <param name="details">The details.</param>
        [MethodImpl(Synchronized)]
        public static void LogFail(string details, string stepName = null)
        {
            Log(LogStatus.FAIL, details, stepName);
        }

        /// <summary>
        /// Logs the fatal.
        /// </summary>
        /// <param name="details">The details.</param>
        /// <param name="stepName">Name of the step.</param>
        [MethodImpl(Synchronized)]
        public static void LogFatal(string details, string stepName = null)
        {
            Log(LogStatus.FATAL, details, $"{ServiceHelper.GetPageServiceName()} - {stepName}");
        }

        /// <summary>
        /// Logs the debug.
        /// </summary>
        /// <param name="details">The details.</param>
        /// <param name="stepName">Name of the step.</param>
        [MethodImpl(Synchronized)]
        public static void LogDebug(string details, string stepName = null)
        {
            Log(LogStatus.DEBUG, details, $"{ServiceHelper.GetPageServiceName()} - {stepName}");
        }

        /// <summary>
        /// Logs the with attach.
        /// </summary>
        /// <param name="details">The details.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="fileContent">Content of the file.</param>
        [MethodImpl(Synchronized)]
        public static void LogInfoWithAttach(string details, string filePath, string fileName, string fileContent)
        {
            FileHelper.LogStream(fileContent, fileName, filePath, out string fullPath);
            ExtentWrapper?.InternalLogWithAttach(LogStatus.INFO, details, fullPath.Replace(TestResultHtmlFilesPath, ".\\"));
        }

        /// <summary>
        /// Logs the with attach.
        /// </summary>
        /// <param name="details">The details.</param>
        /// <param name="filePath">The file path.</param>
        [MethodImpl(Synchronized)]
        public static void LogErrorWithAttach(string details, string filePath, string fileName, string fileContent)
        {
            FileHelper.LogStream(fileContent, fileName, filePath, out string fullPath);
            ExtentWrapper?.InternalLogWithAttach(LogStatus.ERROR, details, fullPath.Replace(TestResultHtmlFilesPath, ".\\"));
        }

        /// <summary>
        /// Logs the with attach.
        /// </summary>
        /// <param name="details">The details.</param>
        /// <param name="filePath">The file path.</param>
        [MethodImpl(Synchronized)]
        public static void LogWarningWithAttach(string details, string filePath, string fileName, string fileContent)
        {
            FileHelper.LogStream(fileContent, fileName, filePath, out string fullPath);
            ExtentWrapper?.InternalLogWithAttach(LogStatus.WARNING, details, fullPath.Replace(TestResultHtmlFilesPath, ".\\"));
        }

        /// <summary>
        /// Logs the with attach.
        /// </summary>
        /// <param name="details">The details.</param>
        /// <param name="filePath">The file path.</param>
        [MethodImpl(Synchronized)]
        public static void LogPassWithAttach(string details, string filePath, string fileName, string fileContent)
        {
            FileHelper.LogStream(fileContent, fileName, filePath, out string fullPath);
            ExtentWrapper?.InternalLogWithAttach(LogStatus.PASS, details, fullPath.Replace(TestResultHtmlFilesPath, ".\\"));
        }

        /// <summary>
        /// Logs the with attach.
        /// </summary>
        /// <param name="details">The details.</param>
        /// <param name="filePath">The file path.</param>
        [MethodImpl(Synchronized)]
        public static void LogFailWithAttach(string details, string filePath, string fileName, string fileContent)
        {
            FileHelper.LogStream(fileContent, fileName, filePath, out string fullPath);
            ExtentWrapper?.InternalLogWithAttach(LogStatus.FAIL, details, fullPath.Replace(TestResultHtmlFilesPath, ".\\"));
        }

        /// <summary>
        /// Logs the with attach.
        /// </summary>
        /// <param name="details">The details.</param>
        /// <param name="filePath">The file path.</param>
        [MethodImpl(Synchronized)]
        public static void LogFatalWithAttach(string details, string filePath, string fileName, string fileContent)
        {
            FileHelper.LogStream(fileContent, fileName, filePath, out string fullPath);
            ExtentWrapper?.InternalLogWithAttach(LogStatus.FATAL, details, fullPath.Replace(TestResultHtmlFilesPath, ".\\"));
        }

        /// <summary>
        /// Logs the with screenshot.
        /// </summary>
        /// <param name="details">The details.</param>
        /// <param name="filePath">The file path.</param>
        [MethodImpl(Synchronized)]
        public static void LogInfoWithScreenshot(string details, string filePath)
        {
            GeneralLogWithScreenShot(LogStatus.INFO, details, filePath);
        }

        /// <summary>
        /// Logs the with screenshot.
        /// </summary>
        /// <param name="details">The details.</param>
        /// <param name="filePath">The file path.</param>
        [MethodImpl(Synchronized)]
        public static void LogErrorWithScreenshot(string details, string filePath)
        {
            GeneralLogWithScreenShot(LogStatus.ERROR, details, filePath);
        }

        /// <summary>
        /// Logs the with screenshot.
        /// </summary>
        /// <param name="details">The details.</param>
        /// <param name="filePath">The file path.</param>
        [MethodImpl(Synchronized)]
        public static void LogWarningWithScreenshot(string details, string filePath)
        {
            GeneralLogWithScreenShot(LogStatus.WARNING, details, filePath);
        }

        /// <summary>
        /// Logs the with screenshot.
        /// </summary>
        /// <param name="details">The details.</param>
        /// <param name="filePath">The file path.</param>
        [MethodImpl(Synchronized)]
        public static void LogPassWithScreenshot(string details, string filePath)
        {
            GeneralLogWithScreenShot(LogStatus.PASS, details, filePath);
        }

        /// <summary>
        /// Logs the with screenshot.
        /// </summary>
        /// <param name="details">The details.</param>
        /// <param name="filePath">The file path.</param>
        [MethodImpl(Synchronized)]
        public static void LogFailWithScreenshot(string details, string filePath)
        {
            GeneralLogWithScreenShot(LogStatus.FAIL, details, filePath);
        }

        /// <summary>
        /// Logs the with screenshot.
        /// </summary>
        /// <param name="details">The details.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="filePath">The file path.</param>
        [MethodImpl(Synchronized)]
        public static void LogFatalWithScreenshot(string details, string filePath)
        {
            GeneralLogWithScreenShot(LogStatus.FATAL, details, filePath);
        }

        /// <summary>
        /// Perform a log with screenshot.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="details">The details.</param>
        /// <param name="filePath">The full file path including filename</param>
        [MethodImpl(Synchronized)]
        public static void LogWithScreenshot(LogStatus logLevel, string details, string filePath)
        {
            if (ExtentReportConfig.LogLevel >= logLevel)
            {
                ExtentWrapper?.InternalLogWithScreenshot(logLevel, details, filePath);
            }
        }

        /// <summary>
        /// Generals the log with screen shot.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="details">The details.</param>
        /// <param name="filePath">The file path.</param>
        private static void GeneralLogWithScreenShot(LogStatus logLevel, string details, string filePath)
        {
            ExtentWrapper?.InternalLogWithScreenshot(logLevel, details,
               (ExtentReportConfig.LogLevel >= logLevel) ? filePath : string.Empty);
        }

        /// <summary>
        /// Ends the test.
        /// </summary>
        [MethodImpl(Synchronized)]
        public static void EndTest()
        {
            ExtentWrapper?.Flush();
        }

        /// <summary>
        /// Add category or categories to the current test
        /// </summary>
        /// <param name="category"></param>
        /// <returns>The extent test or null if it does not exist</returns>
        [MethodImpl(Synchronized)]
        public static void AddCategory(params string[] category)
        {
            ExtentWrapper?.GetExtentTest(TestContext.CurrentContext.Test.Name)?.AssignCategory(category);
        }

        /// <summary>
        /// Add category or categories to the test specified into testReportName
        /// </summary>
        /// <param name="testReportName"></param>
        /// <param name="category"></param>
        /// <returns>The extent test or null if it does not exist</returns>
        [MethodImpl(Synchronized)]
        public static void AddCategoryToTestName(string testReportName, params string[] category)
        {
            ExtentWrapper?.GetExtentTest(testReportName)?.AssignCategory(category);
        }

        /// <summary>
        /// Check if a report test has been added before
        /// </summary>
        /// <param name="testReportName"></param>
        /// <returns>Boolean, true if the test has been added before otherwise false</returns>
        [MethodImpl(Synchronized)]
        public static bool ExistTest(string testReportName)
        {
            return ExtentWrapper?.GetExtentTest(testReportName) != null;
        }

        /// <summary>
        /// Zips the test.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(Synchronized)]
        public static string ZipTest()
        {
            string fullPathZipFile = FileHelper.ReplaceLastOccurrence(TestResultHtmlFilesPath, "\\", ".zip");
            FileHelper.ZipTestResultFolder(TestResultHtmlFilesPath, fullPathZipFile);
            return fullPathZipFile;
        }
    }
}