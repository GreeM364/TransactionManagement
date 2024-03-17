using TransactionManagement.Models.Responses;

namespace TransactionManagement.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<List<TransactionResponse>> SaveAsync(IFormFile file, CancellationToken cancellationToken);
        Task<List<TransactionResponse>> GetTransactionsForClientTimeZoneAsync(int year, string? month, CancellationToken cancellationToken);
        Task<List<TransactionResponse>> GetTransactionsForCurrentTimeZoneAsync(string clientIp, int year, string? month, CancellationToken cancellationToken);
    }
}
