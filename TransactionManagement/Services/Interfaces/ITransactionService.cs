using TransactionManagement.Models.Responses;

namespace TransactionManagement.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<List<TransactionResponse>> SaveAsync(IFormFile file, CancellationToken cancellationToken);
        Task<List<TransactionResponse>> GetTransactionsForDateAsync(int year, string month, CancellationToken cancellationToken);
    }
}
