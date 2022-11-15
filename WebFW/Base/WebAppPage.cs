using Demo.CommonFramework.ExceptionHandler;
using Demo.UIAutomation.Framework.Application;
using Demo.UIAutomation.Framework.Browsers;
using Demo.UIAutomation.Framework.Extensions.SqWebElements;
using Demo.UIAutomation.Framework.Helpers;
using OpenQA.Selenium;
using RunSettings = Demo.CommonFramework.Config.RunSettings;

namespace Demo.WebApp.Framework
{
    public abstract class WebAppPage : AbstractWidget
    {
        protected WebAppPage(IBrowser browser) : base(browser) { }

        public virtual int DefaultMsTimeout => RunSettings.DefaultWebDriverTimeout * 1000;

        /// <summary>
        /// By the data test identifier.
        /// </summary>
        /// <param name="dataTestId">The data test identifier.</param>
        /// <returns></returns>
        // TODO: Remove the data-test-id option when Broking migrates to the new data-testid attribute
        protected By ByDataTestId(string dataTestId) => By.CssSelector(string.Format("[data-testid='{0}']", dataTestId));

        private SqWebElement ValidationMessage(string message) => () => RootElement.FindElement(By.XPath("//span[contains(.,\"" + message + "\")] | //div[contains(text(),'" + message + "')] | //div[contains(.,'" + message + "')]/br/parent::div"));
        protected SqWebElement DataTableDropdownElement(string ngRepeatName, int row, int column) => () => RootElement.FindElement(By.CssSelector("tbody[ng-repeat=\"" + ngRepeatName + "\"] tr:nth-of-type(" + row + ") td:nth-of-type(" + column + ") div:first-child a:first-child span:first-child"));
        protected SqWebElement DataTableFieldElement(string ngRepeatName, int row, int column) => () => RootElement.FindElement(By.CssSelector("tbody[ng-repeat=\"" + ngRepeatName + "\"] tr:nth-of-type(" + row + ") td:nth-of-type(" + column + ") div:first-child input:first-child"));



        /// <summary>
        /// Waits for element to be hidden.
        /// </summary>
        /// <param name="by">The by.</param>
        public void WaitForElementToBeHidden(By by)
        {
            SqWait.WaitForCondition(() =>
            {
                return Driver.FindElements(by).Count == 0;
            }, DefaultMsTimeout, "Wait for element in DOM");
        }

        /// <summary>
        /// Tries the wait for element to be visible.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <returns></returns>
        public bool TryWaitForElementToBeVisible(SqWebElement element, string elementName = null)
        {
            try
            {
                WaitForElementToBeVisible(element, elementName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Tries the wait for element to be clickable.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <returns></returns>
        public bool TryWaitForElementToBeClickable(SqWebElement element, bool scrollToElement = true, string elementName = null)
        {
            try
            {
                element.WaitForElementToBeClickable(DefaultMsTimeout, scrollToElement, elementName: elementName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Waits for element to be visible.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <exception cref="FrameworkException">Element not visible!</exception>
        public void WaitForElementToBeVisible(SqWebElement element, string elementName = null)
        {
            bool displayed = element.ScrollTo(elementName).Displayed(DefaultMsTimeout);
            if (!displayed) throw new FrameworkException("Element not visible!");
        }

        /// <summary>
        /// Waits for element to be enabled.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <exception cref="FrameworkException">Element not enabled!</exception>
        public void WaitForElementToBeEnabled(SqWebElement element, string elementName = null)
        {
            WaitForElementToBeVisible(element, elementName);
            bool enabled = element.ScrollTo(elementName).Enabled(DefaultMsTimeout);
            if (!enabled) throw new FrameworkException("Element not enabled!");
        }

        /// <summary>
        /// Validation message exists.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public bool ValidationMessageExists(string message)
        {
            TryWaitForElementToBeVisible(ValidationMessage(message));
            return ValidationMessage(message).Displayed(DefaultMsTimeout);
        }

        /// <summary>
        /// Determines whether [is element present] [the specified elements].
        /// </summary>
        /// <param name="elements">The elements.</param>
        /// <returns>
        ///   <c>true</c> if [is element present] [the specified elements]; otherwise, <c>false</c>.
        /// </returns>
        protected bool IsElementPresent(SqListWebElement elements)
        {
            if (elements != null)
            {
                return elements().Count > 0;
            }
            return false;
        }
    }
}
