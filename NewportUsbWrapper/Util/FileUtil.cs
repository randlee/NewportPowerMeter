using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using Visyn.Newport.Collections;

namespace Newport.Usb.Util
{
    public static class FileUtil
    {
        public static string GetTempFileName(string extension = null)
        {
            if (string.IsNullOrEmpty(extension)) return Path.GetTempFileName();
            if (!extension.StartsWith(".")) extension = "." + extension;
            return Path.GetTempFileName().Replace(".tmp", extension);
        }

        public static StringBuilder AppendDelimitedString(this StringBuilder builder, IEnumerable collection, string delimiter = ",")
        {
            var str = collection.Cast<object>().Aggregate("", (current, item) => current + (item?.ToString() ?? "") + delimiter);
            builder.Append(str);
            builder.Length -= delimiter.Length;

            return builder;
        }

        public static bool SaveDelimitedFileToDisk(string filePath, ICollection fileData, string delimiter = ",")
        {
            //try
            //{
                var dataString = new StringBuilder(1024);

                foreach (var line in fileData)
                {
                    var delimitedCollection = line as IDelimitedCollection;
                    if (delimitedCollection != null)
                    {
                        dataString.Append(delimitedCollection.ToDelimitedString(delimiter)).Append(Environment.NewLine);
                    }
                    else
                    {
                        var collection = (line is string) ? null : line as IEnumerable;
                        if (collection != null)
                            dataString.AppendDelimitedString(collection, delimiter).Append(Environment.NewLine);
                        else
                            dataString.Append(line).Append(Environment.NewLine);
                    }
                }

                return SaveFileToDisk(filePath, dataString.ToString());
            //}
            //catch (Exception exc)
            //{
            //    if (!HandleException($"Error appending file '{filePath}': {exc.Message}", exc, exceptionHandler)) throw;
            //}
            return false;
        }

        public static bool SaveFileToDisk(string filePath, string fileData)
        {
            if (string.IsNullOrEmpty(filePath)) return false;
            var dir = Path.GetDirectoryName(filePath);

            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (filePath == "") return false;
            File.WriteAllText(filePath, fileData);
            return true;
        }

        public static void XmlSerialize<T>(T data, string filename)
        {
            using (var writer = new StreamWriter(filename))
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                serializer.Serialize(writer, data);
            }
        }
    }
}
