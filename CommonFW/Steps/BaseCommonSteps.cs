using System.Collections.Generic;
using Demo.CommonFramework.Helpers;
using TechTalk.SpecFlow;

namespace Demo.CommonFramework.Steps
{
    public class BaseCommonSteps : TechTalk.SpecFlow.Steps
    {
        [StepArgumentTransformation(@"((?:.,\.+)*(?:.*))")]
        public IEnumerable<string> ListOfParametersTransform(string parameters)
        {
            return parameters.Split(new[] { Constants.StepParameterSeparatorChar });
        }
    }
}
