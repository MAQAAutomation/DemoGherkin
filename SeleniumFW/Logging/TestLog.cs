using System.IO;
using System.Linq;
using Demo.UIAutomation.Framework.Application;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Filter;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using NUnit.Framework;

namespace Demo.UIAutomation.Framework.Logging
{
    /// <summary>
    /// This class manage based on log4net providing support to customize the test logger. 
    /// </summary>
    public class TestLog
    {
        /// <summary>
        /// Set to true to deactivate the log4net FW logs 
        /// </summary>
        public static bool DeactivateTestLogs = false;

        public static string ConsoleLoggerName = "Debug.ConsoleLogger";
        private static LevelMap _levelMap = new LevelMap();
        private static ITestLogger _testLogger;

        public static ITestLogger GetTestLogger()
        {
            if (DeactivateTestLogs)
            {
                return new TestLogger(null, DeactivateTestLogs);
            }

            IAppender fileAppender = GetFileAppender(TestContext.CurrentContext.Test.Name + "_log", GetFilters(StringToLevel(RunSettings.TestLogLevel), Level.Fatal));
            IAppender consoleAppender = GetConsoleAppender(ConsoleLoggerName, GetFilters(StringToLevel(RunSettings.TestLogLevel), Level.Fatal));

            ILog log = GetLogger(TestContext.CurrentContext.Test.Name, consoleAppender, fileAppender);
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
            hierarchy.Root.Level = Level.Trace;
            hierarchy.Configured = true;
            _testLogger = new TestLogger(log, DeactivateTestLogs);

            return _testLogger;
        }
        private static ILog GetLogger(string name, params IAppender[] appenders)
        {
            var log = LogManager.GetLogger(name);
            Logger logger = (Logger)log.Logger;
            logger.Level = Level.All;

            foreach (IAppender appender in appenders)
            {
                logger.AddAppender(appender);
            }

            return log;
        }

        /// <summary>
        /// Get the file appender the approach is that each test methods will have its own appender with its log file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="filters"></param>
        /// <returns>The file appender</returns>
        public static IAppender GetFileAppender(string fileName, params IFilter[] filters)
        {
            var appender = GetNamedAppender(fileName);
            if (appender == null)
            {
                string path = GetLogTestFileFullPath(fileName);
                var fileLayout = new PatternLayout();
                fileLayout.ConversionPattern = "%date{dd MMM yyyy HH:mm:ss} [%thread] %-5level - %message%newline";
                fileLayout.ActivateOptions();

                var fa = new FileAppender
                {
                    Name = "FileAppender",
                    AppendToFile = false,
                    File = path,
                    Layout = fileLayout
                };

                foreach (IFilter filter in filters)
                {
                    fa.AddFilter(filter);
                }
                fa.ActivateOptions();
                appender = fa;
            }
            return appender;
        }

        /// <summary>
        /// Get a console appender to show the full test execution log into the console
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="filters"></param>
        /// <returns>The console appender</returns>
        public static IAppender GetConsoleAppender(string name, params IFilter[] filters)
        {
            var appender = GetNamedAppender(name);
            if (appender == null)
            {
                var consoleLayout = new PatternLayout();
                consoleLayout.ConversionPattern = "%date{dd MMM yyyy HH:mm:ss} [%thread] %-5level - %message%newline";
                consoleLayout.ActivateOptions();

                var ca = new ConsoleAppender
                {
                    Name = name,
                    Layout = consoleLayout,
                    Target = "Console.Out"
                };

                foreach (IFilter filter in filters)
                {
                    ca.AddFilter(filter);
                }
                ca.ActivateOptions();
                appender = ca;
            }
            return appender;
        }

        public static IFilter[] GetFilters(Level minLevel, Level maxLevel)
        {
            var filter = new LevelRangeFilter { LevelMin = minLevel, LevelMax = maxLevel, AcceptOnMatch = true };
            var denyAllFilter = new DenyAllFilter();

            return new IFilter[] { filter, denyAllFilter };
        }

        /// <summary>
        /// Parses from a string to a log4net Level
        /// </summary>
        /// <param name="logLevel"></param>
        /// <returns>The log4net level that matches with the name provided.</returns>
        public static Level StringToLevel(string logLevel)
        {
            InitializeLevelMap();
            return _levelMap[logLevel];
        }

        public static void Dispose(string nameAppender)
        {
            GetNamedAppender(nameAppender).Close();
        }

        #region Private methods

        private static LevelMap InitializeLevelMap()
        {
            if (_levelMap.AllLevels.Count > 0) return _levelMap;

            _levelMap = new LevelMap();
            // Add the predefined levels to the map
            _levelMap.Add(Level.Off);

            // Unrecoverable errors
            _levelMap.Add(Level.Emergency);
            _levelMap.Add(Level.Fatal);
            _levelMap.Add(Level.Alert);

            // Recoverable errors
            _levelMap.Add(Level.Critical);
            _levelMap.Add(Level.Severe);
            _levelMap.Add(Level.Error);
            _levelMap.Add(Level.Warn);

            // Information
            _levelMap.Add(Level.Notice);
            _levelMap.Add(Level.Info);

            // Debug
            _levelMap.Add(Level.Debug);
            _levelMap.Add(Level.Fine);
            _levelMap.Add(Level.Trace);
            _levelMap.Add(Level.Finer);
            _levelMap.Add(Level.Verbose);
            _levelMap.Add(Level.Finest);

            _levelMap.Add(Level.All);

            return _levelMap;
        }

        private static IAppender GetNamedAppender(string name)
        {
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
            return hierarchy.GetAppenders().FirstOrDefault(a => a.Name == name);
        }

        public static string GetLogTestFileFullPath(string name)
        {
            name = RunSettings.RemoveInvalidCharsForFolderOrFilename(name);
            return Path.GetFullPath(Path.Combine(RunSettings.TestResultsDir, string.Format("{0}.log", name)));
        }

        #endregion
    }
}
