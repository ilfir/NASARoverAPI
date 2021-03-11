using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace NASARoverAPI
{
    public static class DateHelper
    {
        /// <summary>
        /// Read the input file dates.txt and return contents.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetDatesFromFileAsString(string fileName)
        {
            var result = "";
            // Open the text file and read contents using a stream reader.
            using (var sr = new StreamReader(fileName))
            {              
                result = sr.ReadToEnd();
            }
            return result;
        }

        public static List<DateTime> GetFormattedDatesFromString(string fileContents)
        {
            CultureInfo usCulture = new CultureInfo("en-US");
            var result = new List<DateTime>();
            string[] datesSplit = fileContents.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < datesSplit.Length; i++)
            {
                DateTime dt;
                DateTime.TryParse(datesSplit[i], usCulture, DateTimeStyles.None, out dt);
                result.Add(dt);                
            }
            return result;
        }

    }
}
