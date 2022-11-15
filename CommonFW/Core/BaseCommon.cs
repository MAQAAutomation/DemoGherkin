using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Demo.CommonFramework.Helpers;

namespace Demo.CommonFramework
{
    public class BaseCommon
    {
        protected BaseCommon() { }

        /// <summary>
        /// Searches for class in assembly.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <returns></returns>
        public static Type SearchForClassInAssembly(string className)
        {
            List<string> files = BaseFileUtils.GetFilesByExtension(BaseFileUtils.CURRENT_PATH, ".dll");
            files.RemoveAll(item => item.Contains("Temp") || item.Contains("Microsoft") || item.ToLower().Contains("mongo")
                                    || item.Contains("libzstd") || item.ToLower().Contains("snappy"));

            List<AssemblyName> loadedAssemblies = Assembly.GetExecutingAssembly()?.GetReferencedAssemblies().ToList();
            for (int i = 0; i < loadedAssemblies.Count; i++)
            {
                files.RemoveAll(item => item.Contains(loadedAssemblies[i].Name));
            }

            Assembly domainAssembly = null;
            for (int i = 0; i < files.Count; i++)
            {
                domainAssembly = Assembly.LoadFrom(files[i]);
                Type type = domainAssembly.GetType(className);
                if (type != null) return type;
            }

            return null;
        }
    }
}
