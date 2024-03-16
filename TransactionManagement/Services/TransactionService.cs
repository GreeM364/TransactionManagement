﻿using TransactionManagement.DatabaseManager.Interfaces;
using TransactionManagement.Entities;
using TransactionManagement.Helpers;
using TransactionManagement.Models.Requests;
using TransactionManagement.Models.Responses;
using TransactionManagement.Services.Interfaces;

namespace TransactionManagement.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionManager _transactionManager;

        public TransactionService(ITransactionManager transactionManager)
        {
            _transactionManager = transactionManager;
        }

        public async Task<List<TransactionResponse>> SaveAsync(IFormFile csvFile, CancellationToken cancellationToken)
        {
            var transactionsRequestScv = CSVParser<TransactionRequestScv>.ParseCSV(csvFile);

            transactionsRequestScv = RemoveDuplicatesAndKeepLatest(transactionsRequestScv);

            var transactionsToSave = Mapper.TransactionRequestScvToTransaction(transactionsRequestScv);

            var (insertedTransactions, updatedTransactions) = 
                await SplitTransactionsAsync(transactionsToSave, cancellationToken);

            await _transactionManager.InsertTransactionsAsync(insertedTransactions, cancellationToken);
            await _transactionManager.UpdateTransactionsAsync(updatedTransactions, cancellationToken);

            var transactionsResponse = Mapper.TransactionToTransactionResponse(insertedTransactions.Concat(updatedTransactions).ToList());

            return transactionsResponse;
        }

        private List<TransactionRequestScv> RemoveDuplicatesAndKeepLatest(List<TransactionRequestScv> transactions)
        {
            var uniqueTransactions = transactions
                .GroupBy(t => t.transaction_id)
                .Select(group => group.Last())
                .ToList();

            return uniqueTransactions;
        }

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
    }
}
