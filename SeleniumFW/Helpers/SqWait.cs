using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using OpenQA.Selenium;
using Demo.CommonFramework.Helpers;
using Demo.UIAutomation.Framework.Logging;

namespace Demo.UIAutomation.Framework.Helpers
{
    public class SqWait : BaseUtils
    {
        public static ITestLogger Log = TestLog.GetTestLogger();

        /// <summary>
        /// Waits retriying a bool condition until it gets true.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <param name="msDuration">Max duration in ms before timeout exception.</param>
        /// <param name="description"></param>
        /// <exception cref="WebDriverTimeoutException"></exception>
        public static void WaitForCondition(Func<bool> condition, int msDuration = 4000, string description = null)
        {
            if (description != null)
            {
                Log.Debug($"waiting for condition: '{description}'");
            }

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            Exception ex;
            var retries = 0;

            do
            {
                try
                {
                    ex = null;
                    retries++;
                    if (condition())
                    {
                        return;
                    }
                }
                catch (Exception e)
                {
                    if (e is StaleElementReferenceException)
                    {
                        Log.Warn("Race conditions occur when trying to accomplish the condition. Retry in process", e);
                    }

                    ex = e;
                    Thread.Sleep(300); // polling time
                }
            } while (stopWatch.ElapsedMilliseconds <= msDuration);
            stopWatch.Stop();

            var errorMessage = $"Error executing the call from '{condition.GetMethodInfo()}' after {retries} attempts. ";

            if (ex != null)
            {
                throw new TimeoutException(errorMessage, ex);
            }

            errorMessage += "The condition was false.";
            throw new TimeoutException(errorMessage);
        }

        /// <summary>
        /// Try to wait for a boolean condition to be true.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <param name="maxWaitMs"></param>
        /// <param name="description"></param>
        /// <exception cref="WebDriverTimeoutException"></exception>
        public static bool TryWaitForCondition(Func<bool> condition, int maxWaitMs = 5000, string description = null)
        {
            try
            {
                WaitForCondition(condition, maxWaitMs, description);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

