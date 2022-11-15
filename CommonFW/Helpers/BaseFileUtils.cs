using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Demo.CommonFramework.ExceptionHandler;
using Demo.TestReport.Framework.Core;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace Demo.CommonFramework.Helpers
{
    public class BaseFileUtils
    {
        public static readonly string CURRENT_PATH = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName;
        public static readonly string PARENT_PATH = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName;
        public static readonly string PATH_TO_RESULTS = PARENT_PATH + Path.DirectorySeparatorChar + "TestResults";
        public static readonly string DEFAULT_SQL_FOLDER = "Templates";

        public enum EFileExtension
        {
            [System.ComponentModel.Description(".json")]
            JSON,
            [System.ComponentModel.Description(".sql")]
            SQL,
            [System.ComponentModel.Description(".txt")]
            TXT,
            [System.ComponentModel.Description(".xml")]
            XML
        }

        public enum EStreamType
        {
            Request,
            Response,
            Expected,
            [System.ComponentModel.Description("")]
            None
        }

        /// <summary>
        /// Logs to file.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="name">The name.</param>
        /// <param name="typeOfStream">The type of stream.</param>
        /// <param name="extension">The extension.</param>
        /// <param name="details">The details.</param>
        public static void AttachFileToLog(string stream, string name, EStreamType typeOfStream, EFileExtension extension, string details)
        {
            if (string.IsNullOrEmpty(stream)) return;

            string fileName = name + BaseUtils.GetEnumDescription(typeOfStream) + DateTime.Now.ToString("yyyyMMdd_HHmmssfff")
                            + BaseUtils.GetEnumDescription(extension);

            if (extension.Equals(EFileExtension.XML))
            {
                stream = BaseXmlHelper.PrettyXml(stream);
            }

            ExtentTestManager.LogInfoWithAttach(details, ExtentTestManager.TestResultResourcesFilesPath, fileName, stream);
        }

        /// <summary>
        /// Gets the files by extension.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="extension">The extension. (i.e. ".dll")</param>
        /// <returns></returns>
        public static List<string> GetFilesByExtension(string path, string extension)
        {
            if (!Directory.Exists(path)) return new List<string>();

            var ext = new List<string> { extension };

            var list = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                 .Where(s => ext.Contains(Path.GetExtension(s)));

            return list.ToList();
        }

        /// <summary>
        /// Get the content form a file 
        /// </summary>
        /// <param name="relativePath">Relative path into the project</param>
        /// <returns>File content as a string</returns>
        [MethodImpl(Synchronized)]
        public static string GetFileContent(string relativePathNameFile)
        {
            string pathToFile = CURRENT_PATH + Path.DirectorySeparatorChar + relativePathNameFile;
            if (!File.Exists(pathToFile))
            {
                throw new FrameworkException("The current file is not found in the indicated directory: '" + pathToFile + "', please copy it in the correct directory");
            }

            return File.ReadAllText(pathToFile);
        }
    }
}

