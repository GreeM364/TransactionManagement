using System.Globalization;
using System.Reflection;
using System.Text;

namespace TransactionManagement.Helpers
{
    /// <summary>
    /// Generic CSV parser class to parse CSV files into objects of type T.
    /// </summary>
    class CsvParser<T> where T : new()
    {
        /// <summary>
        /// Parses a CSV file into a list of objects of type T.
        /// </summary>
        /// <param name="file">The CSV file to parse.</param>
        /// <returns>A list of objects of type T parsed from the CSV file.</returns>
        public static List<T> ParseCsv(IFormFile file)
        {
            List<T> rows = new List<T>();

            var filePath = Path.GetTempFileName();
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            string[] lines = File.ReadAllLines(filePath);
            string[] headers = lines[0].Split(',');

            for (int i = 1; i < lines.Length; i++)
            {
                string[] fields = ParseCsvLine(lines[i]);
                T obj = MapDataToObject(headers, fields);
                rows.Add(obj);
            }

            File.Delete(filePath);
            
            return rows;
        }

        /// <summary>
        /// Parses a single line of CSV data into an array of fields.
        /// </summary>
        /// <param name="line">The CSV line to parse.</param>
        /// <param name="delimiter">The character used to delimit fields in the CSV line. Default is ','.</param>
        /// <param name="shield">The character used to shield fields containing the delimiter character. Default is '\"'.</param>
        /// <returns>An array of fields parsed from the CSV line.</returns>
        private static string[] ParseCsvLine(string line, char delimiter = ',', char shield = '"')
        {
            List<string> fields = new List<string>();
            StringBuilder currentField = new StringBuilder();
            bool insideQuotes = false;

            // Initialize the indexes of the beginning and end of the line
            int startIndex = line.StartsWith(shield) ? 1 : 0;
            int endIndex = line.EndsWith(shield) ? line.Length - 1 : line.Length;

            for (int i = startIndex; i < endIndex; i++)
            {
                char c = line[i];

                if (c == shield)
                {
                    insideQuotes = !insideQuotes;

                    // If these are consecutive screen characters, add one screen since it is our data
                    if (line[i + 1] == shield)
                    {
                        currentField.Append(c);
                        i++; // Skip the next screen since we've already processed it
                    }
                }
                // If this is a split character and we're not inside the screen, add the current field to the list
                else if (c == delimiter && !insideQuotes)
                {
                    fields.Add(currentField.ToString());
                    currentField.Clear();
                }
                else
                    currentField.Append(c);
            }

            fields.Add(currentField.ToString());

            return fields.ToArray();
        }

        /// <summary>
        /// Maps data fields to properties of an object of type T.
        /// </summary>
        /// <param name="headers">The headers from the CSV file.</param>
        /// <param name="fields">The data fields from a CSV line.</param>
        /// <returns>An object of type T with data mapped from the CSV fields.</returns>
        private static T MapDataToObject(string[] headers, string[] fields)
        {
            T obj = new T();
            PropertyInfo[] properties = typeof(T).GetProperties();

            for (int i = 0; i < Math.Min(headers.Length, fields.Length); i++)
            {
                PropertyInfo property = properties.FirstOrDefault(p => p.Name == headers[i]);
                if (property != null && property.CanWrite)
                {
                    object value = Convert.ChangeType(fields[i], property.PropertyType, CultureInfo.InvariantCulture);
                    property.SetValue(obj, value);
                }
            }

            return obj;
        }
    }
}