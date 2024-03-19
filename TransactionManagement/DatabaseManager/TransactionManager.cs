using Dapper;
using TransactionManagement.DatabaseManager.Interfaces;
using TransactionManagement.Entities;
using TransactionManagement.Persistence;

namespace TransactionManagement.DatabaseManager
{
    /// <summary>
    /// Manages transactions in the database.
    /// </summary>
    public class TransactionManager : ITransactionManager
    {
        private readonly SqlConnectionFactory _sqlConnectionFactory;

        public TransactionManager(SqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }


        /// <summary>
        /// Inserts transactions asynchronously into the database.
        /// </summary>
        /// <param name="transactions">The list of transactions to insert.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task InsertTransactionsAsync(List<Transaction> transactions, CancellationToken cancellationToken)
        {
            await using var connection = _sqlConnectionFactory.Create();
            await connection.OpenAsync(cancellationToken);

            string sql = @"INSERT 
                             INTO Transactions (TransactionId, Name, Email, Amount, TransactionDate, Timezone, Latitude, Longitude)
                           VALUES (@TransactionId, @Name, @Email, @Amount, @TransactionDate, @Timezone, @Latitude, @Longitude)";

            foreach (var transaction in transactions)
            {
                await connection.ExecuteAsync(sql, transaction);
            }
        }

        /// <summary>
        /// Updates transactions asynchronously in the database.
        /// </summary>
        /// <param name="transactions">The list of transactions to update.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UpdateTransactionsAsync(List<Transaction> transactions, CancellationToken cancellationToken)
        {
            await using var connection = _sqlConnectionFactory.Create();
            await connection.OpenAsync(cancellationToken);

            string sql = @"UPDATE Transactions 
                              SET Name = @Name, 
                                  Email = @Email, 
                                  Amount = @Amount, 
                                  TransactionDate = @TransactionDate, 
                                  Timezone = @Timezone, 
                                  Latitude = @Latitude, 
                                  Longitude = @Longitude
                            WHERE TransactionId = @TransactionId";

            foreach (var transaction in transactions)
            {
                await connection.ExecuteAsync(sql, transaction);
            }
        }

        /// <summary>
        /// Retrieves existing transaction IDs asynchronously from the database.
        /// </summary>
        /// <param name="transactionIds">The list of transaction IDs to check for existence.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of existing transaction IDs.</returns>
        public async Task<List<string>> GetExistingTransactionIdsAsync(List<string> transactionIds,
            CancellationToken cancellationToken)
        {
            await using var connection = _sqlConnectionFactory.Create();
            await connection.OpenAsync(cancellationToken);

            var sql = @"SELECT TransactionId 
                          FROM Transactions 
                         WHERE TransactionId IN @TransactionIds";

            var existingTransactionIds =
                await connection.QueryAsync<string>(sql, new { TransactionIds = transactionIds });

            return existingTransactionIds.ToList();
        }

        /// <summary>
        /// Retrieves transactions for the client time zone and specified year, month(if provided) asynchronously from the database.
        /// </summary>
        /// <param name="year">The year for which transactions are requested.</param>
        /// <param name="month">The month for which transactions are requested (optional).</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of transactions.</returns>
        public async Task<List<Transaction>> GetTransactionsForClientTimeZoneAsync(int year, int? month, CancellationToken cancellationToken)
        {
            await using var connection = _sqlConnectionFactory.Create();
            await connection.OpenAsync(cancellationToken);

            var sql = @"SELECT TransactionId, Name, Email, Amount,
                               FORMAT(TransactionDate AT TIME ZONE 'UTC' AT TIME ZONE Timezone, 'yyyy-MM-dd HH:mm:ss.ff') AS TransactionDate,
                               Timezone, Latitude, Longitude 
                          FROM Transactions 
                         WHERE YEAR(TransactionDate AT TIME ZONE 'UTC' AT TIME ZONE Timezone) = @Year";

            object queryParams;

            if (month is not null)
            {
                sql += " AND MONTH(TransactionDate AT TIME ZONE 'UTC' AT TIME ZONE Timezone) = @Month";
                queryParams = new { Year = year, Month = month };
            }
            else
            {
                queryParams = new { Year = year };
            }

            var transactions = await connection.QueryAsync<Transaction>(sql, queryParams);

            return transactions.ToList();
        }

        /// <summary>
        /// Retrieves transactions for the current time zone, year, and optionally month asynchronously from the database.
        /// </summary>
        /// <param name="timeZone">The time zone for which transactions are requested.</param>
        /// <param name="year">The year for which transactions are requested.</param>
        /// <param name="month">The month for which transactions are requested (optional).</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of transactions.</returns>
        public async Task<List<Transaction>> GetTransactionsForCurrentTimeZoneAsync(string timeZone, int year, int? month, CancellationToken cancellationToken)
        {
            await using var connection = _sqlConnectionFactory.Create();
            await connection.OpenAsync(cancellationToken);

            var sql = @"SELECT TransactionId, Name, Email, Amount,
                               FORMAT(TransactionDate AT TIME ZONE 'UTC' AT TIME ZONE Timezone, 'yyyy-MM-dd HH:mm:ss.ff') AS TransactionDate,
                               Timezone, Latitude, Longitude 
                          FROM Transactions 
                         WHERE Timezone = @TimeZone
                           AND YEAR(TransactionDate AT TIME ZONE 'UTC' AT TIME ZONE Timezone) = @Year";

            object queryParams;

            if (month is not null)
            {
                sql += " AND MONTH(TransactionDate AT TIME ZONE 'UTC' AT TIME ZONE Timezone) = @Month";
                queryParams = new { Year = year, Month = month, TimeZone = timeZone };
            }
            else
            {
                queryParams = new { Year = year, TimeZone = timeZone };
            }


            var transactions = await connection.QueryAsync<Transaction>(sql, queryParams);

            return transactions.ToList();
        }

        /// <summary>
        /// Retrieves transactions within the specified date range and includes specified columns asynchronously from the database.
        /// </summary>
        /// <param name="startDate">The start date of the range.</param>
        /// <param name="endDate">The end date of the range.</param>
        /// <param name="columnsToInclude">The list of column names to include in the result.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of transactions with specified columns.</returns>
        public async Task<List<Transaction>> GetTransactionsByDateAsync(DateTime startDate, DateTime endDate, List<string> columnsToInclude,
            CancellationToken cancellationToken)
        {
            await using var connection = _sqlConnectionFactory.Create();
            await connection.OpenAsync(cancellationToken);

            var includedColumns = string.Join(", ", columnsToInclude.Select(c => $"[{c}]"));

            var sql = $@"SELECT {includedColumns}
                           FROM Transactions
                          WHERE TransactionDate >= @StartDate AND TransactionDate <= @EndDate";

            var queryParams = new { StartDate = startDate, EndDate = endDate };

            var transactions = await connection.QueryAsync<Transaction>(sql, queryParams);

            return transactions.ToList();
        }
    }
}