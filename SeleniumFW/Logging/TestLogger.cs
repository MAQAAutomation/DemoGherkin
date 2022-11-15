using log4net;
using Demo.TestReport.Framework.Core;
using System;

namespace Demo.UIAutomation.Framework.Logging
{
    public class TestLogger : ITestLogger
    {
        ILog logger { get; set; }

        private static bool DeactivateLogs { get; set; }

        public TestLogger(ILog log, bool deactivateLogs)
        {
            logger = log;
            DeactivateLogs = deactivateLogs;
        }

        //
        // Summary:
        //     Log a message object with Debug level.
        //
        // Parameters:
        //   message:
        //     The message object to log.
        public void Debug(object message)
        {
            if (DeactivateLogs) return;
            ExtentTestManager.LogDebug((string)message);
            logger.Debug(message);
        }

        //
        // Summary:
        //     Log a message object with Debug level including the stack
        //     trace of the System.Exception passed as a parameter.
        //
        // Parameters:
        //   message:
        //     The message object to log.
        //
        //   exception:
        //     The exception to log, including its stack trace.
        public void Debug(object message, Exception exception)
        {
            if (DeactivateLogs) return;
            logger.Debug(message, exception);
        }

        //
        // Summary:
        //     Logs a message object with the Error level.
        //
        // Parameters:
        //   message:
        //     The message object to log.
        public void Error(object message)
        {
            if (DeactivateLogs) return;
            ExtentTestManager.LogError((string)message);
            logger.Error(message);
        }

        //
        // Summary:
        //     Log a message object with the log4net.Core.Level.Error level including the stack
        //     trace of the System.Exception passed as a parameter.
        //
        // Parameters:
        //   message:
        //     The message object to log.
        //
        //   exception:
        //     The exception to log, including its stack trace.
        public void Error(object message, Exception exception)
        {
            if (DeactivateLogs) return;
            logger.Error(message, exception);
        }

        //
        // Summary:
        //     Logs a message object with the INFO level including the stack trace of the System.Exception
        //     passed as a parameter.
        //
        // Parameters:
        //   message:
        //     The message object to log.
        //
        //   exception:
        //     The exception to log, including its stack trace.
        public void Info(object message, Exception exception)
        {
            if (DeactivateLogs) return;
            logger.Info(message, exception);
        }

        //
        // Summary:
        //     Logs a message object with the Info level.
        //
        // Parameters:
        //   message:
        //     The message object to log.
        public void Info(object message)
        {
            if (DeactivateLogs) return;
            ExtentTestManager.LogInfo((string)message);
            logger.Info(message);
        }

        //
        // Summary:
        //     Log a message object with the Warn level.
        //
        // Parameters:
        //   message:
        //     The message object to log.
        public void Warn(object message)
        {
            if (DeactivateLogs) return;
            ExtentTestManager.LogWarning((string)message);
            logger.Warn(message);           
        }

        //
        // Summary:
        //     Log a message object with the log4net.Core.Level.Warn level including the stack
        //     trace of the System.Exception passed as a parameter.
        //
        // Parameters:
        //   message:
        //     The message object to log.
        //
        //   exception:
        //     The exception to log, including its stack trace.
        public void Warn(object message, Exception exception)
        {
            if (DeactivateLogs) return;
            logger.Warn(message, exception);
        }

        /// <summary>
        /// Log steps as are defined 
        /// </summary>
        /// <param name="log"></param>
        /// <param name="stepNumber"></param>
        /// <param name="when">The action</param>
        /// <param name="then">The expected result (optional)</param>
        public void Step(int stepNumber, string when, string then = null)
        {
            if (DeactivateLogs) return;

            if (then != null) then = "----> " + then;
            string logMessage = $"STEP {stepNumber}: {when} {then}";
            logger.Info(logMessage);
            ExtentTestManager.LogInfo($"{when} {then}");
        }

        public void Step(int stepNumber, string description)
        {
            if (DeactivateLogs) return;

            ExtentTestManager.LogInfo($"STEP {stepNumber}: {description}");
            logger.InfoFormat("STEP {0}: {1}", stepNumber, description);
        }
    }
}
