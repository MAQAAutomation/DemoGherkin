using System.Collections.Generic;
using Demo.WebFW.Framework.Steps;

namespace WebFW.Helpers.PageHelper
{
    public static class LogPageHelper
    {
        private static StepPages Page { get; set; }
        internal static bool ReviewLogoutPage(StepPages page, Dictionary<string, string> parameters, out string error)
        {
            Page = page;
            error = string.Empty;
            string currentValue = string.Empty;

            if (parameters.TryGetValue("alertmessage", out string value))
            {
                currentValue = page.SecureAreaPageInstance.GetBannerMessage();
                error += currentValue.Contains(value) ? "" : string.Format(" Banner messages expected {0}, value found {1} ", value, currentValue);
            }
            if (parameters.TryGetValue("textmessage", out value))
            {
                currentValue = page.SecureAreaPageInstance.GetTextMessage();
                error += currentValue.Contains(value) ? "" : string.Format(" text messages expected {0}, value found {1} ", value, currentValue);
            }

            return string.IsNullOrEmpty(error);
        }

        internal static bool ReviewLoginPage(StepPages page, Dictionary<string, string> parameters, out string error)
        {
            Page = page;
            error = string.Empty;
            string currentValue = string.Empty;

            if (parameters.TryGetValue("alertmessage", out string value))
            {
                currentValue = page.LoginPageInstance.GetBannerMessage();
                error += currentValue.Contains(value) ? "" : string.Format(" Banner messages expected {0}, value found {1} ", value, currentValue);
            }

            return string.IsNullOrEmpty(error);
        }
    }
}
