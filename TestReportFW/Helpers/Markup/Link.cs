using AventStack.ExtentReports.MarkupUtils;

namespace Demo.TestReport.Framework.Helpers.Markup
{
    internal class Link : IMarkup
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        public string Url { get; set; }

        /// <summary>
        /// Gets the markup.
        /// </summary>
        /// <returns></returns>
        public string GetMarkup()
        {
            var lhs = $"<a href='{Url}'>";
            var rhs = "</a>";
            return lhs + Name + rhs;
        }
    }
}
