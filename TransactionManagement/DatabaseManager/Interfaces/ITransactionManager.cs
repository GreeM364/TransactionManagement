using TransactionManagement.Entities;

namespace TransactionManagement.DatabaseManager.Interfaces
{
    public interface ITransactionManager
    {
        Task InsertTransactionsAsync(List<Transaction> transactions, CancellationToken cancellationToken);
        Task UpdateTransactionsAsync(List<Transaction> transactions, CancellationToken cancellationToken);
        Task<List<string>> GetExistingTransactionIdsAsync(List<string> transactionIds,CancellationToken cancellationToken);
    }
}
