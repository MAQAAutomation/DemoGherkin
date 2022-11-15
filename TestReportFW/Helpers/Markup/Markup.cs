using AventStack.ExtentReports.MarkupUtils;

namespace Demo.TestReport.Framework.Helpers.Markup
{
    public static class Markup
    {


        /// <summary>
        /// Create a html stack trace to be shown into extents reports
        /// </summary>
        /// <param name="stackTrace">The stack trace.</param>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        public static IMarkup CreateStackTrace(string stackTrace, ExtentColor color = ExtentColor.Black)
        {
            return new StackTrace
            {
                Trace = stackTrace,
                TextColor = color
            };
        }

        /// <summary>
        /// Create a html exception message to be shown into extents reports
        /// </summary>
        /// <param name="exceptionMessage">The exception message.</param>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        public static IMarkup CreateExceptionMessage(string exceptionMessage, ExtentColor color = ExtentColor.Black)
        {
            return new ExceptionMessage
            {
                Message = exceptionMessage,
                TextColor = color
            };
        }
    }
}
