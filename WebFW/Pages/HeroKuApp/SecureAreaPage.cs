using Demo.CommonFramework.ExceptionHandler;
using Demo.UIAutomation.Framework.Browsers;
using Demo.UIAutomation.Framework.Extensions.SqWebElements;
using Demo.WebFW.Framework;
using OpenQA.Selenium;
using WebFW.PageReferences;
using RunSettings = Demo.CommonFramework.Config.RunSettings;

namespace Demo.WebApp.Framework.Pages.HeroKuApp
{
    public class SecureAreaPage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationPage"/> class.
        /// </summary>
        /// <param name="browser">The browser.</param>
        public SecureAreaPage(IBrowser browser) : base(browser) { }

        private SqWebElement TitlePage => () => RootElement.FindElement(By.XPath("//title[contains(.,'The Internet')]"));
        private SqWebElement NamePage => () => RootElement.FindElement(By.XPath("//h2[contains(.,'Secure Area')]"));
        private SqWebElement TextMessagePage => () => RootElement.FindElement(By.XPath("//h4"));
        private SqWebElement FlashMessagePage => () => RootElement.FindElement(By.Id("flash"));
        private SqWebElement ButtonLoginOutPage => () => RootElement.FindElement(By.XPath("//a[contains(.,'Logout')]"));


        /// <summary>
        /// Determines whether [is done loading].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is done loading]; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsDoneLoading()
        {
            return TitlePage.ScrollTo().Enabled(RunSettings.DefaultWebDriverTimeout) &&
                NamePage.ScrollTo().Displayed(RunSettings.DefaultWebDriverTimeout);
        }


        /// <summary>Gets the text message.</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        public string GetTextMessage()
        {
            return TextMessagePage.Text();
        }

        /// <summary>Gets the banner message.</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        public string GetBannerMessage()
        {
            return FlashMessagePage.Text();
        }

        /// <summary>Clicks the button.</summary>
        public void PressButton(string button)
        {
            ;
            switch (button.ToLower())
            {
                case PageNameRefs.LOGINOUT_HEROKUAPP_BUTTON:
                    ButtonLoginOutPage.Click(RunSettings.DefaultWebDriverTimeout, scrollToElement: true, "Logout");
                    break;
                default:
                    throw new FrameworkException(button + " button is not supported");
            }
        }

    }
}
