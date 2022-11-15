using Demo.CommonFramework.ExceptionHandler;
using Demo.UIAutomation.Framework.Browsers;
using Demo.UIAutomation.Framework.Extensions.SqWebElements;
using OpenQA.Selenium;
using WebFW.PageReferences;
using RunSettings = Demo.CommonFramework.Config.RunSettings;

namespace Demo.WebApp.Framework.Pages.HeroKuApp
{
    public class LoginPage : WebFW.Framework.Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationPage"/> class.
        /// </summary>
        /// <param name="browser">The browser.</param>
        public LoginPage(IBrowser browser) : base(browser) { }

        private SqWebElement TitlePage => () => RootElement.FindElement(By.XPath("//title[contains(.,'The Internet')]"));
        private SqWebElement NamePage => () => RootElement.FindElement(By.XPath("//h2[contains(.,'Login Page')]"));
        private SqWebElement InputUserPage => () => RootElement.FindElement(By.Id("username"));
        private SqWebElement InputPasswdPage => () => RootElement.FindElement(By.Id("password"));
        private SqWebElement ButtonLoginPage => () => RootElement.FindElement(By.XPath("//button[contains(.,'Login')]"));
        private SqWebElement FlashMessagePage => () => RootElement.FindElement(By.Id("flash"));


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

        /// <summary>Sets the user.</summary>
        /// <param name="user">The user.</param>
        public void SetUser(string user)
        {
            InputUserPage.Input(user, tabOut: false, scrollToElement: true, "user");
        }

        /// <summary>Sets the passwd.</summary>
        /// <param name="passwd">The passwd.</param>
        public void SetPasswd(string passwd)
        {
            InputPasswdPage.Input(passwd, tabOut: false, scrollToElement: true, "passwd");
        }

        /// <summary>Clicks the button.</summary>
        /// <param name="button"></param>
        public void PressButton(string button)
        {
            switch (button.ToLower())
            {
                case PageNameRefs.LOGIN_HEROKUAPP_BUTTON:
                    ButtonLoginPage.Click(RunSettings.DefaultWebDriverTimeout, scrollToElement: true, "Login");
                    break;
                default:
                    throw new FrameworkException(button + " button is not supported");
            }
        }

        /// <summary>Gets the banner message.</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        public string GetBannerMessage()
        {
            return FlashMessagePage.Text();
        }

    }
}
