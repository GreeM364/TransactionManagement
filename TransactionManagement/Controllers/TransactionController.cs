using Microsoft.AspNetCore.Mvc;
using TransactionManagement.Models.Requests;
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
        [ProducesResponseType(typeof(List<TransactionResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadCsv(IFormFile file, CancellationToken cancellationToken = default)
        {
            var downloadedTransactionData = await _transactionService.SaveAsync(file, cancellationToken);

            return StatusCode(StatusCodes.Status201Created, downloadedTransactionData);
        }

        [HttpGet("GetAll/ClientTimeZone/{year}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<TransactionResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetTransactionsForClientTimeZone(int year, string? month = null, CancellationToken cancellationToken = default)
        { 
            var transactions = await _transactionService.GetTransactionsForClientTimeZoneAsync(year, month, cancellationToken);

            return Ok(transactions);
        }

        [HttpGet("GetAll/CurrentTimeZone/{year}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<TransactionResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetTransactionsForCurrentUserTimeZone(int year, string? month = null, CancellationToken cancellationToken = default)
        {
            string clientIp = HttpContext.Connection.RemoteIpAddress!.ToString();

            var transactions = await _transactionService.GetTransactionsForCurrentTimeZoneAsync(clientIp, year, month, cancellationToken);

            return Ok(transactions);
        }

        [HttpPost("export/excel")]
        [Consumes("application/json")]
        [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ExportToExcel([FromBody] ExportTransactionsRequest request, CancellationToken cancellationToken = default)
        {
            var excelBytes = await _transactionService.ExportAsync(request, cancellationToken);

            var stream = new MemoryStream(excelBytes);

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "transactions.xlsx");
        }

    }
}
