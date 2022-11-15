using System;
using Demo.UIAutomation.Framework.Application;
using Demo.UIAutomation.Framework.Logging;

namespace Demo.UIAutomation.Framework.Browsers
{
    /// <summary>
    /// this class get different browsers instances (IWebDriver) depending on the value set into App.config, it is an easy approach to add more browsers easily. 
    /// </summary>
    public static class BrowserFactory
    {
        private static ITestLogger _logger = TestLog.GetTestLogger();

        /// <summary>
        /// Gets the browser.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">Invalid browser of {RunSettings.Browser} specified</exception>
        public static IBrowser GetBrowser()
        {
            string browser = RunSettings.Browser.ToLower();
            _logger.Info("Initializing " + RunSettings.Browser + " browser");
            switch (browser)
            {
                case "chrome":
                    {
                        var driver = new Chrome();
                        driver.Maximize();
                        return driver;
                    }
                case "chromeheadless":
                case "chrome-headless":
                case "chrome headless":
                    {
                        var driver = new ChromeHeadless();
                        driver.Maximize();
                        return driver;
                    }
                case "firefox":
                    {
                        var driver = new Firefox();
                        driver.Maximize();
                        return driver;
                    }
                case "ie":
                case "internetexplorer":
                case "internet explorer":
                    {
                        var driver = new InternetExplorer();
                        driver.Maximize();
                        return driver;
                    }
                case "edge":
                    {
                        var driver = new Edge();
                        driver.Maximize();
                        return driver;
                    }

                default: throw new Exception($"Invalid browser of {RunSettings.Browser} specified");
            }
        }
    }
}
