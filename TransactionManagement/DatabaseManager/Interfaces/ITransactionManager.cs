﻿using TransactionManagement.Entities;

namespace TransactionManagement.DatabaseManager.Interfaces
{
    public interface ITransactionManager
    {
        Task<List<Transaction>> GetAllTransactionsAsync(CancellationToken cancellationToken);
        Task InsertTransactionsAsync(List<Transaction> transactions, CancellationToken cancellationToken);
        Task UpdateTransactionsAsync(List<Transaction> transactions, CancellationToken cancellationToken);
        Task<List<string>> GetExistingTransactionIdsAsync(List<string> transactionIds,CancellationToken cancellationToken);
    }
}
