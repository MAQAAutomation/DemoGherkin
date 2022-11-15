using System;
using System.Collections.Generic;
using System.IO;
using Demo.UIAutomation.Framework.Application;
using Demo.UIAutomation.Framework.Extensions;
using Demo.UIAutomation.Framework.Helpers;
using Demo.UIAutomation.Framework.Logging;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using static Demo.UIAutomation.Framework.Helpers.ScreenshotGenerator;

namespace Demo.UIAutomation.Framework.Browsers
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Demo.UIAutomation.Framework.Browsers.IBrowser" />
    public abstract class AbstractBrowser : IBrowser
    {
        public IWebDriver Driver { get; set; }
        public ITestLogger Log = TestLog.GetTestLogger();

        /// <summary>
        /// Gets the driver.
        /// </summary>
        /// <returns></returns>
        protected IWebDriver GetDriver()
        {
            if (Driver != null)
            {
                return Driver;
            }

            if (RunSettings.UseGrid == "true")
            {
                var capabilities = SetRemoteDriverCapabilities();
                Log.Info($"Initializing Remote Webdriver using hub: {RunSettings.GridHubUrl}");
                Uri hubUri = new Uri(RunSettings.GridHubUrl);
                var remoteWebDriver = new RemoteWebDriver(hubUri, capabilities.ToCapabilities(), TimeSpan.FromMinutes(6));
                FilesMustBeUploadedToRemoteServerFromLocalFiles(remoteWebDriver);
                return remoteWebDriver;
            }

            return null;
        }

        /// <summary>
        /// Maximizes this instance.
        /// </summary>
        public void Maximize()
        {
            Driver.Manage().Window.Maximize();
        }

        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <returns></returns>
        protected abstract DriverOptions GetOptions();

        /// <summary>
        /// Goes to URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        public void GoToUrl(string url)
        {
            Log.Info($"Navigating to URL {url}");
            Driver.Navigate().GoToUrl(url);
        }

        /// <summary>
        /// Gets the URL.
        /// </summary>
        /// <returns></returns>
        public string GetUrl()
        {
            return Driver.Url;
        }

        /// <summary>
        /// Deletes all cookies.
        /// </summary>
        public void DeleteAllCookies()
        {
            Driver.Manage().Cookies.DeleteAllCookies();
            Driver.Navigate().Refresh();
        }

        /// <summary>
        /// Refreshes this instance.
        /// </summary>
        public void Refresh()
        {
            Log.Debug("Refresh the browser");
            Driver.Navigate().Refresh();
        }

        /// <summary>
        /// Backs this instance.
        /// </summary>
        public void Back()
        {
            Driver.Navigate().Back();
        }

        /// <summary>
        /// Forwards this instance.
        /// </summary>
        public void Forward()
        {
            Driver.Navigate().Forward();
        }

        /// <summary>
        /// Escapes this instance.
        /// </summary>
        public void Escape()
        {
            Actions action = new Actions(Driver);
            action.SendKeys(Keys.Escape).Perform();
        }

        /// <summary>
        /// Navigates to a url on a new tab
        /// </summary>
        /// <param name="url"></param>
        public void NewTab(string url = "")
        {
            Driver.ExecuteJavaScript($"window.open(\"{url}\",\"_blank\")");
        }

        /// <summary>
        /// Driver switch as active window another browser tab
        /// </summary>
        /// <param name="tabNumber">Tab number starting by zero</param>
        public void SwitchToTab(int tabNumber)
        {
            Driver.SwitchTo().Window(Driver.WindowHandles[tabNumber]);
        }

        /// <summary>
        /// This method check if a file exist into a folder for selenoid it does not work the default method so the approach that
        /// we follow is to access to the default download folder of selenoid with the browser and check that the file is there.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="matchType"></param>
        /// <returns>
        /// True if file exist
        /// </returns>
        public bool FileExistOnSelenoidBrowser(string fileName, TextMatchType matchType = TextMatchType.Contains)
        {
            Log.Info($"Check if a file exist into selenoid default download folder {RunSettings.SelenoidDownloadFolder}");
            IList<string> filesDownloadedOnSelenoidDownloadFolder = GetFilenamesFromFolder(RunSettings.SelenoidDownloadFolder);

            if (filesDownloadedOnSelenoidDownloadFolder == null)
            {
                Log.Debug($"Not any file has been found into {RunSettings.SelenoidDownloadFolder}");
                return false;
            }

            foreach (var file in filesDownloadedOnSelenoidDownloadFolder)
            {
                switch (matchType)
                {
                    case TextMatchType.Contains:
                        if (file.Contains(fileName)) return true;
                        break;
                    case TextMatchType.StartsWith:
                        if (file.StartsWith(fileName)) return true;
                        break;
                    case TextMatchType.EndsWith:
                        if (file.EndsWith(fileName)) return true;
                        break;
                    default:
                        if (file == fileName) return true;
                        break;
                }
            }

            return false;
        }

        /// <summary>
        /// Get the file names from a selenoid folder
        /// </summary>
        /// <param name="folderPath">Folder path e.g: /home/selenoid/downloads</param>
        /// <returns></returns>
        public IList<string> GetFilenamesFromFolder(string folderPath)
        {
            int numberOfTabs = Driver.WindowHandles.Count;
            NewTab();
            SwitchToTab(numberOfTabs);
            Driver.Url = "file://" + folderPath;
            IList<string> filesNames = Driver.SqFindElements(SqBy.CssSelector("#tbody a")).ToListString();
            Driver.Close();
            SwitchToTab(numberOfTabs - 1);

            return filesNames;
        }

        /// <summary>
        /// Take an screenshot and save it into the Default.Runsettings defined test results directory sub folder images if nothing it is
        /// specified into the parameters.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="path"></param>
        /// <param name="isStep"></param>
        /// <returns>Complete path where image file has been saved.</returns>
        public string TakeScreenshot(string fileName = null, string path = null)
        {
            string imageDirectory = path ?? $"{RunSettings.TestResultsDir}\\images\\";
            if (!Directory.Exists(imageDirectory))
            {
                Directory.CreateDirectory(imageDirectory);
            }

            fileName = fileName ?? TestContext.CurrentContext.Test.Name + DateTime.Now.ToString("_yyyyMMdd_HHmmss") + ".jpg";
            string filePath = Path.Combine(imageDirectory, fileName);

            if (RunSettings.UseGrid.Equals(true))
            {
                IWebDriver webDriver = this.Driver;
                var quality = EImageQuality.MEDIUM;
                SaveImage(webDriver, ref filePath, quality);
                filePath = TakeFullPageScreenshot(fileName, webDriver as ChromeDriver, quality, imageDirectory) ?? filePath;
            }
            else
            {
                Screenshot image = ((ITakesScreenshot)Driver).GetScreenshot();
                image.SaveAsFile(filePath, ScreenshotImageFormat.Jpeg);
            }

            TestContext.AddTestAttachment(filePath);
            Log.Debug($"Screenshot browser URL: '{GetUrl()}'");

            return Path.GetFullPath(filePath);
        }

        /// <summary>
        /// Accepts the alert.
        /// </summary>
        /// <returns></returns>
        public bool AcceptAlert()
        {
            bool switched = false;
            try
            {
                IAlert alert = Driver.SwitchTo().Alert();
                switched = true;
                alert.Accept();
            }
            catch (Exception e)
            {
                Log.Warn(e.Message);
            }

            return switched;
        }

        /// <summary>
        /// Waits the until alert appears.
        /// </summary>
        /// <param name="maxWaitMs">The maximum wait ms.</param>
        /// <returns></returns>
        public IAlert WaitUntilAlertAppears(int maxWaitMs = 500)
        {
            var wait = new WebDriverWait(Driver, TimeSpan.FromMilliseconds(maxWaitMs));
            return wait.Until((d) => d.SwitchTo().Alert());
        }

        /// <summary>
        /// Sets the remote driver capabilities.
        /// </summary>
        /// <returns></returns>
        private DriverOptions SetRemoteDriverCapabilities()
        {
            DriverOptions options = new ChromeOptions();

            if (RunSettings.Browser.Contains("explorer") || RunSettings.Browser.Equals("ie"))
            {
                options = new InternetExplorerOptions();
            }
            else if (RunSettings.Browser.Contains("firefox"))
            {
                options = new FirefoxOptions();
                options.AddGlobalCapability(CapabilityType.AcceptInsecureCertificates, true);
            }
            else
            {
                options.AddGlobalCapability(CapabilityType.AcceptInsecureCertificates, true);
                options.AddGlobalCapability("safebrowsing.enabled", true);
            }

            if (RunSettings.Browser.ToLower().Contains("headless"))
            {
                options.AddGlobalArgument("headless");
                options.AddGlobalArgument("disable-gpu");
                options.AddGlobalArgument("window-size=1920,1200");
                options.AddGlobalArgument("remote-debugging-port=9222");
            }

            options.AddGlobalCapability(CapabilityType.AcceptSslCertificates, true);
            options.AddGlobalCapability("enableVNC", true);
            options.AddGlobalCapability("screenResolution", $"{RunSettings.GridResolution}");
            options.AddGlobalCapability("name", $"{RunSettings.GridRunningQA} {TestContext.CurrentContext.Test.Name}");
            options.AddGlobalCapability("disable-infobars", true);

            Log.Debug($"Browser capabilities: {options.ToCapabilities()}");

            return options;
        }

        /// <summary>
        /// Set the source to upload files the local project file instead of use the files of the docker container generated by
        /// Selenoid
        /// </summary>
        /// <param name="driver"></param>
        private void FilesMustBeUploadedToRemoteServerFromLocalFiles(IWebDriver driver)
        {
            if (driver is IAllowsFileDetection allowsDetection)
            {
                allowsDetection.FileDetector = new LocalFileDetector();
            }
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
