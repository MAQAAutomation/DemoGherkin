using SeleniumBy = OpenQA.Selenium.By;

namespace Demo.UIAutomation.Framework.Extensions
{
    /// <summary>
    /// Wraps OpenQA.Selenium.By providing extensibility so we can add our custom mechanisms to find elements.
    /// </summary>
    public class SqBy
    {
        public SeleniumBy SelectBy { get; set; }

        /// <summary> Gets a mechanism to find elements by their cascading style sheet (CSS) selector.
        /// </summary>
        /// <param name="cssSelectorToFind">The CSS selector to find.</param>
        /// <returns>A <see cref="T:Demo.UIAutomation.Framework.Extensions.By" /> object the driver can use to find the elements.</returns>
        public static SqBy CssSelector(string cssSelectorToFind)
        {
            return new SqBy()
            {
                SelectBy = SeleniumBy.CssSelector(cssSelectorToFind)
            };
        }


    }
}