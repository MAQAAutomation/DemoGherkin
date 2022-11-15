using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using AventStack.ExtentReports.Utils;
using Demo.UIAutomation.Framework.Logging;
using static System.Reflection.MethodBase;

namespace Demo.UIAutomation.Framework.Helpers
{
    /// <summary>
    /// Class to support the possibility of get the caller variable name of a method. That is really useful for logging purposes as we can log
    /// the variable name which calls a method automatically.
    /// </summary>
    public static class CallerName
    {
        private static readonly ITestLogger Log = TestLog.GetTestLogger();

        /// <summary>
        /// Get the variable name which is calling to a method.
        /// </summary>
        /// <param name="methodName">Method name e.g Click</param>
        public static string GetCallerName(string methodName)
        {
            var stack = new StackTrace(1, true);
            var stackFrames = stack.GetFrames();

            foreach (var frame in stackFrames)
            {
                if (IsSequel(frame)
                    && !IsCurrentAssembly(frame))
                {
                    Log.Debug($"Method:{GetCurrentMethod().Name} Method:{methodName} on Frame: {frame}");
                    return ExtractVariableNameFrom(frame, methodName);
                }
            }

            return "element";
        }

        private static bool IsCurrentAssembly(StackFrame frame)
        {
            return frame.GetMethod().DeclaringType.Assembly == typeof(CallerName).Assembly;
        }

        private static bool IsSequel(StackFrame frame)
        {
            var frameNamespace = frame.GetMethod().DeclaringType.Namespace;
            return frameNamespace?.StartsWith("Sequel.", StringComparison.InvariantCultureIgnoreCase) == true;
        }

        private static string ExtractVariableNameFrom(StackFrame frame, string methodName)
        {
            var frameExpectedLine = frame.GetFileLineNumber();

            if (frameExpectedLine > 5)
            {
                for (var fileLine = frameExpectedLine; fileLine > frameExpectedLine - 5; fileLine--)
                {
                    var statement = GetSourceCodeLineFrom(frame, fileLine);
                    if (statement != null && IsMethodNameFoundOnStatement(statement, methodName))
                    {
                        var indexOfMethod = statement.IndexOf("." + methodName, StringComparison.InvariantCulture);
                        var notFound = -1;
                        if (indexOfMethod != notFound)
                        {
                            var candidate = statement.Substring(0, indexOfMethod).TrimStart();
                            if (IsAVarName(candidate))
                            {
                                return FormatToEnglish(candidate);
                            }
                        }
                    }
                }
            }

            return frame.GetMethod().Name + " method";
        }

        private static string GetSourceCodeLineFrom(StackFrame frame, int expectedLineNumber = -1)
        {
            if (expectedLineNumber == -1)
            {
                expectedLineNumber = frame.GetFileLineNumber();
            }
            string fileName = frame.GetFileName();

            Log.Debug($"Method:{GetCurrentMethod().Name} File:{fileName} Line number: {expectedLineNumber}");

            if (string.IsNullOrEmpty(fileName) || (expectedLineNumber == 0))
            {
                return null;
            }

            try
            {
                return ReadLine(fileName, expectedLineNumber);              
            }
            catch(Exception e)
            {
                Log.Debug(e.Message);
                return null;
            }
        }

        private static string ReadLine(string filePath, int lineNumber)
        {
            using (var reader = new StreamReader(File.OpenRead(filePath)))
            {
                string line;
                var currentLine = 1;
                while ((line = reader.ReadLine()) != null && currentLine < lineNumber)
                {
                    currentLine++;
                }
                return (currentLine == lineNumber) ? line : null;
            }
        }

        private static bool IsMethodNameFoundOnStatement(string statement, string methodName)
        {
            return statement.IndexOf("." + methodName, StringComparison.InvariantCulture) != -1;
        }

        private static bool IsAnStatement(string candidate)
        {
            return candidate.Contains(" ");
        }

        private static bool UsesNewKeyword(string candidate)
        {
            return Regex.IsMatch(candidate, @"new(\s?\[|\s?\{|\s\w+)");
        }

        private static bool IsStringLiteral(string candidate)
        {
            return candidate.StartsWith("\"", StringComparison.Ordinal);
        }

        private static bool IsAVarName(string candidate)
        {
            return !candidate.IsNullOrEmpty()
                   && !IsStringLiteral(candidate)
                   && !UsesNewKeyword(candidate)
                   && !IsAnStatement(candidate);
        }

        private static string FormatToEnglish(string callerName)
        {
            Log.Debug(GetCurrentMethod().Name + $" CallerName: {callerName}");

            if (callerName.Length == 0)
            {
                return string.Empty;
            }

            var callerNameWithSpaces = string.Empty;
            callerNameWithSpaces += callerName[0];
            for (var i = 1; i < callerName.Length; i++)
            {
                if (char.IsUpper(callerName[i]))
                {
                    callerNameWithSpaces += ' ';
                }
                callerNameWithSpaces += callerName[i];
            }

            return callerNameWithSpaces.ToLower()
                .Replace("btn", "button")
                .Replace("lbl", "label")
                .Replace("drp", "dropdown")
                .Replace("lnk", "link")
                .Replace("txt", "text field");
        }
    }
}
