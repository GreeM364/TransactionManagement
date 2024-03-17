using System.Globalization;
using TransactionManagement.DatabaseManager.Interfaces;
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
        private readonly IIpInfoService _ipInfoService;

        public TransactionService(ITransactionManager transactionManager, IIpInfoService infoService)
        {
            _transactionManager = transactionManager;
            _ipInfoService = infoService;
        }

        public async Task<List<TransactionResponse>> SaveAsync(IFormFile csvFile, CancellationToken cancellationToken)
        {
            var transactionsRequestScv = CSVParser<TransactionRequestCsv>.ParseCSV(csvFile);

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

        public async Task<List<TransactionResponse>> GetTransactionsForClientTimeZoneAsync(int year, string? month, CancellationToken cancellationToken)
        {
            var monthNumber = ParseMonth(month);

            var transactions = await _transactionManager.GetTransactionsForClientTimeZoneAsync(year, monthNumber, cancellationToken);

            if (!transactions.Any())
                return Enumerable.Empty<TransactionResponse>().ToList();

            var transactionsResponse = Mapper.TransactionToTransactionResponse(transactions);

            return transactionsResponse;
        }

        public async Task<List<TransactionResponse>> GetTransactionsForCurrentTimeZoneAsync(string clientIp, int year, string? month, CancellationToken cancellationToken)
        {
            var monthNumber = ParseMonth(month);

            var currentTimeZone = await _ipInfoService.GetCurrentTimeZoneAsync(clientIp);
            var standardCurrentTimeZone = TimeZoneHelper.ConvertIanaToWindows(currentTimeZone);

            var transactions = await _transactionManager.GetTransactionsForCurrentTimeZoneAsync(standardCurrentTimeZone, year, monthNumber, cancellationToken);

            if (!transactions.Any())
                return Enumerable.Empty<TransactionResponse>().ToList();

            var transactionsResponse = Mapper.TransactionToTransactionResponse(transactions);

            return transactionsResponse;
        }

        private List<TransactionRequestCsv> RemoveDuplicatesAndKeepLatest(List<TransactionRequestCsv> transactions)
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

        private int? ParseMonth(string? month)
        {
            if (month is not null)
            {
                if (!DateTime.TryParseExact(month, "MMMM", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedMonth))
                {
                    throw new ArgumentException("Invalid month format", nameof(month));
                }

                return parsedMonth.Month;
            }

            return null;
        }
    }
}
