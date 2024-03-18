using ClosedXML.Excel;
using TransactionManagement.Entities;

namespace TransactionManagement.Helpers
{
    public static class ExcelHelper
    {
        public static byte[] ExportToExcel(List<Transaction> transactions, List<string> columnsToInclude)
        {
            using var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add();

            for (int i = 0; i < columnsToInclude.Count; i++)
            {
                worksheet.Cell(1, i + 1).Value = columnsToInclude[i];
            }

            for (int rowIndex = 0; rowIndex < transactions.Count; rowIndex++)
            {
                var transaction = transactions[rowIndex];
                for (int columnIndex = 0; columnIndex < columnsToInclude.Count; columnIndex++)
                {
                    var propertyName = columnsToInclude[columnIndex];
                    var propertyValue = transaction.GetType().GetProperty(propertyName)?.GetValue(transaction, null)?.ToString();
                    worksheet.Cell(rowIndex + 2, columnIndex + 1).Value = propertyValue;
                }
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return stream.ToArray();
        }
    }
}
