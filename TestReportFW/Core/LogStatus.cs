using AventStack.ExtentReports;

namespace Demo.TestReport.Framework.Core
{
    public enum LogStatus
    {
        PASS = Status.Pass,
        FAIL = Status.Fail,
        FATAL = Status.Fatal,
        ERROR = Status.Error,
        WARNING = Status.Warning,
        INFO = Status.Info,
        SKIP = Status.Skip,
        DEBUG = Status.Debug,

        //Value only by logs level configurations     
        OFF = -1,
        ALL = 101
    }
};

