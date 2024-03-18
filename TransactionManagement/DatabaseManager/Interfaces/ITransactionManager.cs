using TransactionManagement.Entities;

namespace TransactionManagement.DatabaseManager.Interfaces
{
    public interface ITransactionManager
    {
        Task<List<Transaction>> GetTransactionsForClientTimeZoneAsync(int year, int? month, CancellationToken cancellationToken);
        Task<List<Transaction>> GetTransactionsForCurrentTimeZoneAsync(string timeZone, int year, int? month, CancellationToken cancellationToken);
        Task<List<Transaction>> GetTransactionsByDateAsync(DateTime startDate, DateTime endDate, List<string> columnsToInclude, CancellationToken cancellationToken);
        Task InsertTransactionsAsync(List<Transaction> transactions, CancellationToken cancellationToken);
        Task UpdateTransactionsAsync(List<Transaction> transactions, CancellationToken cancellationToken);
        Task<List<string>> GetExistingTransactionIdsAsync(List<string> transactionIds,CancellationToken cancellationToken);
    }
}
