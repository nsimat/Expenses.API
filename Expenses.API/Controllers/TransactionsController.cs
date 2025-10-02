using Expenses.API.Data;
using Expenses.API.Data.Services;
using Expenses.API.Dtos;
using Expenses.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Expenses.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TransactionsController : ControllerBase
{
    #region Fields
    private readonly ILogger<TransactionsController> _logger;
    private readonly ITransactionsService _transactionsService;
    #endregion

    #region Constructor
    /// <summary>
    /// Private constructor to initialize the TransactionsController with transaction service and logger.
    /// </summary>
    /// <param name="transactionsService"></param>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public TransactionsController(ITransactionsService transactionsService, ILogger<TransactionsController> logger)
    {
        _transactionsService = transactionsService ?? throw new ArgumentNullException(nameof(transactionsService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    #endregion

    #region Endpoints for Transactions
    
    [HttpGet("All")]
    [EndpointSummary("Get all transactions.")]
    [EndpointDescription("Fetches all transactions from the database.")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Transaction>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> GetAllTransactions()
    {
        _logger.LogInformation("Fetching all transactions...");
        try
        {
            var transactions = await _transactionsService.GetAllAsync();
            if (!transactions.Any())
            {
                _logger.LogWarning("No transactions found!");
                return NoContent();
            }

            return Ok(transactions);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An error occurred while retrieving all transactions!");

            return Problem(
                detail: "An error occurred while processing your request. Please try again later.",
                instance: HttpContext.TraceIdentifier,
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Internal Server Error"
            );
        }
    }

    [HttpGet("Details/{id:int}")]
    [EndpointSummary("Get transaction by ID from the database.")]
    [EndpointDescription("Fetches a specific transaction by its ID.")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Transaction))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> GetTransactionById(int id)
    {
        _logger.LogInformation("Fetching transaction with ID: {Id}...", id);
        try
        {
            var transaction = await _transactionsService.GetByIdAsync(id);
            if (transaction != null)
            {
                _logger.LogInformation("Transaction with ID: {Id} found.", id);
                return Ok(transaction);
            }

            _logger.LogWarning("Transaction with ID: {Id} not found!", id);
            return NotFound("Transaction not found!");
        }
        catch (Exception e)
        {
            _logger.LogError("An error occurred while fetching the transaction with ID: {Id}.", id);
            return Problem(
                detail: "An error occurred while processing your request. Please try again later.",
                instance: HttpContext.TraceIdentifier,
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Internal Server Error"
            );
        }
    }

    [HttpPost("Create")]
    [EndpointSummary("Create a new transaction.")]
    [EndpointDescription("Creates a new transaction in the database.")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Transaction))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> CreateTransaction([FromBody] TransactionForCreationDto payload)
    {
        _logger.LogInformation("Creating a new transaction...");
        try
        {
            var createdTransaction = await _transactionsService.AddAsync(payload);
            if (createdTransaction != null)
                return Ok(createdTransaction);

            _logger.LogWarning("Failed to create transaction!");
            return BadRequest("TransactionForCreationDto object is null!");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An error occurred while creating the transaction!!");
            return Problem(
                detail: "An error occurred while processing your request. Please try again later.",
                instance: HttpContext.TraceIdentifier,
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Internal Server Error"
            );
        }
    }

    [HttpPut("Update/{id:int}")]
    [EndpointSummary("Update an existing transaction.")]
    [EndpointDescription("Updates an existing transaction in the database.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> UpdateTransaction(int id, [FromBody] TransactionForUpdateDto payload)
    {
        _logger.LogInformation("Updating transaction with ID: {Id}...", id);
        try
        {
            var existingTransaction = await _transactionsService.UpdateAsync(id, payload);

            if (existingTransaction != null)
            {
                _logger.LogInformation("Transaction with ID: {Id} updated successfully.", id);
                return Ok(existingTransaction);
            }

            _logger.LogWarning("Transaction with ID: {Id} not found!", id);
            return BadRequest("TransactionForUpdateDto object is null or invalid!");
        }
        catch (Exception exception)
        {
            _logger.LogError("An error occurred while updating the transaction with ID: {Id}.", id);
            return Problem(
                detail: "An error occurred while processing your request. Please try again later.",
                instance: HttpContext.TraceIdentifier,
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Internal Server Error"
            );
        }
    }

    [HttpDelete("Delete/{id:int}")]
    [EndpointSummary("Delete a transaction.")]
    [EndpointDescription("Deletes a transaction from the database.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> DeleteTransaction(int id)
    {
        _logger.LogInformation("Deleting transaction with ID: {Id}...", id);
        try
        {
            var isDeleted = await _transactionsService.Delete(id);
            if (isDeleted)
            {
                _logger.LogInformation("Transaction with ID: {Id} deleted successfully.", id);
                return NoContent();
            }

            _logger.LogWarning("Transaction with ID: {Id} not found!", id);
            return NotFound("Transaction not found!");
        }
        catch (Exception e)
        {
            _logger.LogError("An error occurred while deleting the transaction with ID: {Id}!", id);
            return Problem(
                detail: "An error occurred while processing your request. Please try again later.",
                instance: HttpContext.TraceIdentifier,
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Internal Server Error"
            );
        }
    }
    #endregion
}