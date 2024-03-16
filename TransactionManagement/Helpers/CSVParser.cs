using System.Globalization;
using System.Reflection;
using System.Text;

class CSVParser<T> where T : new()
{
    public static List<T> ParseCSV(IFormFile file)
    {
        List<T> rows = new List<T>();

        try
        {
            var filePath = Path.GetTempFileName();
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            string[] lines = File.ReadAllLines(filePath);
            string[] headers = lines[0].Split(',');

            for (int i = 1; i < lines.Length; i++)
            {
                string[] fields = ParseCSVLine(lines[i]);
                T obj = MapDataToObject(headers, fields);
                rows.Add(obj);
            }

            File.Delete(filePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while parsing the CSV file: " + ex.Message);
        }

        return rows;
    }

    private static string[] ParseCSVLine(string line)
    {
        List<string> fields = new List<string>();
        StringBuilder currentField = new StringBuilder();
        bool insideQuotes = false;

        bool startWithQuote = line.StartsWith("\"");
        int startIndex = startWithQuote ? 1 : 0;

        for (int i = startIndex; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                insideQuotes = !insideQuotes;

                if (i == 0 && startWithQuote)
                    continue;

                if (insideQuotes && i < line.Length - 1 && line[i + 1] == '"')
                {
                    currentField.Append(c);
                    i++; 
                    continue;
                }

                if (insideQuotes && i == line.Length - 1)
                {
                    continue;
                }
            }
            else if (c == ',' && !insideQuotes)
            {
                fields.Add(currentField.ToString());
                currentField.Clear();
                continue;
            }

            currentField.Append(c);
        }

        fields.Add(currentField.ToString());

        for (int i = 0; i < fields.Count; i++)
        {
            if (fields[i].EndsWith("\""))
                fields[i] = fields[i].Substring(0, fields[i].Length - 1);
        }

        return fields.ToArray();
    }

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