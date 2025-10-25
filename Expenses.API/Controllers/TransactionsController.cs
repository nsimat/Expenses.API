using Expenses.API.Data.Services;
using Expenses.API.Dtos;
using Expenses.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Expenses.API.Controllers;

/// <summary>
/// Controller for managing transactions CRUD operations.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class TransactionsController : ControllerBase
{
    #region Fields
    // readonly Fields to ensure immutability after construction.
    private readonly ILogger<TransactionsController> _logger;
    private readonly ITransactionsService _transactionsService;
    #endregion

    #region Constructor
    /// <summary>
    /// Private constructor to initialize the TransactionsController with transaction service and logger.
    /// </summary>
    /// <param name="transactionsService">Service to handle business logic related to transactions.</param>
    /// <param name="logger">An ILogger property for capturing valuable information during runtime.</param>
    /// <exception cref="ArgumentNullException">Throws argument exception if injected objects are null.</exception>
    public TransactionsController(ITransactionsService transactionsService, ILogger<TransactionsController> logger)
    {
        _transactionsService = transactionsService ?? throw new ArgumentNullException(nameof(transactionsService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    #endregion

    #region Endpoints for Transactions
    // Actions should always be clean and simple, delegate complex logic to services.
    // Each action should handle a single responsibility. Their main job is to handle HTTP requests,
    // validate models, catch errors, and return responses.
    
    /// <summary>
    /// List all transactions.
    /// </summary>
    /// <returns>List of transactions from database</returns>
    /// <response code="200">Successfully returns all transactions found in the database.</response>
    /// <response code="204">If no transaction found in the database.</response>
    /// <response code="500">If an error occurred while processing the request.</response>
    /// <exception cref="Exception">Throws exception if an error occurs while retrieving transactions.</exception>
    [HttpGet("All")]
    [EndpointSummary("Obtain a list of all transactions.")]
    [EndpointDescription("Fetches all transactions from the database.")]
    [EndpointName("All Transactions")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Transaction>))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
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

    /// <summary>
    /// Retrieves a specific transaction by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the transaction to retrieve.</param>
    /// <returns>The requested transaction if found, or NotFound if not available.</returns>
    /// <response code="200">Transaction found and returned successfully.</response>
    /// <response code="404">Transaction with specified ID not found.</response>
    /// <response code="500">An error occurred while processing the request.</response>
    /// <exception cref="Exception">Throws exception if an error occurs while retrieving the transaction.</exception>
    [HttpGet("Details/{id:int}")]
    [EndpointSummary("Obtain transaction by ID from the database.")]
    [EndpointDescription("Fetches a specific transaction by its ID.")]
    [EndpointName("Transaction Details")]
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
        catch (Exception exception)
        {
            _logger.LogError(exception, "An error occurred while fetching the transaction with ID: {Id}.", id);
            return Problem(
                detail: "An error occurred while processing your request. Please try again later.",
                instance: HttpContext.TraceIdentifier,
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Internal Server Error"
            );
        }
    }

    /// <summary>
    /// Creates a new transaction.
    /// </summary>
    /// <param name="payload">A DTO that represents parts of transaction to be created.</param>
    /// <returns>The created transaction if creation is successful. Otherwise, an error is returned.</returns>
    /// <response code="201">Transaction created successfully.</response>
    /// <response code="400">The provided payload is null or invalid.</response>
    /// <response code="500">An error occurred while processing the request.</response>
    /// <exception cref="ArgumentNullException">Thrown when the provided payload is null.</exception>
    [HttpPost("Create")]
    [EndpointSummary("Create a new transaction.")]
    [EndpointDescription("Creates a new transaction in the database.")]
    [EndpointName("Create Transaction")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Transaction))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> CreateTransaction([FromBody] TransactionForCreationDto payload)
    {
        _logger.LogInformation("Creating a new transaction...");
        try
        {
            if(!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for creating transaction!");
                ModelState.AddModelError("message", "The provided transaction is invalid.");
                return BadRequest(ModelState);
            }
            
            var createdTransaction = await _transactionsService.AddAsync(payload);
            if (createdTransaction != null)
                return Ok(createdTransaction);

            _logger.LogWarning("Failed to create transaction. TransactionForCreationDto object is null!");
            return BadRequest("Transaction object is null!");
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

    /// <summary>
    /// Modifies the content of existing transaction.
    /// </summary>
    /// <param name="id">the unique identifier of transaction to modify.</param>
    /// <param name="payload">A DTO object representing the transaction to modify</param>
    /// <returns>Transaction updated from database.</returns>
    /// <response code="200">Transaction updated successfully.</response>
    /// <response code="400">The provided payload is null or invalid.</response>
    /// <response code="404">Transaction with specified ID not found.</response>
    /// <response code="500">An error occurred while processing the request.</response>
    /// <exception cref="ArgumentNullException">Thrown when the provided payload is null.</exception>
    [HttpPut("Update/{id:int}")]
    [EndpointSummary("Update an existing transaction.")]
    [EndpointDescription("Modifies an existing transaction in the database.")]
    [EndpointName("Update Transaction")]
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
            _logger.LogError(exception, "An error occurred while updating the transaction with ID: {Id}.", id);
            return Problem(
                detail: "An error occurred while processing your request. Please try again later.",
                instance: HttpContext.TraceIdentifier,
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Internal Server Error"
            );
        }
    }

    /// <summary>
    /// Deletes a transaction by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of transaction to delete.</param>
    /// <returns>No content if deletion is successful. Otherwise, an error is returned.</returns>
    /// <response code="204">Transaction deleted successfully.</response>
    /// <response code="404">Transaction with specified ID not found.</response>
    /// <response code="403">Forbidden. User does not have permission to delete the transaction.</response>
    /// <response code="500">An error occurred while processing the request.</response>
    /// <exception cref="Exception">Throws exception if an error occurs while deleting the transaction.</exception>
    [HttpDelete("Delete/{id:int}")]
    [EndpointSummary("Deletes an identified transaction.")]
    [EndpointDescription("Deletes a transaction by its identifier.")]
    [EndpointName("Delete Transaction")]
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
        catch (Exception exception)
        {
            _logger.LogError(exception, "An error occurred while deleting the transaction with ID: {Id}!", id);
            return Problem(
                detail: "An error occurred while processing your request. Please try again later.",
                instance: HttpContext.TraceIdentifier,
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Internal Server Error"
            );
        }
    }
    #endregion
    
    // Rest to do:
    // Add filtering to Web API (Backend) and Transactions Page (Frontend)
    // Add sorting to Web API (Backend) and Transactions Page (Frontend)
    // Add pagination to Web API (Backend) and Transactions Page (Frontend)
}