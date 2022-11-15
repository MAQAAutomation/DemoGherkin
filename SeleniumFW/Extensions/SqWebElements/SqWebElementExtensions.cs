namespace Demo.UIAutomation.Framework.Extensions.SqWebElements
{
    public static class SqWebElementExtensions
    {
        /// <summary>
        /// Clicks the element.
        /// </summary>
        /// <param name="element">The get the element.</param>
        /// <param name="maxWaitMs">The maximum wait ms.</param>
        /// <param name="scrollToElement">if set to <c>true</c> [scroll to element].</param>
        /// <param name="elementName">Name of the element.</param>
        /// <returns></returns>
        public static SqWebElement Click(this SqWebElement element, int maxWaitMs = 5000, bool scrollToElement = false, string elementName = null)
        {
            return element.Click<SqWebElement>(maxWaitMs, scrollToElement, true, elementName);
        }

        /// <summary>
        /// Waits for element to be displayed and enabled.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="maxMsWait">The maximum ms wait.</param>
        /// <param name="scrollToElement">if set to <c>true</c> [scroll to element].</param>
        /// <param name="elementName">Name of the element.</param>
        /// <returns>
        /// The element if it is clickable otherwise and exception
        /// </returns>
        public static SqWebElement WaitForElementToBeClickable(this SqWebElement element, int maxMsWait = 10000, bool scrollToElement = false, string elementName = null)
        {
            return element.WaitForElementToBeClickable<SqWebElement>(maxMsWait, scrollToElement, elementName: elementName);
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="maxWaitMs"></param>
        /// <returns></returns>
        public static string Text(this SqWebElement element, int maxWaitMs = 0)
        {
            return element.Text<SqWebElement>(maxWaitMs);
        }

        /// <summary>
        /// Inputs the value.
        /// </summary>
        /// <param name="element">The get the element.</param>
        /// <param name="text">The text.</param>
        /// <param name="tabOut">if set to <c>true</c> [tab out].</param>
        /// <param name="scrollToElement">if set to <c>true</c> [scroll to element].</param>
        /// <param name="elementName">Name of the element.</param>
        /// <returns></returns>
        public static SqWebElement Input(this SqWebElement element, string text, bool tabOut = true, bool scrollToElement = false, string elementName = null, int defaultTransitionsTimeout = 4000)
        {
            return element.Input<SqWebElement>(text, tabOut, scrollToElement, elementName: elementName, defaultTransitionsTimeout);
        }

        /// <summary>
        /// Selects the option by value.
        /// </summary>
        /// <param name="element">The get the element.</param>
        /// <param name="value">The value.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <returns></returns>
        public static SqWebElement SelectOptionByValue(this SqWebElement element, string value, string elementName = null)
        {
            return element.SelectOptionByValue<SqWebElement>(value, elementName);
        }


        /// <summary>
        /// Get a random dropdown option. To use this method the dropdown must contain a <select> tag.
        /// </summary>
        /// <param name="drpSelectElement">IWebElement it must contain a <select> tag </param>
        /// <param name="ignoreFirstOption">Optional it ignores the first option. </param>
        /// <returns></returns>
        public static SqWebElement GetRandomDropdownOption(this SqWebElement drpSelectElement, bool ignoreFirstOption = true)
        {
            return drpSelectElement.GetRandomDropdownOption<SqWebElement>(ignoreFirstOption);
        }

        #region Common actions 


        /// <summary>
        /// Enableds the specified maximum wait ms.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="maxWaitMs">The maximum wait ms.</param>
        /// <returns></returns>
        public static bool Enabled(this SqWebElement element, int maxWaitMs = 4000) => element.Enabled<SqWebElement>(maxWaitMs);

        /// <summary>
        /// Scrolls to.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <returns></returns>
        public static SqWebElement ScrollTo(this SqWebElement element, string elementName = null) => element.ScrollTo<SqWebElement>(elementName: elementName);

        /// <summary>
        /// Determines whether the specified element is displayed.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="maxWaitMs">The maximum wait ms.</param>
        /// <returns></returns>
        public static bool Displayed(this SqWebElement element, int maxWaitMs = 4000) => element.Displayed<SqWebElement>(maxWaitMs);

        #endregion
    }
}
