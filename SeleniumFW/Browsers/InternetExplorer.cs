using OpenQA.Selenium;
using OpenQA.Selenium.IE;

namespace Demo.UIAutomation.Framework.Browsers
{
    public class InternetExplorer : AbstractBrowser
    {
        public InternetExplorer()
        {
            Driver = GetDriver();
        }

        public new IWebDriver GetDriver()
        {
            return base.GetDriver() ?? new InternetExplorerDriver((InternetExplorerOptions)GetOptions());
        }

        protected override DriverOptions GetOptions()
        {
            return new InternetExplorerOptions
            {
                EnableNativeEvents = true,
                EnsureCleanSession = true,
                IgnoreZoomLevel = true,
                UnhandledPromptBehavior = UnhandledPromptBehavior.Accept,
                EnablePersistentHover = true,
                PageLoadStrategy = PageLoadStrategy.Eager
            };
        }
    }
}
