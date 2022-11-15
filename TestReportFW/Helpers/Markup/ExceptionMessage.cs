using AventStack.ExtentReports.MarkupUtils;

namespace Demo.TestReport.Framework.Helpers.Markup
{
    internal class ExceptionMessage : IMarkup
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }
        public ExtentColor TextColor { get; set; }

        /// <summary>
        /// Gets the markup.
        /// </summary>
        /// <returns></returns>
        public string GetMarkup()
        {
            var htmlHeader = $"<h3 style=\"color: {TextColor.ToString().ToLower()}\">Exception message</h3>";
            var htmlErrorMessage = $"<div style=\"diplay:inline-block; margin:0; white-space:pre; font-family: Consolas; font-size:12px; color: {TextColor.ToString().ToLower()}\">{Message}</div>";

            return htmlHeader + htmlErrorMessage;
        }
    }
}

