using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AventStack.ExtentReports;
using AventStack.ExtentReports.MarkupUtils;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Configuration;
using NUnit.Framework;
using static Demo.TestReport.Framework.Configuration.ConfigurationLoader;
using static Demo.TestReport.Framework.Configuration.ExtentReportConfig;

namespace Demo.TestReport.Framework.Core
{
    public class ExtentWrapper
    {
        internal ExtentReports Report { get; set; }

        internal ExtentHtmlReporter HtmlReporter { get; set; }

        /// <summary>
        /// Gets the test result resources files path.
        /// </summary>
        /// <value>
        /// The test result resources files path.
        /// </value>
        public string TestResultResourcesFilesPath
        {
            get
            {
                var fullTestLogsDir = TestResultHtmlFilesPath + "Test_" + TestContext.CurrentContext.Test.Name + Path.DirectorySeparatorChar;
                Directory.CreateDirectory(fullTestLogsDir);

                return fullTestLogsDir;
            }
        }

        /// <summary>
        /// This list contains all the tests that are included into the report.
        /// </summary>
        private static readonly List<ExtentTest> TestList = new List<ExtentTest>();

        /// <summary>
        /// Gets the test result HTML files path.
        /// </summary>
        /// <value>
        /// The test result HTML files path.
        /// </value>
        public string TestResultHtmlFilesPath { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtentWrapper"/> class.
        /// </summary>
        public ExtentWrapper()
        {
            Report = new ExtentReports();
            CreateTestResultHtmlFilesPath();
            HtmlReporter = CreateHTMLReport();
        }

        /// <summary>
        /// This add a new test to the report
        /// </summary>
        /// <param name="testReportName">Name of the report test.</param>
        /// <param name="logLevel">The log level.</param>
        /// <param name="description">The description.</param>
        internal ExtentTest CreateTest(string testReportName, string logLevel, string description = null)
        {
            if (Report == null)
            {
                new ExtentWrapper();
            }

            ExtentTest test = Report.CreateTest(testReportName, description);
            TestList.Add(test);

            if (!Enum.TryParse(logLevel, true, out LogStatus log))
            {
                log = LogStatus.ALL;
                Log(LogStatus.WARNING,
                    "Logging Level property is not defined. The default value ALL will be used. Correct Values: " + string.Join(",", Enum.GetNames(typeof(LogStatus))),
                    string.Format("{0}. ", logLevel));
            }
            LogLevel = log;

            return test;
        }

        /// <summary>
        /// Get report extent test which name contains <see cref="testReportName"/>
        /// </summary>
        /// <param name="testReportName">Name of the test.</param>
        /// <returns>The extent test or null if it does not exist</returns>
        internal ExtentTest GetExtentTest(string testReportName)
        {
            return TestList.FirstOrDefault(t => t.Model.Name.Equals(testReportName)) ??
                   TestList.FirstOrDefault(t => t.Model.Name.Contains(testReportName));
        }

        /// <summary>
        /// Creates the test result HTML files path.
        /// </summary>
        private void CreateTestResultHtmlFilesPath()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            string testLogsDir = TestContext.CurrentContext.TestDirectory + Path.DirectorySeparatorChar + "TestResults";
            string fullTestLogsDir = Path.GetFullPath(testLogsDir);
            Directory.CreateDirectory(fullTestLogsDir);

            TestResultHtmlFilesPath = fullTestLogsDir + Path.DirectorySeparatorChar + "TestResult" + DateTime.Now.ToString("_yyyyMMdd_HHmmssfff");

            if (Directory.Exists(TestResultHtmlFilesPath))
            {
                TestResultHtmlFilesPath += new Random().Next(1000);
            }

            TestResultHtmlFilesPath += Path.DirectorySeparatorChar;
            Directory.CreateDirectory(TestResultHtmlFilesPath);
        }

        /// <summary>
        /// Creates the HTML report.
        /// </summary>
        /// <returns>Html reporter</returns>
        internal ExtentHtmlReporter CreateHTMLReport()
        {
            var htmlReporter = new ExtentHtmlReporter(TestResultHtmlFilesPath);
            htmlReporter.Config.Theme = Theme.Dark;

            if (File.Exists(ReportConfigFilePath))
            {
                LoadConfig(htmlReporter, ReportConfigFilePath);
            }

            if (DocumentTitle != null) htmlReporter.Config.DocumentTitle = DocumentTitle;
            if (ReportName != null) htmlReporter.Config.DocumentTitle = DocumentTitle;

            Report.AttachReporter(htmlReporter);

            return htmlReporter;
        }

        /// <summary>
        /// Logs the specified status.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="details">The details.</param>
        /// <param name="stepName">Name of the step.</param>
        /// <param name="testReportName">The name of the test shown into the report</param>
        internal void Log(LogStatus status, string details, string stepName = null, string testReportName = null)
        {
            string text = string.IsNullOrEmpty(stepName) ? status + ": " + details : status + ": " + stepName + ". " + details;
            ColorByStatus(status, out ExtentColor backGroundColor, out ExtentColor lettersColor);
            Log(status, MarkupHelper.CreateLabel(text, backGroundColor, lettersColor), testReportName);
        }

        /// <summary>
        /// Logs the specified status.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="markup">The markup.</param>
        /// <param name="testReportName">Name of the test report.</param>
        internal void Log(LogStatus status, IMarkup markup, string testReportName = null)
        {
            if (LogLevel >= status)
            {
                testReportName = testReportName ?? TestContext.CurrentContext.Test.Name;
                GetExtentTest(testReportName)?.Log((Status)status, markup);
                Flush();
            }
        }

        /// <summary>
        /// Logs the snapshot.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="description">The description.</param>
        /// <param name="attachPath">The attachPath.</param>
        internal void InternalLogWithScreenshot(LogStatus status, string description, string attachPath)
        {
            if (File.Exists(attachPath) && LogLevel >= status)
            {
                GetExtentTest(TestContext.CurrentContext.Test.Name)?.Log((Status)status, description, MediaEntityBuilder.CreateScreenCaptureFromPath(attachPath.Replace(TestResultHtmlFilesPath, @"./")).Build());
                Flush();
            }
        }

        /// <summary>
        /// Flushes this instance.
        /// </summary>
        internal void Flush()
        {
            Report.Flush();
        }

        /// <summary>
        /// Logs with attach
        /// </summary>
        /// <param name="status"></param>
        /// <param name="details"></param>
        /// <param name="filePath"></param>
        internal void InternalLogWithAttach(LogStatus status, string details, string filePath)
        {
            if (Path.IsPathRooted(filePath))
            {
                LogWithAttachAbsolute(status, filePath, details);
            }
            else
            {
                LogWithAttachRelative(status, filePath, details);
            }
        }

        /// <summary>
        /// Logs the with attach relative.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="relativePath">The relative path.</param>
        /// <param name="details"></param>
        private void LogWithAttachRelative(LogStatus status, string relativePath, string details = "")
        {
            Log(status, "<a href='" + relativePath + "'>" + Path.GetFileName(relativePath) + "</a>", details);
        }

        /// <summary>
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="snapshot">The snapshot.</param>
        private void LogWithAttachAbsolute(LogStatus status, string absolutePath, string details = "")
        {
            Log(status, "<a href='file:///" + absolutePath + "'>" + Path.GetFileName(absolutePath) + "</a>", details);
        }

        /// <summary>
        /// Colors the by status.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="backGroundColor">Color of the back ground.</param>
        /// <param name="lettersColor">Color of the letters.</param>
        private void ColorByStatus(LogStatus status, out ExtentColor backGroundColor, out ExtentColor lettersColor)
        {
            backGroundColor = ExtentColor.Transparent;
            lettersColor = ExtentColor.Black;

            switch (status)
            {
                case LogStatus.PASS:
                    backGroundColor = ExtentColor.Green;
                    break;
                case LogStatus.FAIL:
                    backGroundColor = ExtentColor.Red;
                    break;
                case LogStatus.SKIP:
                    backGroundColor = ExtentColor.Grey;
                    break;
                case LogStatus.FATAL:
                case LogStatus.ERROR:
                    lettersColor = ExtentColor.Red;
                    break;
                case LogStatus.WARNING:
                    lettersColor = ExtentColor.Orange;
                    break;
                case LogStatus.INFO:
                case LogStatus.DEBUG:
                    lettersColor = (HtmlReporter.Config.Theme == Theme.Dark) ? ExtentColor.Grey : ExtentColor.Black;
                    break;

            }
        }
    }
}
