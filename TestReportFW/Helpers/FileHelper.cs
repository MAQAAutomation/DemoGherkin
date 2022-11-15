using System.IO;
using System.IO.Compression;
using System.Text;
using NUnit.Framework;

namespace Demo.TestReport.Framework.Helpers
{
    public static class FileHelper
    {
        /// <summary>
        /// Zips the test result folder.
        /// </summary>
        /// <param name="pathFolder">The path folder.</param>
        /// <param name="pathfileZip">The pathfile zip.</param>
        public static void ZipTestResultFolder(string pathFolder, string pathfileZip)
        {
            int filesWithSameName = 0;

            while (File.Exists(pathfileZip))
            {
                pathfileZip = AddNumberFileName(pathfileZip, filesWithSameName++);
            }
            ZipFile.CreateFromDirectory(pathFolder, pathfileZip);
            TestContext.AddTestAttachment(pathfileZip);
        }

        /// <summary>
        /// Adds the name of the number file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="number">The number.</param>
        /// <returns></returns>
        public static string AddNumberFileName(string fileName, int number)
        {
            string[] fileNameSplitted = fileName.Split('.');
            return fileNameSplitted[0].Substring(0, fileNameSplitted[0].Length - 2) + '_' + number + '.' + fileNameSplitted[1];
        }

        /// <summary>
        /// Trims the illegal characters.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        public static string TrimIllegalCharacters(string candidate, string filePath = "")
        {
            string candidateWithoutIllegalCharacters = candidate;
            candidateWithoutIllegalCharacters = string.Join("_", candidateWithoutIllegalCharacters.Split(Path.GetInvalidFileNameChars()));
            candidateWithoutIllegalCharacters = string.Join("_", candidateWithoutIllegalCharacters.Split(Path.GetInvalidPathChars()));
            candidateWithoutIllegalCharacters = candidateWithoutIllegalCharacters.Replace("#", "").Replace("-", "_");

            var fileFullPath = filePath + candidateWithoutIllegalCharacters;
            if (fileFullPath.Length > 256)
            {
                if (Path.HasExtension(fileFullPath))
                {
                    var extension = Path.GetExtension(fileFullPath);
                    return candidateWithoutIllegalCharacters.Substring(0, 252 - filePath.Length) + extension;
                }
                else
                {
                    return candidateWithoutIllegalCharacters.Substring(0, 255 - filePath.Length);
                }
            }
            return candidateWithoutIllegalCharacters;
        }

        /// <summary>
        /// Logs the stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="filePath">The file path.</param>
        public static void LogStream(string stream, string fileName, string filePath, out string fullPath)
        {
            fullPath = string.Empty;
            if (!string.IsNullOrEmpty(filePath))
            {
                fullPath = filePath + TrimIllegalCharacters(fileName, filePath);
                SaveFile(stream, fullPath);
                TestContext.AddTestAttachment(fullPath);
            }
        }

        /// <summary>
        /// Saves the file.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="filePath">The file path.</param>
        public static void SaveFile(string value, string filePath)
        {
            using (var stream = File.Open(filePath, FileMode.OpenOrCreate))
            {
                stream.Write(Encoding.ASCII.GetBytes(value), 0, Encoding.ASCII.GetByteCount(value));
                stream.Close();
            }
        }

        /// <summary>
        /// Replaces the last occurrence.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="find">The find.</param>
        /// <param name="replace">The replace.</param>
        /// <returns></returns>
        public static string ReplaceLastOccurrence(string source, string find, string replace)
        {
            int place = source.LastIndexOf(find);
            string result = source.Remove(place, find.Length).Insert(place, replace);
            return result;
        }
    }
}
