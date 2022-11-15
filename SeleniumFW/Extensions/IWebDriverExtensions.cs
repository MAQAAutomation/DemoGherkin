using System.Collections.ObjectModel;
using OpenQA.Selenium;

namespace Demo.UIAutomation.Framework.Extensions
{
    public static class IWebDriverExtensions
    {
        /// <summary>
        /// Finds the first <see cref="T:OpenQA.Selenium.IWebElement" /> using the given method.
        /// </summary>
        /// <param name="sqBy">The locating mechanism to use.</param>
        /// <returns>The first matching <see cref="T:OpenQA.Selenium.IWebElement" /> on the current context.</returns>
        public static IWebElement SqFindElement(this IWebDriver driver, SqBy sqBy)
        {
            return driver.FindElement(sqBy.SelectBy);
        }

        /// <summary>
        /// Finds all <see cref="T:OpenQA.Selenium.IWebElement">IWebElements</see> within the current context
        /// using the given mechanism.
        /// </summary>
        /// <param name="sqBy">The locating mechanism to use.</param>
        /// <returns>A <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" /> of all <see cref="T:OpenQA.Selenium.IWebElement">WebElements</see>
        /// matching the current criteria, or an empty list if nothing matches.</returns>
        public static ReadOnlyCollection<IWebElement> SqFindElements(this IWebDriver driver, SqBy sqBy)
        {
            return driver.FindElements(sqBy.SelectBy);
        }
    }
}



