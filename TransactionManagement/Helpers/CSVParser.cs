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
        /// <returns>An array of fields parsed from the CSV line.</returns>
        private static string[] ParseCsvLine(string line)
        {
            List<string> fields = new List<string>();
            StringBuilder currentField = new StringBuilder();
            bool insideQuotes = false;

            // Check if the string starts with double quotes
            bool startWithQuote = line.StartsWith("\"");
            int startIndex = startWithQuote ? 1 : 0;

            for (int i = startIndex; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    insideQuotes = !insideQuotes;

                    // If they are double quotes and we are not in the middle of the data, we skip them
                    if (i == 0 && startWithQuote)
                        continue;

                    // If these are double quotes inside the data, just add them to the current field
                    if (insideQuotes && i < line.Length - 1 && line[i + 1] == '"')
                    {
                        currentField.Append(c);
                        i++; // Skip the next character because we've already processed it
                        continue;
                    }

                    // If these are the last double quotes and we are at the end of the line, remove them
                    if (insideQuotes && i == line.Length - 1)
                    {
                        continue;
                    }
                }
                // If it's a comma and we're not inside double quotes, then we add the current field to the list of fields
                else if (c == ',' && !insideQuotes)
                {
                    fields.Add(currentField.ToString());
                    currentField.Clear();
                    continue;
                }

                currentField.Append(c);
            }

            fields.Add(currentField.ToString());

            // Remove the closing quotation marks, if they are at the end of the line
            for (int i = 0; i < fields.Count; i++)
            {
                if (fields[i].EndsWith("\""))
                    fields[i] = fields[i].Substring(0, fields[i].Length - 1);
            }

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