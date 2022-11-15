using System;
using System.IO;
using Demo.TestReport.Framework.Core;
using Demo.UIAutomation.Framework.Browsers;
using Demo.UIAutomation.Framework.Helpers;
using Demo.WebApp.Framework.Pages.HeroKuApp;
using WebFW.Pages.HeroKuApp;

namespace Demo.WebFW.Framework.Steps
{
    public class StepPages
    {


        /// <summary>
        /// Gets or sets the browser.
        /// </summary>
        /// <value>
        /// The browser.
        /// </value>
        private IBrowser Browser { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StepPages"/> class.
        /// </summary>
        /// <param name="browser">The browser.</param>
        public StepPages(IBrowser browser)
        {
            Browser = browser;
        }

        /// <summary>Adds more log screen shots to the report.</summary>
        /// <param name="screenShotName">Name of the screen shot.</param>
        public void addMoreLogScreenShots(string screenShotName = "")
        {
            string stepName = String.Empty;
            stepName = RandomGenerator.GetAlphaString(5);

            string filePath = ExtentTestManager.TestResultResourcesFilesPath;
            string fileName = Path.GetFileNameWithoutExtension(stepName.Replace(" ", "_")) + ".png";
            string screenshotPath = ScreenshotGenerator.TakeScreenshot(fileName, Browser.Driver, ScreenshotGenerator.EImageQuality.LOW, filePath);
            ExtentTestManager.LogInfoWithScreenshot(screenShotName, screenshotPath);
        }

        public LoginPage LoginPageInstance => new LoginPage(Browser);
        public SecureAreaPage SecureAreaPageInstance => new SecureAreaPage(Browser);
        public MainPage MainPageInstance => new MainPage(Browser);


    }

}
