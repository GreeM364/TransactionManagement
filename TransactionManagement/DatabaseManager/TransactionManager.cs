using Dapper;
using TransactionManagement.DatabaseManager.Interfaces;
using TransactionManagement.Entities;
using TransactionManagement.Services;

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

            string sql = @"INSERT INTO Transactions (TransactionId, Name, Email, Amount, TransactionDate, Timezone, Latitude, Longitude)
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

        public async Task<List<string>> GetExistingTransactionIdsAsync(List<string> transactionIds, CancellationToken cancellationToken)
        {
            await using var connection = _sqlConnectionFactory.Create();
            await connection.OpenAsync(cancellationToken);

            var sql = @"SELECT TransactionId 
                          FROM Transactions 
                         WHERE TransactionId IN @TransactionIds";

            var existingTransactionIds = await connection.QueryAsync<string>(sql, new { TransactionIds = transactionIds });

            return existingTransactionIds.ToList();
        }
    }
}
