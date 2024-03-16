using TransactionManagement.Models.Responses;

namespace TransactionManagement.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<List<TransactionResponse>> SaveAsync(IFormFile file, CancellationToken cancellationToken);
    }
}
