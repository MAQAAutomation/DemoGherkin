using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Demo.UIAutomation.Framework.Application;

namespace Demo.UIAutomation.Framework.Browsers
{
    public class Chrome : AbstractBrowser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Chrome"/> class.
        /// </summary>
        public Chrome()
        {
            Driver = GetDriver();
        }

        /// <summary>
        /// Gets the driver.
        /// </summary>
        /// <returns></returns>
        public new IWebDriver GetDriver()
        {
            return base.GetDriver() ?? new ChromeDriver((ChromeOptions)GetOptions());
        }

        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <returns></returns>
        protected override DriverOptions GetOptions()
        {
            var options = new ChromeOptions();
            options.AddArguments(
                "--start-maximized",
                "--disable-extensions",
                "--disable-infobars",
                "--disable-extensions-file-access-check",
                "--disable-default-apps",
                "--ignore-certificate-errors"
                );


            options.AddUserProfilePreference("safebrowsing.enabled", true);
            options.AddUserProfilePreference("download.default_directory", RunSettings.TestResultsDir);

            return options;
        }
    }
}
