using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;

namespace Demo.UIAutomation.Framework.Extensions
{
    public static class DriverOptionsExtensions
    {
        /// <summary>
        /// Adds the global capability.
        /// This method will be able to be removed when added GlobalCapability parameter to true by default in the 
        /// AddAdditionalCapability generic method (see https://github.com/SeleniumHQ/selenium/issues/6563)
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="name">The name.</param>
        /// <param name="capabilityValue">The capability value.</param>
        public static void AddGlobalCapability(this DriverOptions options, string name, object capabilityValue)
        {
            switch (options)
            {
                case ChromeOptions chromeOptions:
                    chromeOptions.AddAdditionalCapability(name, capabilityValue, true);
                    break;
                case FirefoxOptions firefoxOptions:
                    firefoxOptions.AddAdditionalCapability(name, capabilityValue, true);
                    break;
                case InternetExplorerOptions internetExplorerOptions:
                    internetExplorerOptions.AddAdditionalCapability(name, capabilityValue, true);
                    break;
                default:
                    options.AddAdditionalCapability(name, capabilityValue);
                    break;
            }
        }

        /// <summary>
        /// Adds the global argument 
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="argumentValue"></param>
        public static void AddGlobalArgument(this DriverOptions options, string argumentValue)
        {
            switch (options)
            {
                case ChromeOptions chromeOptions:
                    chromeOptions.AddArgument(argumentValue);
                    break;
                case FirefoxOptions firefoxOptions:
                    firefoxOptions.AddArgument(argumentValue);
                    break;
                default:
                    throw new ArgumentException("Only chrome and firefox options are supported.");
            }
        }
    }
}
