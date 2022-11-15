using System;

namespace Demo.CommonFramework.ExceptionHandler
{
    public class FrameworkException : Exception
    {
        public FrameworkException() { }
        public FrameworkException(string message) : base(message) { }
        public FrameworkException(string message, Exception inner) : base(message, inner) { }
    }
}
