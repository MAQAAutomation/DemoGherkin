using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace Demo.UIAutomation.Framework.Browsers
{
    public class Firefox : AbstractBrowser
    {
        public Firefox()
        {
            Driver = GetDriver();
        }

        public new IWebDriver GetDriver()
        {
            return base.GetDriver() ?? new FirefoxDriver((FirefoxOptions)GetOptions());
        }

        protected override DriverOptions GetOptions()
        {
            DriverOptions opts = new FirefoxOptions
            {
                AcceptInsecureCertificates = true
            };
            return opts;
        }
    }
}
