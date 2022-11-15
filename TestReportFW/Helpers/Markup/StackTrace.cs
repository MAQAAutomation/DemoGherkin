using AventStack.ExtentReports.MarkupUtils;

namespace Demo.TestReport.Framework.Helpers.Markup
{
    internal class StackTrace : IMarkup
    {
        /// <summary>
        /// Gets or sets the trace.
        /// </summary>
        /// <value>
        /// The trace.
        /// </value>
        public string Trace { get; set; }
        public ExtentColor TextColor { get; set; }

        /// <summary>
        /// Gets the markup.
        /// </summary>
        /// <returns></returns>
        public string GetMarkup()
        {
            var htmlHeader = $"<h3 style=\"color: {TextColor.ToString().ToLower()}\">Stack trace</h3>";
            var htmlStackTrace = $"<div style=\"diplay:inline-block; margin:0; white-space:pre; font-family: Consolas; font-size:10px; color: {TextColor.ToString().ToLower()}\">{Trace}</div>";

            return htmlHeader + htmlStackTrace;
        }
    }
}

