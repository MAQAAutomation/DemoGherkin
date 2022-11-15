using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.Extensions;
using Demo.TestReport.Framework.Helpers;

namespace Demo.UIAutomation.Framework.Helpers
{
    public static class ScreenshotGenerator
    {
        public enum EImageQuality
        {
            LOW = 25,
            MEDIUM = 50,
            HIGH = 100
        };

        /// <summary>
        /// Takes the screenshot.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="pathFolder">The path folder.</param>
        /// <param name="webDriver">The web driver.</param>
        /// <param name="quality">The quality.</param>
        /// <param name="tfsOutPut">The tfsOutPut.</param>
        /// <returns></returns>
        public static string TakeScreenshot(string fileName, IWebDriver webDriver, EImageQuality quality, string pathFileName)
        {
            string newFileName = string.Empty;
            string fullPath = pathFileName + FileHelper.TrimIllegalCharacters(fileName, pathFileName);
            int screenshotCount = 0;

            while (File.Exists(fullPath))
            {
                newFileName = FileHelper.AddNumberFileName(fileName, screenshotCount++);
                fullPath = pathFileName + newFileName;
            }

            SaveImage(webDriver, ref fullPath, quality);

            fullPath = TakeFullPageScreenshot((string.IsNullOrEmpty(newFileName) ? fileName : newFileName), webDriver as ChromeDriver, quality, pathFileName) ?? fullPath;

            TestContext.AddTestAttachment(fullPath);

            return fullPath;
        }

        /// <summary>
        /// Saves the image.
        /// </summary>
        /// <param name="webDriver">The web driver.</param>
        /// <param name="fullPath">The full path.</param>
        /// <param name="quality">The quality.</param>
        public static void SaveImage(IWebDriver webDriver, ref string fullPath, EImageQuality quality)
        {
            byte[] imageArray;
            try
            {
                imageArray = webDriver.TakeScreenshot().AsByteArray;
            }
            catch (FormatException)
            {
                fullPath = null;
                return;
            }

            using (var myImage = Image.FromStream(new MemoryStream(imageArray)))
            {
                ImageHelper.SaveImage(fullPath, myImage, (int)quality);
            }
        }

        /// <summary>
        /// Takes the full page screenshot.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="webDriver">The web driver.</param>
        /// <param name="quality">The quality.</param>
        /// <param name="pathFileName">Name of the path file.</param>
        /// <returns></returns>
        public static string TakeFullPageScreenshot(string fileName, ChromeDriver webDriver, EImageQuality quality, string pathFileName)
        {
            if (webDriver == null) return null;

            int scrollHeight = Convert.ToInt32(webDriver.ExecuteJavaScript<object>("return Math.max(document.body.scrollHeight, document.documentElement.scrollHeight)"));
            int width = Convert.ToInt32(webDriver.ExecuteJavaScript<object>("return Math.max(window.innerWidth,document.body.scrollWidth,document.documentElement.scrollWidth)"));
            int height = Convert.ToInt32(webDriver.ExecuteJavaScript<object>("return Math.max(window.innerHeight, document.documentElement.clientHeight)"));

            bool hasScrollBar = Convert.ToInt32(height) < Convert.ToInt32(scrollHeight);

            if (!hasScrollBar)
            {
                scrollHeight += GetCustomScrollBarOffset(webDriver, scrollHeight);
                scrollHeight = GetSidePanelScrollHeight(webDriver, scrollHeight);

                hasScrollBar = Convert.ToInt32(height) < Convert.ToInt32(scrollHeight);
            }

            if (hasScrollBar)
            {
                string fullPath = pathFileName + FileHelper.TrimIllegalCharacters(fileName, pathFileName);

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }

                Dictionary<string, object> metrics = new Dictionary<string, object>
                {
                    ["width"] = width,
                    ["height"] = scrollHeight,
                    ["deviceScaleFactor"] = webDriver.ExecuteJavaScript<object>("return window.devicePixelRatio || 1"),
                    ["mobile"] = webDriver.ExecuteJavaScript<object>("return typeof window.orientation !== 'undefined'")
                };

                webDriver.ExecuteChromeCommandWithResult("Emulation.setDeviceMetricsOverride", metrics);

                SaveImage(webDriver, ref fullPath, quality);

                webDriver.ExecuteChromeCommandWithResult("Emulation.clearDeviceMetricsOverride", new Dictionary<string, object>());

                return fullPath;
            }

            return null;
        }

        /// <summary>
        /// Gets the height of the side panel scroll.
        /// </summary>
        /// <param name="webDriver">The web driver.</param>
        /// <param name="scrollHeight">Height of the scroll.</param>
        /// <returns></returns>
        private static int GetSidePanelScrollHeight(ChromeDriver webDriver, int scrollHeight)
        {
            try
            {
                var sidePanels = webDriver.FindElementsByXPath("//div[contains(@class,'side-panel__content')]");
                var sidePanel = sidePanels.AsEnumerable().Where(x => x.Displayed).ToList();
                if (sidePanel.Count > 0)
                {
                    int sidePanelScrollHeight = 0;
                    sidePanel.ForEach(x => sidePanelScrollHeight = Math.Max(sidePanelScrollHeight, Convert.ToInt32(webDriver.ExecuteJavaScript<object>("return arguments[0].scrollHeight", x)))
                        + Convert.ToInt32(x.GetCssValue("margin-top").Replace("px", ""))
                        + Convert.ToInt32(x.GetCssValue("margin-bottom").Replace("px", ""))
                        + Convert.ToInt32(x.GetCssValue("padding-top").Replace("px", ""))
                        + Convert.ToInt32(x.GetCssValue("padding-bottom").Replace("px", "")));
                    scrollHeight = Math.Max(scrollHeight, sidePanelScrollHeight);
                }
            }
            catch
            {
                //Do nothing
            }

            return scrollHeight;
        }

        /// <summary>
        /// Gets the custom scroll bar offset.
        /// </summary>
        /// <param name="webDriver">The web driver.</param>
        /// <param name="scrollHeight">Height of the scroll.</param>
        /// <returns></returns>
        private static int GetCustomScrollBarOffset(ChromeDriver webDriver, int scrollHeight)
        {
            int offset = 0;
            try
            {
                webDriver.ExecuteJavaScript("window.scrollTo(0, " + scrollHeight + ")");
                ReadOnlyCollection<IWebElement> scrollVerticalBar = webDriver.FindElementsByClassName("ps__rail-y");
                List<IWebElement> webElements = scrollVerticalBar.AsEnumerable().Where(x => Convert.ToInt32(x.GetCssValue("top").Replace("px", "")) != 0).ToList();
                webElements.ForEach(x => offset = Math.Max(offset, Convert.ToInt32(x.GetCssValue("top").Replace("px", ""))));
                if (webElements.Count > 0)
                {
                    return offset;
                }
            }
            catch
            {
                // do nothing
            }

            return 0;
        }
    }
}
