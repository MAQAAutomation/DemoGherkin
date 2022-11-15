using Demo.CommonFramework.Config;
using Demo.UIAutomation.Framework.Browsers;
using Demo.UIAutomation.Framework.Extensions.SqWebElements;
using Demo.WebFW.Framework;
using OpenQA.Selenium;

namespace WebFW.Pages.HeroKuApp
{
    public class MainPage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationPage"/> class.
        /// </summary>
        /// <param name="browser">The browser.</param>
        public MainPage(IBrowser browser) : base(browser) { }

        private SqWebElement TitlePage => () => RootElement.FindElement(By.XPath("//title[contains(.,'The Internet')]"));
        private SqWebElement NamePage => () => RootElement.FindElement(By.XPath("//h2[contains(.,'Available Examples')]"));
        private SqWebElement LinkElementPage(string ElementDescription) => () => RootElement.FindElement(By.XPath("//a[contains(.,'" + ElementDescription + "')]"));



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

        /// <summary>Links to element.</summary>
        /// <param name="elementDescription">The element description.</param>
        public void LinkToElement(string elementDescription)
        {
            LinkElementPage(elementDescription).Click(RunSettings.DefaultWebDriverTimeout, scrollToElement: true, elementDescription);
        }

    }
}
