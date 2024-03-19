using ClosedXML.Excel;
using TransactionManagement.Entities;

namespace TransactionManagement.Helpers
{
    /// <summary>
    /// Helper class for exporting transaction data to Excel.
    /// </summary>
    public static class ExcelHelper
    {
        /// <summary>
        /// Exports a list of transactions to Excel format.
        /// </summary>
        /// <param name="transactions">The list of transactions to export.</param>
        /// <param name="columnsToInclude">The columns to include in the Excel file.</param>
        /// <returns>The byte array representing the Excel file.</returns>
        public static byte[] ExportToExcel(List<Transaction> transactions, List<string> columnsToInclude)
        {
            using var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add();

            // Write column headers to the worksheet
            for (int i = 0; i < columnsToInclude.Count; i++)
            {
                worksheet.Cell(1, i + 1).Value = columnsToInclude[i];
            }

            // Write transaction data to the worksheet
            for (int rowIndex = 0; rowIndex < transactions.Count; rowIndex++)
            {
                var transaction = transactions[rowIndex];
                for (int columnIndex = 0; columnIndex < columnsToInclude.Count; columnIndex++)
                {
                    var propertyName = columnsToInclude[columnIndex];
                    var property = transaction.GetType().GetProperty(propertyName);

                    if (property != null)
                    {
                        var propertyValue = property.GetValue(transaction, null);

                        // Check the property type and handle accordingly
                        if (property.PropertyType == typeof(DateTime))
                        {
                            worksheet.Cell(rowIndex + 2, columnIndex + 1).Value = (DateTime)propertyValue;
                        }
                        else if (property.PropertyType == typeof(decimal) || property.PropertyType == typeof(double))
                        {
                            worksheet.Cell(rowIndex + 2, columnIndex + 1).Value = Convert.ToDouble(propertyValue);
                        }
                        else
                        {
                            worksheet.Cell(rowIndex + 2, columnIndex + 1).Value = propertyValue?.ToString();
                        }
                    }
                }
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return stream.ToArray();
        }
    }
}
