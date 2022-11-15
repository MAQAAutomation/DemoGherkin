using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using Demo.UIAutomation.Framework.Helpers;

namespace Demo.UIAutomation.Framework.Browsers
{
    public interface IBrowser : IDisposable 
    {
        IWebDriver Driver { get; }

        /// <summary>
        /// Accepts the alert.
        /// </summary>
        /// <returns></returns>
        bool AcceptAlert();

        void Maximize();

        void GoToUrl(string url);

        string GetUrl();

        void DeleteAllCookies();

        void Refresh();

        void Back();

        void Forward();

        void Escape();

        /// <summary>
        /// Switch to tab 
        /// </summary>
        /// <param name="tabNumber">Number of tab starting by 0</param>
        void SwitchToTab(int tabNumber);

        IAlert WaitUntilAlertAppears(int maxWaitMs = 500);

        /// <summary>
        /// This method check if a file exist into a folder for selenoid it does not work the default method so the approach that
        /// we follow is to access to the default download folder of selenoid with the browser and check that the file is there.
        /// </summary>
        /// <param name="path">Optional, Complete path with filename by default will u</param>
        /// <returns>True if file exist</returns>
        bool FileExistOnSelenoidBrowser(string fileName, TextMatchType matchType = TextMatchType.Contains);

        /// <summary>
        /// Get the file names from a selenoid folder
        /// </summary>
        /// <param name="folderPath">Folder path e.g: /home/selenoid/downloads</param>
        /// <returns></returns>
        IList<string> GetFilenamesFromFolder(string folderPath);

        /// <summary>
        /// Take an screenshot and save it into the App.config defined test results directory sub folder images if nothing it is
        /// specified into the parameters. 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="path"></param>
        /// <returns>Path and filename where the screenshot is saved</returns>
        string TakeScreenshot(string filename = null, string path = null);
    }
}
