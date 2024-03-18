using Dapper;
using TransactionManagement.DatabaseManager.Interfaces;
using TransactionManagement.Entities;
using TransactionManagement.Persistence;

namespace TransactionManagement.DatabaseManager
{
    public class TransactionManager : ITransactionManager
    {
        private readonly SqlConnectionFactory _sqlConnectionFactory;

        public TransactionManager(SqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

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

        public async Task<List<Transaction>> GetTransactionsForClientTimeZoneAsync(int year, int? month, CancellationToken cancellationToken)
        {
            await using var connection = _sqlConnectionFactory.Create();
            await connection.OpenAsync(cancellationToken);

            var sql = @"SELECT TransactionId, Name, Email, Amount,
                               FORMAT(TransactionDate AT TIME ZONE 'UTC' AT TIME ZONE Timezone, 'yyyy-MM-dd HH:mm:ss.ff') AS TransactionDate,
                               Timezone, Latitude, Longitude 
                          FROM Transactions 
                         WHERE YEAR(TransactionDate) = @Year";

            object queryParams;

            if (month is not null)
            {
                sql += " AND MONTH(TransactionDate) = @Month";
                queryParams = new { Year = year, Month = month };
            }
            else
            {
                queryParams = new { Year = year };
            }

            var transactions = await connection.QueryAsync<Transaction>(sql, queryParams);

            return transactions.ToList();
        }

        public async Task<List<Transaction>> GetTransactionsForCurrentTimeZoneAsync(string timeZone, int year, int? month, CancellationToken cancellationToken)
        {
            await using var connection = _sqlConnectionFactory.Create();
            await connection.OpenAsync(cancellationToken);

            var sql = @"SELECT TransactionId, Name, Email, Amount,
                               FORMAT(TransactionDate AT TIME ZONE 'UTC' AT TIME ZONE Timezone, 'yyyy-MM-dd HH:mm:ss.ff') AS TransactionDate,
                               Timezone, Latitude, Longitude 
                          FROM Transactions 
                         WHERE YEAR(TransactionDate) = @Year AND Timezone = @TimeZone";

            object queryParams;

            if (month is not null)
            {
                sql += " AND MONTH(TransactionDate) = @Month";
                queryParams = new { Year = year, Month = month, TimeZone = timeZone };
            }
            else
            {
                queryParams = new { Year = year, TimeZone = timeZone };
            }

            var transactions = await connection.QueryAsync<Transaction>(sql, queryParams);

            return transactions.ToList();
        }

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