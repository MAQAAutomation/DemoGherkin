using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Demo.TestReport.Framework.Helpers
{
    public static class ServiceHelper
    {
        /// <summary>
        /// Gets the name of the page service.
        /// </summary>
        /// <returns></returns>
        public static string GetPageServiceName()
        {
            var declaringType = GetMethod(3).DeclaringType;
            var name = declaringType != null
                ? GetDeclaringTypeName(declaringType)
                : string.Empty;
            return FileHelper.TrimIllegalCharacters(name);
        }

        /// <summary>
        /// Gets the name of the declaring type.
        /// </summary>
        /// <param name="declaringType">Type of the declaring.</param>
        /// <returns></returns>
        private static string GetDeclaringTypeName(Type declaringType)
        {
            return declaringType.Name.Contains("Service")
                    ? declaringType.Name.Replace("Service", string.Empty)
                    : declaringType.Name;
        }

        /// <summary>
        /// Gets the method.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static MethodBase GetMethod(int index)
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(index);
            return sf.GetMethod();
        }
    }
}
