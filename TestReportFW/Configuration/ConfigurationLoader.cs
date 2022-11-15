using System.IO;
using System.Linq;
using System.Xml.Linq;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Configuration;

namespace Demo.TestReport.Framework.Configuration
{
    public static class ConfigurationLoader
    {
        /// <summary>
        /// Load the configuration of a extent report xml configuration file into the ExtentHtmlReporter.        
        /// </summary>
        /// <param name="htmlReporter"></param>
        /// <param name="filePath"></param>
        public static void LoadConfig(ExtentHtmlReporter htmlReporter, string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The file " + filePath + " was not found.");
            }

            htmlReporter.LoadConfig(filePath);
            XDocument xdoc = XDocument.Load(filePath, LoadOptions.None);
            InitializeHtmlReporterConfigProperties(htmlReporter, xdoc);
        }

        /// <summary>
        /// Initializes the HTML reporter configuration properties.
        /// </summary>
        /// <param name="htmlReporter">The HTML reporter.</param>
        /// <param name="xdoc">The xdoc.</param>
        private static void InitializeHtmlReporterConfigProperties(ExtentHtmlReporter htmlReporter, XDocument xdoc)
        {
            foreach (XElement element in xdoc.Descendants("configuration").First().Elements())
            {
                if (element.Name.LocalName.Equals("theme"))
                {
                    htmlReporter.Config.UserConfigurationMap.Remove("theme");
                    htmlReporter.Config.Theme = element.Value.Equals("standard") ? Theme.Standard : Theme.Dark;
                }

                if (element.Name.LocalName.Equals("enableTimeline")) htmlReporter.Config.EnableTimeline = bool.Parse(element.Value);
                if (element.Name.LocalName.Equals("documentTitle")) htmlReporter.Config.DocumentTitle = element.Value;
                if (element.Name.LocalName.Equals("reportName")) htmlReporter.Config.ReportName = element.Value;
                if (element.Name.LocalName.Equals("encoding")) htmlReporter.Config.Encoding = element.Value;
            }
        }
    }
}


