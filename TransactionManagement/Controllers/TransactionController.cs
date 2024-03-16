using Microsoft.AspNetCore.Mvc;
using TransactionManagement.Models.Responses;
using TransactionManagement.Services.Interfaces;

namespace TransactionManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }


        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadCsv(IFormFile file, CancellationToken cancellationToken = default)
        {
            var downloadedTransactionData = await _transactionService.SaveAsync(file, cancellationToken);

            return StatusCode(StatusCodes.Status201Created, downloadedTransactionData);
        }
    }
}
