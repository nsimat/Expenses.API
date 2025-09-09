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
    private readonly ILogger<TransactionsController> _logger;
    private readonly ITransactionsService _transactionsService;

    public TransactionsController(ITransactionsService transactionsService, ILogger<TransactionsController> logger)
    {
        _transactionsService = transactionsService ?? throw new ArgumentNullException(nameof(transactionsService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet("All")]
    [EndpointSummary("Get all transactions")]
    [EndpointDescription("Fetches all transactions from the database.")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Transaction>))]
    public IActionResult GetAllTransactions()
    {
        _logger.LogInformation("Fetching all transactions...");
        try
        {
            var transactions = _transactionsService.GetAll();
            if (!transactions.Any())
            {
                _logger.LogWarning("No transactions found!");
                return NoContent();
            }
            return Ok(_transactionsService.GetAll());
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An error occurred while fetching transactions...");

            return Problem(
                detail: "An error occurred while processing your request. Please try again later.",
                instance: HttpContext.TraceIdentifier,
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Internal Server Error"
           );
        }
    }

    [HttpGet("Details/{id}")]
    [EndpointSummary("Get transaction by ID from the database")]
    [EndpointDescription("Fetches a specific transaction by its ID.")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Transaction))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public IActionResult GetTransactionById(int id)
    {
        _logger.LogInformation("Fetching transaction with ID: {Id}...", id);
        try
        {
            var transaction = _transactionsService.GetById(id);
            if (transaction == null)
            {
                _logger.LogWarning("Transaction with ID: {Id} not found!", id);
                return NotFound("Transaction not found!");
            }
            return Ok(transaction);
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
    [EndpointSummary("Create a new transaction")]
    [EndpointDescription("Creates a new transaction in the database.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public IActionResult CreateTransaction([FromBody] PostTransactionDto payload)
    {
        _logger.LogInformation("Creating a new transaction...");
        try
        {
            var createdTransaction = _transactionsService.Add(payload);
            if (createdTransaction == null)
            {
                _logger.LogWarning("Failed to create transaction!");
                return BadRequest("Failed to create transaction!");
            }
            return Ok(createdTransaction);
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
    
    [HttpPut("Update/{id}")]
    [EndpointSummary("Update an existing transaction")]
    [EndpointDescription("Updates an existing transaction in the database.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public IActionResult UpdateTransaction(int id, [FromBody] PutTransactionDto payload)
    {
        _logger.LogInformation("Updating transaction with ID: {Id}...", id);
        try
        {
          var existingTransaction = _transactionsService.Update(id, payload);
          if (existingTransaction == null)
          {
              _logger.LogWarning("Transaction with ID: {Id} not found!", id);
              return NotFound("Transaction not found!");
          }
          return Ok("Transaction updated!");
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

    [HttpDelete("Delete/{id}")]
    [EndpointSummary("Delete a transaction")]
    [EndpointDescription("Deletes a transaction from the database.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public IActionResult DeleteTransaction(int id)
    {
        _logger.LogInformation("Deleting transaction with ID: {Id}...", id);
        try
        {
            var isDeleted = _transactionsService.Delete(id);
            if (!isDeleted)
            {
                _logger.LogWarning("Transaction with ID: {Id} not found!", id);
                return NotFound("Transaction not found!");
            }
            return Ok("Transaction deleted!");
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
}