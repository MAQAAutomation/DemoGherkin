using Demo.UIAutomation.Framework.Browsers;
using Demo.UIAutomation.Framework.Helpers;
using Demo.UIAutomation.Framework.Logging;
using OpenQA.Selenium;

namespace Demo.UIAutomation.Framework.Application
{
    /// <summary>
    /// This class encapsulates the interactions with a group of DOM elements.
    /// The scope of this class is limited to just these elements.
    /// Examples of these area are application pop-up dialogs, calendars, navigation
    /// controls, headers, footers, etc.
    /// On its own this class should not represent a full Page and instead the
    /// AbstractPage should be used as it contains some additional methods.
    /// </summary>
    /// <seealso cref="Demo.UIAutomation.Framework.Application.IWidget" />
    public abstract class AbstractWidget : IWidget
    {
        public virtual By Locator { get; set; } = By.TagName("html");
        public IBrowser Browser { get; set; }
        protected IWebElement RootElement => GetRootElement(ParentLocator, ElementIndex);
        public By ParentLocator { get; set; }
        public int PanelNumber { get; set; }
        public int ElementIndex { get; set; }
        public static ITestLogger Log { get; set; }
        protected int MaxMsWait { get; set; } = 10000;
        public IWebDriver Driver => Browser.Driver;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractWidget"/> class.
        /// </summary>
        /// <param name="browser">The browser.</param>
        protected AbstractWidget(IBrowser browser)
        {
            Browser = browser;
            Log = TestLog.GetTestLogger();
        }

        /// <summary>
        /// Method that filter the dom to get only the web elements that belong to a widget or a page
        /// </summary>
        /// <param name="parentLocator">The locator of the page in case that it is a widget</param>
        /// <param name="elementIndex"></param>
        /// <returns></returns> 
        protected virtual IWebElement GetRootElement(By parentLocator = null, int elementIndex = 0)
        {
            IWebElement parentRoot;
            IWebElement root = null;
            SqWait.WaitForCondition(() =>
            {
                if (parentLocator != null)
                {
                    parentRoot = Browser.Driver.FindElements(parentLocator)[0];
                    root = parentRoot.FindElements(Locator)[elementIndex];
                }
                else
                {
                    root = Browser.Driver.FindElements(Locator)[elementIndex];
                }
                return root != null;
            });
            return root;
        }

        /// <summary>
        /// This makes mandatory to all pages to implement it, the idea is guarantee that all page elements has been loaded and are displayed
        /// as expected        
        /// </summary>
        /// <returns></returns>
        public abstract bool IsDoneLoading();

        /// <summary>
        /// Wait until a page or widget has been loaded as expected.
        /// </summary>
        /// <param name="maxMsWait">The max milliseconds time that can wait (default 45 seconds)</param>
        public virtual void WaitForValidation(int maxMsWait = 45000)
        {
            Log.Debug($"Wait for validation of page {this}");
            WaitForPageToBeReady();
            SqWait.WaitForCondition(IsDoneLoading, maxMsWait);
        }

        /// <summary>
        /// Executes the javascript.
        /// </summary>
        /// <param name="js">The js.</param>
        /// <param name="param">The parameter.</param>
        /// <returns></returns>
        protected object ExecuteJavascript(string js, string param = null)
        {
            return ((IJavaScriptExecutor)Driver).ExecuteScript(js, param);
        }

        /// <summary>
        /// Wait until javascript document.readyState is complete
        /// </summary>
        /// <param name="maxMsWait"></param>
        protected void WaitForPageToBeReady(int maxMsWait = 20000)
        {
            SqWait.WaitForCondition(() => ExecuteJavascript("return document.readyState").Equals("complete"), maxMsWait);
        }
    }
}
