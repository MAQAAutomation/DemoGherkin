using System.Collections.Generic;
using Demo.TFSAutomationFramework.Request.Common;

namespace Demo.TFSAutomationFramework.Request.Test.Results.Update
{
    public class TestCaseResultRequest
    {
        public int Id { get; set; }
        public string State { get; set; }
        public string Comment { get; set; }
        public List<IdRequest> AssociatedBugs { get; set; } = new List<IdRequest>();
        public string FailureType { get; set; }
    }
}
