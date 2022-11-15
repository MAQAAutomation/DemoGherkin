using NUnit.Framework;
using Demo.CommonFramework.Config;
using Demo.CommonFramework.Helpers;
using Demo.CommonFramework.Steps;
using Demo.DataUniverseFramework.SQL;
using TechTalk.SpecFlow;

namespace Demo.DataUniverseFramework.Steps
{
    public class CommonSteps : BaseCommonSteps
    {
        private DataBaseManager instanceDatasBaseManager;
        protected RequestProducer requestProducerInstance;
        private bool Result { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonSteps"/> class.
        /// </summary>
        /// <param name="producer">The producer.</param>
        public CommonSteps(RequestProducer producer)
        {
            requestProducerInstance = producer;
        }

        /// <summary>
        /// Givens an environment deployed.
        /// </summary>
        [Given("an environment deployed")]
        public void GivenAnEnvironmentDeployed()
        {
            instanceDatasBaseManager = new DataBaseManager(RunSettings.BackendServer);
        }

        /// <summary>
        /// Whens the run actions.
        /// </summary>
        /// <param name="sqlPathFile">The SQL path file.</param>
        [When("I run the actions included into the SQL file located in (.*)")]
        public void WhenRunActions( string sqlPathFile)
        {
            instanceDatasBaseManager.SqlPath = sqlPathFile;
            Result = instanceDatasBaseManager.RunStatements();
        }

        /// <summary>
        /// Whens the run actions SQL.
        /// </summary>
        /// <param name="sqlPath">The SQL path.</param>
        /// <param name="appLayer">The application layer.</param>
        [When("I run the query included into the SQL file located in (.*) executed in the (.*) database")]
        public void WhenRunQuerySQL(string sqlPath, string appLayer)
        {            
            requestProducerInstance.ModifyEnvironmentData(sqlPath
                , new SqlAccess(requestProducerInstance.GetSqlConfig(appLayer.ToLower().Equals("frontend"))), isStatement: false);
        }

        /// <summary>
        /// Whens the run statements.
        /// </summary>
        /// <param name="sqlPathFile">The SQL path file.</param>
        [Given("I run the statements included into the SQL file located in (.*) executed in the (.*) database")]
        [When("I run the statements included into the SQL file located in (.*) executed in the (.*) database")]
        public void WhenRunActionsSQL(string sqlPath, string appLayer)
        {
            requestProducerInstance.ModifyEnvironmentData(sqlPath
                , new SqlAccess(requestProducerInstance.GetSqlConfig(appLayer.ToLower().Equals("frontend"))));
        }

        /// <summary>
        /// Thens the is resul sucessfull.
        /// </summary>
        [Then("the result of the action is sucessfull")]
        public void ThenIsResulSucessfull()
        {
            Assert.IsTrue(Result);
        }

        /// <summary>
        /// Sets the value into data universe.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="key">The key.</param>
        [Given("I set the value (.*) with the key (.*) into the data universe")]
        [When("I set the value (.*) with the key (.*) into the data universe")]
        public void SetValueIntoDataUniverse(string value, string key)
        {
            requestProducerInstance.GenerateEnvironmentParamRequest(new string[] { value }, null, new SqlAccess(requestProducerInstance.GetSqlConfig()));
            requestProducerInstance.AddOrderedQueryValues(new string[] { key }, BaseUtils.BuildDictionary(new string[] { key }, new string[] { requestProducerInstance.RequestResponseProvider.Param[0] }));
        }
    }
}
