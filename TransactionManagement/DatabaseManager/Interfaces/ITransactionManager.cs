using TransactionManagement.Entities;

namespace TransactionManagement.DatabaseManager.Interfaces
{
    /// <summary>
    /// Defines methods to manage transactions in the database.
    /// </summary>
    public interface ITransactionManager
    {
        /// <summary>
        /// Retrieves transactions for the client time zone and specified year, month(if provided) asynchronously from the database.
        /// </summary>
        /// <param name="year">The year for which transactions are requested.</param>
        /// <param name="month">The month for which transactions are requested (optional).</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of transactions.</returns>
        Task<List<Transaction>> GetTransactionsForClientTimeZoneAsync(int year, int? month, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves transactions for the current time zone, year, and optionally month asynchronously from the database.
        /// </summary>
        /// <param name="timeZone">The time zone for which transactions are requested.</param>
        /// <param name="year">The year for which transactions are requested.</param>
        /// <param name="month">The month for which transactions are requested (optional).</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of transactions.</returns>
        Task<List<Transaction>> GetTransactionsForCurrentTimeZoneAsync(string timeZone, int year, int? month, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves transactions within the specified date range and includes specified columns asynchronously from the database.
        /// </summary>
        /// <param name="startDate">The start date of the range.</param>
        /// <param name="endDate">The end date of the range.</param>
        /// <param name="columnsToInclude">The list of column names to include in the result.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of transactions with specified columns.</returns>
        Task<List<Transaction>> GetTransactionsByDateAsync(DateTime startDate, DateTime endDate, List<string> columnsToInclude, CancellationToken cancellationToken);

        /// <summary>
        /// Inserts transactions asynchronously into the database.
        /// </summary>
        /// <param name="transactions">The list of transactions to insert.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task InsertTransactionsAsync(List<Transaction> transactions, CancellationToken cancellationToken);

        /// <summary>
        /// Updates transactions asynchronously in the database.
        /// </summary>
        /// <param name="transactions">The list of transactions to update.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateTransactionsAsync(List<Transaction> transactions, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves existing transaction IDs asynchronously from the database.
        /// </summary>
        /// <param name="transactionIds">The list of transaction IDs to check for existence.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of existing transaction IDs.</returns>
        Task<List<string>> GetExistingTransactionIdsAsync(List<string> transactionIds, CancellationToken cancellationToken);
    }
}
