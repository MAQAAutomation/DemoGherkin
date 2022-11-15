using System;

namespace Demo.CommonFramework.ExceptionHandler
{
    public class InconclusiveFrameworkException : FrameworkException
    {
        public InconclusiveFrameworkException() { }
        public InconclusiveFrameworkException(string message) : base(message) { }
        public InconclusiveFrameworkException(string message, Exception inner) : base(message, inner) { }
    }
}
