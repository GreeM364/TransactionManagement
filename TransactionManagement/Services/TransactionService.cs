using System.Globalization;
using TransactionManagement.DatabaseManager.Interfaces;
using TransactionManagement.Entities;
using TransactionManagement.Exceptions;
using TransactionManagement.Helpers;
using TransactionManagement.Models.Requests;
using TransactionManagement.Models.Responses;
using TransactionManagement.Services.Interfaces;

namespace TransactionManagement.Services
{
    /// <summary>
    /// Service for managing transactions.
    /// </summary>
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionManager _transactionManager;
        private readonly IIpInfoService _ipInfoService;

        public TransactionService(ITransactionManager transactionManager, IIpInfoService infoService)
        {
            _transactionManager = transactionManager;
            _ipInfoService = infoService;
        }

        /// <summary>
        /// Saves transactions from a CSV file.
        /// </summary>
        /// <remarks>
        /// This method parses a CSV file containing transaction data, removes duplicates, processes transactions,
        /// and saves them to the database. It returns a list of processed transactions.
        /// </remarks>
        /// <param name="csvFile">The CSV file containing transaction data.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of processed transactions.</returns>
        public async Task<List<TransactionResponse>> SaveAsync(IFormFile csvFile, CancellationToken cancellationToken)
        {
            var transactionsRequestScv = CsvParser<TransactionRequestCsv>.ParseCsv(csvFile);

            transactionsRequestScv = RemoveDuplicatesAndKeepLatest(transactionsRequestScv);

            var transactionsToSave = Mapper.TransactionRequestCsvToTransaction(transactionsRequestScv);

            var (insertedTransactions, updatedTransactions) =
                await SplitTransactionsAsync(transactionsToSave, cancellationToken);

            await Task.WhenAll(
                _transactionManager.InsertTransactionsAsync(insertedTransactions, cancellationToken),
                _transactionManager.UpdateTransactionsAsync(updatedTransactions, cancellationToken)
            );

            var transactionsResponse = Mapper.TransactionToTransactionResponse(insertedTransactions.Concat(updatedTransactions).ToList());

            return transactionsResponse;
        }

        /// <summary>
        /// Exports transactions to an Excel file based on the provided request parameters.
        /// </summary>
        /// <remarks>
        /// This method exports transactions to an Excel file based on the provided request parameters.
        /// The request must include parameters specifying the transactions to be included in the export, as well as which fields to include.
        /// If successful, it returns the Excel file containing the exported transactions.
        /// </remarks>
        /// <param name="request">The request containing parameters for exporting transactions.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An array of bytes representing the Excel file containing the exported transactions.
        /// </returns>
        /// <exception cref="BadRequestException">
        /// Thrown when the start date is greater than the end date or when no fields are included for export.
        /// </exception>
        /// <exception cref="NotFoundException">
        /// Thrown when no transactions are found for the given date range.
        /// </exception>
        public async Task<byte[]> ExportAsync(ExportTransactionsRequest request, CancellationToken cancellationToken)
        {
            if (request.StartDate > request.EndDate)
                throw new BadRequestException("Start date cannot be greater than end date");

            if (!request.IsAtLeastOneFieldIncluded())
                throw new BadRequestException("At least one field must be included for export.");

            var columnsToInclude = GetColumnsToInclude(request);

            var transactions = await _transactionManager.GetTransactionsByDateAsync(request.StartDate, request.EndDate, columnsToInclude, cancellationToken);

            if (transactions == null || transactions.Count == 0)
                throw new NotFoundException("No transactions found for the given date range");

            var excelBytes = ExcelHelper.ExportToExcel(transactions, columnsToInclude);

            return excelBytes;
        }

        /// <summary>
        /// Retrieves transactions for the client's time zone and specified year, optionally month.
        /// </summary>
        /// <remarks>
        /// This method retrieves transactions for the client's time zone and specified year. 
        /// If a specific month is provided, transactions for that month are retrieved; otherwise, transactions for the entire year are retrieved.
        /// </remarks>
        /// <param name="year">The year for which transactions are requested.</param>
        /// <param name="month">The month for which transactions are requested (optional).</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A list of <see cref="TransactionResponse"/> objects representing the retrieved transactions.
        /// </returns>
        /// <exception cref="BadRequestException">
        /// Thrown when the provided year is less than 1.
        /// </exception>
        public async Task<List<TransactionResponse>> GetTransactionsForClientTimeZoneAsync(int year, string? month, CancellationToken cancellationToken)
        {
            if (year < 1)
                throw new BadRequestException("Year must be greater than or equal to 1.");

            var monthNumber = ParseMonth(month);

            var transactions = await _transactionManager.GetTransactionsForClientTimeZoneAsync(year, monthNumber, cancellationToken);

            if (transactions.Count == 0)
                return Enumerable.Empty<TransactionResponse>().ToList();

            var transactionsResponse = Mapper.TransactionToTransactionResponse(transactions);

            return transactionsResponse;
        }

        /// <summary>
        /// Retrieves transactions for the current user's time zone, specified year, and optionally month.
        /// </summary>
        /// <remarks>
        /// This method retrieves transactions for the current user's time zone based on their IP address, 
        /// for the specified year and month (if provided). If successful, it returns the retrieved transactions.
        /// </remarks>
        /// <param name="clientIp">The IP address of the client.</param>
        /// <param name="year">The year for which transactions are requested.</param>
        /// <param name="month">The month for which transactions are requested (optional).</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A list of <see cref="TransactionResponse"/> objects representing the retrieved transactions.
        /// </returns>
        /// <exception cref="BadRequestException">
        /// Thrown when the provided year is less than 1.
        /// </exception>
        public async Task<List<TransactionResponse>> GetTransactionsForCurrentTimeZoneAsync(string clientIp, int year, string? month, CancellationToken cancellationToken)
        {
            if (year < 1)
                throw new BadRequestException("Year must be greater than or equal to 1.");

            var monthNumber = ParseMonth(month);

            var currentTimeZone = await _ipInfoService.GetCurrentTimeZoneAsync(clientIp);
            var standardCurrentTimeZone = TimeZoneHelper.ConvertIanaToWindows(currentTimeZone);

            var transactions = await _transactionManager.GetTransactionsForCurrentTimeZoneAsync(standardCurrentTimeZone, year, monthNumber, cancellationToken);

            if (transactions.Count == 0)
                return Enumerable.Empty<TransactionResponse>().ToList();

            var transactionsResponse = Mapper.TransactionToTransactionResponse(transactions);

            return transactionsResponse;
        }

        /// <summary>
        /// Removes duplicate transactions and retains the latest ones.
        /// </summary>
        /// <param name="transactions">The list of transactions to process.</param>
        /// <returns>
        /// A list of unique transactions where only the latest occurrence of each transaction ID is retained.
        /// </returns>
        private List<TransactionRequestCsv> RemoveDuplicatesAndKeepLatest(List<TransactionRequestCsv> transactions)
        {
            var uniqueTransactions = transactions
                .GroupBy(t => t.transaction_id)
                .Select(group => group.Last())
                .ToList();

            return uniqueTransactions;
        }

        /// <summary>
        /// Splits the transactions into two lists: inserted transactions and updated transactions.
        /// </summary>
        /// <param name="transactionsToSave">The list of transactions to process.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A tuple containing two lists:
        /// - <paramref name="insertedTransactions"/>: The transactions to be inserted.
        /// - <paramref name="updatedTransactions"/>: The transactions to be updated.
        /// </returns>
        private async Task<(List<Transaction> insertedTransactions, List<Transaction> updatedTransactions)>
            SplitTransactionsAsync(List<Transaction> transactionsToSave, CancellationToken cancellationToken)
        {
            var insertedTransactions = new List<Transaction>();
            var updatedTransactions = new List<Transaction>();

            var existingTransactionIds = await _transactionManager.GetExistingTransactionIdsAsync(transactionsToSave.Select(t => t.TransactionId).ToList(), cancellationToken);

            foreach (var transaction in transactionsToSave)
            {
                if (existingTransactionIds.Contains(transaction.TransactionId))
                {
                    updatedTransactions.Add(transaction);
                }
                else
                {
                    insertedTransactions.Add(transaction);
                }
            }

            return (insertedTransactions, updatedTransactions);
        }

        /// <summary>
        /// Parses the month string into its corresponding month number.
        /// </summary>
        /// <param name="month">The month string to parse.</param>
        /// <returns>
        /// An integer representing the month number (1-12) if parsing is successful; otherwise, null.
        /// </returns>
        /// <exception cref="BadRequestException">Thrown when the month parameter is not in a valid format.</exception>
        private int? ParseMonth(string? month)
        {
            if (month is not null)
            {
                if (!DateTime.TryParseExact(month, "MMMM", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedMonth))
                {
                    throw new BadRequestException("Invalid month format. The month parameter should be a valid month name (e.g., January, February, etc.).");
                }

                return parsedMonth.Month;
            }

            return null;
        }

        /// <summary>
        /// Gets the list of columns to include based on the specified export request.
        /// </summary>
        /// <param name="request">The export request containing parameters for exporting transactions.</param>
        /// <returns>A list of column names to include in the export.</returns>
        private List<string> GetColumnsToInclude(ExportTransactionsRequest request)
        {
            var columnsToInclude = new List<string>();

            if (request.IncludeTransactionId)
                columnsToInclude.Add("TransactionId");

            if (request.IncludeName)
                columnsToInclude.Add("Name");

            if (request.IncludeEmail)
                columnsToInclude.Add("Email");

            if (request.IncludeAmount)
                columnsToInclude.Add("Amount");

            if (request.IncludeTransactionDate)
                columnsToInclude.Add("TransactionDate");

            if (request.IncludeTimezone)
                columnsToInclude.Add("Timezone");

            if (request.IncludeLocation)
                columnsToInclude.AddRange(new[] { "Latitude", "Longitude" });

            return columnsToInclude;
        }
    }
}
