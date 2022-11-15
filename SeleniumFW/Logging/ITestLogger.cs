using System;

namespace Demo.UIAutomation.Framework.Logging
{
    public interface  ITestLogger
    {
        void Debug(object message);
        void Debug(object message, Exception exception);
        void Error(object message);
        void Error(object message, Exception exception);
        void Info(object message, Exception exception);
        void Info(object message);
        void Warn(object message);
        void Warn(object message, Exception exception);
        void Step(int stepNumber, string description);
        void Step(int stepNumber, string when, string then = null);
    }
}
