using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Demo.UIAutomation.Framework.Application;

namespace Demo.UIAutomation.Framework.Browsers
{
    public class ChromeHeadless : AbstractBrowser
    {
        public ChromeHeadless()
        {
            Driver = GetDriver();
        }

        public new IWebDriver GetDriver()
        {
            return base.GetDriver() ?? new ChromeDriver((ChromeOptions)GetOptions());
        }

        protected override DriverOptions GetOptions()
        {
            var options = new ChromeOptions();
            options.AddArguments(
                "headless",
                "disable-gpu",
                "window-size=1920,1200",
                "remote-debugging-port=9222");

            options.AddUserProfilePreference("download.default_directory", RunSettings.TestResultsDir);

            return options;
        }
    }
}
