using Demo.CommonFramework.Steps;
using Demo.TestReport.Framework.Core;
using Demo.UIAutomation.Framework.Browsers;
using Demo.WebFW.Framework.Helpers;
using Demo.WebFW.Framework.Utils;
using TechTalk.SpecFlow;

namespace Demo.WebFW.Framework.Steps
{
    public class CommonSteps : BaseCommonSteps
    {
        public StepPages page;
        public IBrowser Browser { get; set; }
        private string Action { get; set; }

        /// <summary>Opens the application.</summary>
        /// <param name="mainPage">The main page.</param>
        [Given("I open the browser with the (.*) main page")]
        [When("I open the browser with the (.*) main page")]
        [Then("I open the browser with the (.*) main page")]
        [Given("yo abro el navegador con la (.*) pagina principal")]
        public void OpenApplication(string mainPage)
        {
            page = new StepPages(Browser);
            TestHelper.IsAt(mainPage, page);
        }


        /// <summary>Whens the i log in.</summary>
        /// <param name="applications">The applications.</param>
        [Given("I login into the application (.*)")]
        [When("I login into the application (.*)")]
        [Then("I login into the application (.*)")]
        [When("yo me logueo en la aplicacion (.*)")]
        public void WhenILogIn(string applications)
        {
            User user = UserService.GetTestUser();
            page = new StepPages(Browser);

            TestHelper.Authentication(page, applications, user.UserName, user.Password);
            page.addMoreLogScreenShots("Login details filled");
            TestHelper.IsAt(page.SecureAreaPageInstance);
            ExtentTestManager.LogInfo("Login successfull");
        }

        /// <summary>
        /// Presses the button.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <param name="pageName">Name of the page.</param>
        [Given("I press the (Button|Link) (.*) of the (.*) page")]
        [When("I press the (Button|Link) (.*) of the (.*) page")]
        [Then("I press the (Button|Link) (.*) of the (.*) page")]
        [When("yo presiono el (Boton|Enlace) (.*) de la (.*) pagina")]
        public void PressButton(string type, string button, string pageName)
        {
            TestHelper.PressButton(page, button, pageName);
        }

        /// <summary>
        /// Reviews the loaded page.
        /// </summary>
        /// <param name="pageName">Name of the page.</param>
        [Given("the (.*) page is loaded successfully")]
        [When("the (.*) page is loaded successfully")]
        [Then("the (.*) page is loaded successfully")]
        [Given("la (.*) pagina es cargado correctamente")]
        [When("la (.*) pagina es cargado correctamente")]
        [Then("la (.*) pagina es cargado correctamente")]
        public void ThenReviewLoadedPage(string pageName)
        {
            TestHelper.IsAt(pageName, page);
        }



        /**

        /// <summary>
        /// Selects the policy section tab.
        /// </summary>
        /// <param name="tabName">Name of the tab.</param>
        [Given("I select (.*) tab in (.*) page")]
        [When("I select (.*) tab in (.*) page")]
        public void SelectPolicySectionTab(string tabName, string pageName)
        {
            OriginTestHelper.SelectTab(page, pageName, tabName);
        }

        

        /// <summary>Refreshes the website.</summary>
        [Given(@"I refresh the website")]
        [When(@"I refresh the website")]
        public void RefreshTheWebsite()
        {
            Browser.Refresh();
        }

        /// <summary>
        /// Fills the page.
        /// </summary>
        /// <param name="pageName">Name of the item.</param>
        /// <param name="tableFieldsValues">The table fields values.</param>
        [Given("I fill the (.*) page with the values")]
        [When("I fill the (.*) page with the values")]
        [Then("I fill the (.*) page with the values")]
        public void FillPage(string pageName, Table tableFieldsValues)
        {
            FillPageUsingSQL(pageName, null, tableFieldsValues);
        }

        /// <summary>
        /// Fills the page using SQL.
        /// </summary>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="sqlPath">The SQL path.</param>
        /// <param name="tableFieldsValues">The table fields values.</param>
        [Given("I fill the (.*) page with the values using the SQL file located at (.*)")]
        [When("I fill the (.*) page with the values using the SQL file located at (.*)")]
        [Then("I fill the (.*) page with the values using the SQL file located at (.*)")]
        public void FillPageUsingSQL(string pageName, string sqlPath, Table tableFieldsValues)
        {
            BaseUtils.TableToIEnumerable(tableFieldsValues, out string[] fieldsName, out string[] values);
            requestProducerInstance.GenerateEnvironmentParamRequest(values, sqlPath, new SqlAccess(requestProducerInstance.GetSqlConfig()));
            Dictionary<string, string> keyValues = BaseUtils.BuildDictionary(fieldsName, requestProducerInstance.RequestResponseProvider.Param);
            ExtentTestManager.Log(LogStatus.INFO, MarkupHelper.CreateTable(keyValues.ToTable()));
            OriginTestHelper.IsAt(pageName, page);
            OriginTestHelper.FillPageIn(page, pageName, keyValues);
        }

        /// <summary>
        /// Reviews the page.
        /// </summary>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="tableFieldsValues">The table fields values.</param>
        [Given("I review the (.*) page with the values")]
        [When("I review the (.*) page with the values")]
        [Then("I review the (.*) page with the values")]
        public void ReviewPage(string pageName, Table tableFieldsValues)
        {
            ReviewPageUsingSQL(pageName, null, tableFieldsValues);
        }

        /// <summary>
        /// Reviews the page using SQL.
        /// </summary>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="sqlPath">The SQL path.</param>
        /// <param name="tableFieldsValues">The table fields values.</param>
        [Given("I review the (.*) page with the values using the SQL file located at (.*)")]
        [When("I review the (.*) page with the values using the SQL file located at (.*)")]
        [Then("I review the (.*) page with the values using the SQL file located at (.*)")]
        public void ReviewPageUsingSQL(string pageName, string sqlPath, Table tableFieldsValues)
        {
            BaseUtils.TableToIEnumerable(tableFieldsValues, out string[] fieldsName, out string[] values);
            requestProducerInstance.GenerateEnvironmentParamRequest(values, sqlPath, new SqlAccess(requestProducerInstance.GetSqlConfig()));
            Dictionary<string, string> keyValues = BaseUtils.BuildDictionary(fieldsName, requestProducerInstance.RequestResponseProvider.Param);
            ExtentTestManager.Log(LogStatus.INFO, MarkupHelper.CreateTable(keyValues.ToTable()));
            string error = string.Empty;
            OriginTestHelper.IsAt(pageName, page);
            Assert.IsTrue(OriginTestHelper.ReviewPageIn(page, pageName, keyValues, out error), error);
        }

        /// <summary>
        /// Fills the page.
        /// </summary>
        /// <param name="pageName">Name of the item.</param>
        /// <param name="tableFieldsValues">The table fields values.</param>
        [Given("I fill the (.*) page with the values")]
        [When("I fill the (.*) page with the values")]
        [Then("I fill the (.*) page with the values")]
        public void FillPage(string pageName, Table tableFieldsValues)
        {
            FillPageUsingSQL(pageName, null, tableFieldsValues);
        }

        /// <summary>
        /// Fills the page using SQL.
        /// </summary>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="sqlPath">The SQL path.</param>
        /// <param name="tableFieldsValues">The table fields values.</param>
        [Given("I fill the (.*) page with the values using the SQL file located at (.*)")]
        [When("I fill the (.*) page with the values using the SQL file located at (.*)")]
        [Then("I fill the (.*) page with the values using the SQL file located at (.*)")]
        public void FillPageUsingSQL(string pageName, string sqlPath, Table tableFieldsValues)
        {
            BaseUtils.TableToIEnumerable(tableFieldsValues, out string[] fieldsName, out string[] values);
            requestProducerInstance.GenerateEnvironmentParamRequest(values, sqlPath, new SqlAccess(requestProducerInstance.GetSqlConfig()));
            Dictionary<string, string> keyValues = BaseUtils.BuildDictionary(fieldsName, requestProducerInstance.RequestResponseProvider.Param);
            ExtentTestManager.Log(LogStatus.INFO, MarkupHelper.CreateTable(keyValues.ToTable()));
            OriginTestHelper.IsAt(pageName, page);
            OriginTestHelper.FillPageIn(page, pageName, keyValues);
        }

        [Given("the application available")]
        public void GivenTheUrlInBrowser()
        {
            AuthenticationPage authPage = new AuthenticationPage(Browser);
            authPage.WaitForValidation(RunSettings.DefaultTransitionsTimeout * 1000);
            Assert.IsTrue(authPage.IsDoneLoading());
        }
*/
    }
}