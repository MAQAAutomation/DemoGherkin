using Demo.TFSAutomationFramework.Response.Common;

namespace Demo.TFSAutomationFramework.Response.GetPoint
{
    public class Value
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public AssignedTo AssignedTo { get; set; }
        public bool Automated { get; set; }
        public IdResponse LastTestRun { get; set; }
        public IdResponse LastResult { get; set; }
        public string Outcome { get; set; }
        public string State { get; set; }
        public string LastResultState { get; set; }
        public TestCase TestCase { get; set; }
        public LastResultDetails LastResultDetails { get; set; }
    }
}
