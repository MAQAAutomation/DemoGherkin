using System.Linq;
using Demo.UIAutomation.Framework.Browsers;
using Demo.UIAutomation.Framework.Extensions.SqWebElements;
using Demo.WebApp.Framework;
using OpenQA.Selenium;

namespace Demo.WebFW.Framework
{
    public abstract class Page : WebAppPage
    {
        protected Page(IBrowser browser) : base(browser) { }
        private int m_child = 1;

        private string SidePanelId { get; set; }
        private SqWebElement DropdownSearchField => () => RootElement.FindElement(By.Id(SidePanelId));
        private SqWebElement SelectInputField => () => RootElement.FindElement(By.CssSelector("#select2-drop:not([style*='display: none']) .select2-input"));
        private SqWebElement SelectInputReactField => () => RootElement.FindElement(By.XPath("//div[@class='ui-select-dropdown select2-drop select2-with-searchbox select2-drop-active']/div/input"));
        private SqWebElement SelectInputMultipleField => () => RootElement.FindElement(By.CssSelector(".select2-search-field .select2-input"));


        private SqWebElement ReactItemResultList => () => RootElement.FindElement(By.XPath("//li[contains(@class,'ui-select-choices-row')][" + m_child + "]"));



        private SqListWebElement ListOfAngular11DropdownOptions => () => RootElement.FindElements(By.XPath("//ul[@class='ka-dropdown--list ka-dropdown-simpleselect--list']/li |" +
                                                                                                            " //ul[@class='select2-result-single']/li"));

        private SqListWebElement OriginSpinner => () => RootElement.FindElements(By.XPath("//div[@data-testid='originLoadingSpinner']/div/p " +
                                                                                                "| //div[@data-screen-editor-custom-element='OriginSpinner']/div/p " +
                                                                                                "| //div[@class='custom-spinner__panel']/p")).ToList();
        protected SqListWebElement ModalDialogs => () => RootElement.FindElements(By.CssSelector(".modal__dialog")).ToList();
        protected SqWebElement BtnConfirmDialog => () => RootElement.FindElement(ByDataTestId("confirm"));
        protected SqWebElement BtnCancelDialog => () => RootElement.FindElement(ByDataTestId("cancel"));
        protected SqWebElement FieldReason => () => RootElement.FindElement(ByDataTestId("reason")).FindElement(By.XPath("//div[1]/div[1]/a[1]/span"));
        protected SqWebElement FieldComment => () => RootElement.FindElement(ByDataTestId("comment"));
        protected SqWebElement BtnMaximizeElement => () => RootElement.FindElement(By.CssSelector("a[class='split-screen-item__fullscreen-button'], a[class='split-screen-item__fullscreen-button split-screen-item__fullscreen-button--breadcrumb']"));
        protected SqWebElement BtnCloseRHScreen => () => RootElement.FindElement(By.XPath("//div[@ng-if ='policyCtrl.canClose()']/div"));
        protected SqListWebElement MultivalueDropDownSelectedElements => () => RootElement.FindElements(By.CssSelector("li[class=\"ui-select-match-item select2-search-choice ng-scope\"]")).ToList();
        protected SqListWebElement ElementsMultivalueWithoutFilter => () => RootElement.FindElements(By.XPath("//div[@class=\"select2-result-label ui-select-choices-row-inner\"] | //div[@class=\"select2-result-label\"]")).ToList();
        protected SqWebElement OriginPopUp => () => RootElement.FindElement(By.CssSelector("div[class='popup-service-btn-container'] , div[class='custom-popup__button-container']"));
        protected SqWebElement OriginPopUpGeneralError => () => RootElement.FindElement(By.Id("error-modal-close-button"));
        protected SqWebElement HamburgerMenuBtn => () => RootElement.FindElement(By.Id("btn-toggle-drawer"));
        protected SqWebElement BtnNavHome => () => RootElement.FindElement(By.Id("btn-nav-home"));
        protected SqWebElement BtnNavWork => () => RootElement.FindElement(By.Id("btn-nav-work"));
        protected SqWebElement BtnNavNewEnquiry => () => RootElement.FindElement(By.Id("btn-nav-enquiry"));
        protected SqWebElement BtnNavNewOpenMarketEnquiry => () => RootElement.FindElement(By.Id("btn-nav-openmarketenquiry"));
        protected SqWebElement BtnNavNewPolicy => () => RootElement.FindElement(By.Id("btn-nav-openmarketpolicy"));
        protected SqWebElement BtnNavNewOrg => () => RootElement.FindElement(By.Id("btn-nav-organisation"));
        protected SqWebElement BtnNavSearch => () => RootElement.FindElement(By.Id("btn-nav-search"));
        protected SqWebElement BtnNavConfiguration => () => RootElement.FindElement(By.Id("btn-nav-refdata"));


    }

}
