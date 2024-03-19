using TransactionManagement.Exceptions;
using TransactionManagement.Models.Requests;
using TransactionManagement.Models.Responses;

namespace TransactionManagement.Services.Interfaces
{
    /// <summary>
    /// Defines methods for managing transactions.
    /// </summary>
    public interface ITransactionService
    {
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
        Task<List<TransactionResponse>> SaveAsync(IFormFile csvFile, CancellationToken cancellationToken);

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
        Task<byte[]> ExportAsync(ExportTransactionsRequest request, CancellationToken cancellationToken);

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
        Task<List<TransactionResponse>> GetTransactionsForClientTimeZoneAsync(int year, string? month, CancellationToken cancellationToken);

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
        Task<List<TransactionResponse>> GetTransactionsForCurrentTimeZoneAsync(string clientIp, int year, string? month, CancellationToken cancellationToken);
    }
}
