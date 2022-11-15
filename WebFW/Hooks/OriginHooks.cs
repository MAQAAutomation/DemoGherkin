using System;
using System.IO;
using BoDi;
using Demo.CommonFramework.Config;
using Demo.CommonFramework.ExceptionHandler;
using Demo.TestReport.Framework.Core;
using Demo.UIAutomation.Framework.Browsers;
using Demo.UIAutomation.Framework.Helpers;
using Demo.WebFW.Framework.Steps;
using OpenQA.Selenium.Remote;
using TechTalk.SpecFlow;

namespace Demo.WebFW.Framework.Hooks
{
    [Binding]
    public sealed class OriginHooks : CommonSteps
    {
        private readonly IObjectContainer m_objectContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="OriginHooks"/> class.
        /// </summary>
        /// <param name="objectContainer">The object container.</param>
        public OriginHooks(IObjectContainer objectContainer)
        {
            m_objectContainer = objectContainer;
        }

        /// <summary>
        /// Befores the scenario.
        /// </summary>
        [BeforeScenario(Order = 4)]
        public void BeforeScenario()
        {
            Browser = Browser ?? BrowserFactory.GetBrowser();
            Browser.Maximize();
            Browser.Driver.Manage().Cookies.DeleteAllCookies();
            Browser.GoToUrl(RunSettings.EnvironmentURL);

            string browserVersion = (string)((RemoteWebDriver)Browser.Driver).Capabilities.GetCapability("browserVersion");
            ExtentTestManager.LogDebug(string.Empty, "Browser Version: " + browserVersion);

            m_objectContainer.RegisterInstanceAs(Browser, typeof(IBrowser));
            page = new StepPages(Browser);
            m_objectContainer.RegisterInstanceAs(page, typeof(StepPages));
        }

        /// <summary>
        /// Quits this instance.
        /// </summary>
        [AfterScenario]
        public void Quit()
        {
            Browser.Driver.Dispose();
        }

        /// <summary>
        /// Cleans up.
        /// </summary>
        [AfterTestRun]
        public static void CleanUp()
        {
            //Do nothing
        }

        /// <summary>
        /// Afters the step.
        /// </summary>
        [AfterStep(Order = 0)]
        public void AfterStep()
        {

            try
            {
                string stepName = ScenarioContext.StepContext.StepInfo.StepDefinitionType.ToString() + " " + ScenarioContext.StepContext.StepInfo.Text;
                string filePath = ExtentTestManager.TestResultResourcesFilesPath;
                string fileName = Path.GetFileNameWithoutExtension(stepName.Replace(" ", "_")) + ".png";

                string screenshotPath = ScreenshotGenerator.TakeScreenshot(fileName, Browser.Driver, ScreenshotGenerator.EImageQuality.LOW, filePath);

                if (ScenarioContext.TestError != null)
                {
                    if (!ScenarioContext.TestError.GetType().Equals(typeof(InconclusiveFrameworkException)))
                    {

                        ExtentTestManager.LogErrorWithScreenshot(ScenarioContext.StepContext.StepInfo.Text, screenshotPath);
                    }
                }
                else
                {
                    ExtentTestManager.LogInfoWithScreenshot(ScenarioContext.StepContext.StepInfo.Text, screenshotPath);
                }
            }
            catch (Exception e)
            {
                ExtentTestManager.LogWarning("The test skipped an Error happened writting the logs in the test execution please take a look: " + e.Message);
            }
        }
    }
}
