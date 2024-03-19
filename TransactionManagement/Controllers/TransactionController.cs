using Microsoft.AspNetCore.Mvc;
using TransactionManagement.Models.Requests;
using TransactionManagement.Models.Responses;
using TransactionManagement.Services.Interfaces;

namespace TransactionManagement.Controllers
{
    /// <summary>
    /// Controller for managing transactions.
    /// </summary>
    /// <remarks>
    /// Controller for managing transactions, including uploading CSV files, retrieving transactions for different time zones,
    /// and exporting transactions to Excel format.
    /// </remarks> 
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        /// <summary>
        /// Uploads a CSV file to process transactions.
        /// </summary>
        /// <remarks>
        /// This method accepts a CSV file containing transaction data. It then processes the file to extract
        /// transaction information and saves the transactions. If successful, it returns the processed transactions.
        /// </remarks>
        /// <param name="file">The CSV file containing transaction data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the processed transactions as a list of <see cref="TransactionResponse"/> objects.
        /// </returns>
        /// <response code="201">Returns the processed transactions.</response>
        /// <response code="400">Returned when the request is invalid.</response>
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

        /// <summary>
        /// Retrieves transactions for client's time zone specified year and month.
        /// </summary>
        /// <remarks>
        /// This method retrieves transactions based on the client's time zone and year. If a specific month is provided,
        /// transactions for that month are retrieved; otherwise, transactions for the entire year are retrieved.
        /// </remarks>
        /// <param name="year">The year for which transactions are requested.</param>
        /// <param name="month">The month for which transactions are requested (optional).</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the retrieved transactions as a list of <see cref="TransactionResponse"/> objects.
        /// </returns>
        /// <response code="200">Returns the list of transactions</response>
        /// <response code="400">Returned when the request is invalid</response>
        [HttpGet("getAll/clientTimeZone/{year}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<TransactionResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetTransactionsForClientTimeZone(int year, string? month = null, CancellationToken cancellationToken = default)
        { 
            var transactions = await _transactionService.GetTransactionsForClientTimeZoneAsync(year, month, cancellationToken);

            return Ok(transactions);
        }

        /// <summary>
        /// Retrieves transactions for the current user's time zone specified year and month.
        /// </summary>
        /// <remarks>
        /// This method retrieves transactions for the current user's time zone based on their IP address,
        /// for the specified year and month(if provided). If successful, it returns the retrieved transactions.
        /// </remarks>
        /// <param name="year">The year for which transactions are requested.</param>
        /// <param name="month">The month for which transactions are requested (optional).</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the retrieved transactions as a list of <see cref="TransactionResponse"/> objects.
        /// </returns>
        /// <response code="200">Returns the list of transactions.</response>
        /// <response code="400">Returned when the request is invalid.</response>
        [HttpGet("getAll/currentTimeZone/{year}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<TransactionResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetTransactionsForCurrentUserTimeZone(int year, string? month = null, CancellationToken cancellationToken = default)
        {
            string clientIp = HttpContext.Connection.RemoteIpAddress!.ToString();

            var transactions = await _transactionService.GetTransactionsForCurrentTimeZoneAsync(clientIp, year, month, cancellationToken);

            return Ok(transactions);
        }

        /// <summary>
        /// Exports transactions to an Excel file.
        /// </summary>
        /// <remarks>
        /// This method exports transactions to an Excel file based on the provided request parameters relative to time in UTC format.
        /// The request must include parameters specifying the transactions to be included in the export, as well as which fields to include.
        /// If successful, it returns the Excel file containing the exported transactions.
        /// </remarks>
        /// <param name="request">The request containing parameters for exporting transactions.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The Excel file containing the exported transactions.</returns>
        /// <response code="200">Returns the Excel file containing the exported transactions.</response>
        /// <response code="400">Returned when the request is invalid.</response>
        /// <response code="404">Returned when no transactions are found for the given parameters.</response>
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
