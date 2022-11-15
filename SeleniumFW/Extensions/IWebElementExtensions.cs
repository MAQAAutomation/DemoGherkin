using System.Collections.Generic;
using Demo.UIAutomation.Framework.Logging;
using OpenQA.Selenium;

namespace Demo.UIAutomation.Framework.Extensions
{
    public static class IWebElementExtensions
    {
        public static ITestLogger Log = TestLog.GetTestLogger();

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static string GetValue(this IWebElement element)
        {
            return element.GetAttribute("value");
        }

        /// <summary>
        /// To the list string.
        /// </summary>
        /// <param name="elements">The elements.</param>
        /// <returns></returns>
        public static List<string> ToListString(this IList<IWebElement> elements)
        {
            if (elements == null || elements.Count == 0) return null;
            List<string> result = new List<string>();

            foreach (IWebElement element in elements)
            {
                result.Add(element.Text);
            }
            return result;
        }
    }
}
