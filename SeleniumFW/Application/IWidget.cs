using OpenQA.Selenium;
using Demo.UIAutomation.Framework.Browsers;

namespace Demo.UIAutomation.Framework.Application
{
    public interface IWidget
    {
        /// <summary>
        /// Provides a mechanism to find the root element from a page, this is particular helpful to filter html pages.
        /// </summary>
        By Locator { get; set; }

        /// <summary>
        /// As sequel application contains a multipanel UI that represents the panel that belong to the widget (normally css #SlidePanel1 #SlidePanel2 etc...
        /// this is useful to filter and find the widget into the correct panel.
        /// </summary>
        int PanelNumber { get; set; }

        /// <summary>
        /// There are scenarios where we can have a list of widgets into the same panel and cannot be selected by a unique css selector with this we can indicate which
        /// one we would like to get into the list of widgets.
        /// </summary>
        int ElementIndex { get; set; }

        /// <summary>
        /// Interface to access to the Browser instance that contains the widget
        /// </summary>
        IBrowser Browser { get; set; }

        /// <summary>
        /// Is mandatory to implement this method into we should add all the checks to verify that the web element contains all the expected elements with the expected value.
        /// </summary>
        /// <returns></returns>
        bool IsDoneLoading();

        /// <summary>
        /// This method will wait until the page has been loaded as expected (method IsDoneLoading) otherwise it will throw and exception.
        /// </summary>
        /// <param name="maxMsWait"></param>
        void WaitForValidation(int maxMsWait = 30000);
    }
}