using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

namespace Demo.UIAutomation.Framework.Browsers
{
    public class Edge : AbstractBrowser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Edge"/> class.
        /// </summary>
        public Edge()
        {
            Driver = GetDriver();
        }

        /// <summary>
        /// Gets the driver.
        /// </summary>
        /// <returns></returns>
        public new IWebDriver GetDriver()
        {
            return base.GetDriver() ?? new EdgeDriver((EdgeOptions)GetOptions());
        }

        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <returns></returns>
        protected override DriverOptions GetOptions()
        {
            return new EdgeOptions
            {
                AcceptInsecureCertificates = true,
                UseInPrivateBrowsing = true,
                UnhandledPromptBehavior = UnhandledPromptBehavior.Accept,
                PageLoadStrategy = PageLoadStrategy.Eager
            };
        }
    }
}
