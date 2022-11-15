using BoDi;
using TechTalk.SpecFlow;

namespace Demo.RestServiceFramework.Steps
{
    [Binding]
    public sealed class RestServiceHooks : CommonSteps
    {
        private readonly IObjectContainer m_objectContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="RestServiceHooks"/> class.
        /// </summary>
        /// <param name="objectContainer">The object container.</param>
        public RestServiceHooks(IObjectContainer objectContainer)
        {
            m_objectContainer = objectContainer;
        }
    }
}
