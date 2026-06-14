using Expenses.API.Brokers.Loggings;
using Expenses.API.DTOs.Transactions;
using Expenses.API.Models;
using Expenses.API.Models.Exceptions.Transactions;
using Expenses.API.Services.Foundations.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace Expenses.API.Controllers;

/// <summary>
/// Represents a controller to manage transaction CRUD operations in the Expenses API. This controller handles HTTP
/// requests related to transactions, such as creating, retrieving, updating, and deleting transactions.
/// </summary>
[Route("api/v1/transactions")]
[ApiController]
[Authorize]
public class TransactionsController : RESTFulController
{
    #region Fields

    // Readonly Fields to ensure immutability after construction.

    /// <summary>
    /// Represents a logging broker that is responsible for handling logging operations
    /// within the controller to facilitate application monitoring and diagnostics.
    /// </summary>
    private readonly ILoggingBroker _loggingBroker;

    /// <summary>
    /// Provides methods to manage transactions including creation, retrieval, updating,
    /// and handling transaction-related business logic within the application.
    /// </summary>
    private readonly ITransactionService _transactionService;

    #endregion

    #region Constructor: Called when using new to instantiate a TransactionsController type

    /// <summary>
    /// Initializes a new instance of the TransactionsController class with the specified transaction service and
    /// logging broker.
    /// </summary>
    /// <param name="transactionService">Service to handle business logic related to transactions.</param>
    /// <param name="loggingBroker">A logger for capturing valuable information during runtime.</param>
    /// <exception cref="ArgumentNullException">Throws argument null exception if injected objects are null.</exception>
    public TransactionsController(ITransactionService transactionService, ILoggingBroker loggingBroker)
    {
        _transactionService = transactionService ?? throw new ArgumentNullException(nameof(transactionService));
        _loggingBroker = loggingBroker ?? throw new ArgumentNullException(nameof(loggingBroker));
    }

    #endregion

    #region Endpoints for Transactions CRUD operations

    // Actions should always be clean and simple, delegate complex logic to services.
    // Each action should handle a single responsibility. Their main job is to handle HTTP requests,
    // validate models, catch errors, and return responses.

    // POST: api/transactions/create
    /// <summary>
    /// Creates a new transaction.
    /// </summary>
    /// <param name="newTransactionRequest">A DTO that represents parts of the transaction to be created.</param>
    /// <returns>The created transaction if creation is successful. Otherwise, an error is returned.</returns>
    /// <response code="201">Transaction created successfully. Returns created transaction and location header pointing
    /// to GetTransactionById.</response>
    /// <response code="400">The provided payload is null or invalid.</response>
    /// <response code="409">Transaction with specified ID already exists.</response>
    /// <response code="424">Transaction dependency failed.</response>
    /// <response code="500">An Internal Server Error occurred while processing the request.</response>
    /// <exception cref="ArgumentNullException">Thrown when the provided payload is null.</exception>
    [HttpPost("create")]
    [Tags("Transactions")]
    [EndpointSummary("Creates a new transaction.")]
    [EndpointDescription("POST to create a new transaction. Accepts a TransactionForCreationDTO.")]
    [EndpointName("Create Transaction")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Transaction))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ProblemDetails) )]
    [ProducesResponseType(StatusCodes.Status424FailedDependency, Type = typeof(ProblemDetails) )]   
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async ValueTask<IActionResult> CreateTransaction([FromBody] TransactionCreateRequest newTransactionRequest)
    {
        _loggingBroker.LogInformation("Attempting to create a new transaction...");
        
        try
        {
            var createdTransaction = await _transactionService.AddTransactionAsync(newTransactionRequest);

            // Returns a 201 Created response with the location header of the newly created transaction.
            return CreatedAtAction(nameof(GetTransactionById),
                new { transactionId = createdTransaction.Id }, createdTransaction);
            
            // List of possible exceptions and their handling:
            // 1- NullTransactionException --> TransactionValidationException(NullTransactionIdException)
            // 2- InvalidTransactionException --> TransactionValidationException(InvalidTransactionIdException)
            // 3- SqlException --> TransactionDependencyException(FailedTransactionStorageException)
            // 4- NotFoundTransactionException --> TransactionValidationException(NotFoundTransactionException)
            // 5- DuplicateKeyException --> TransactionDependencyValidationException(AlreadyExistsTransactionException)
            // 6- ForeignKeyConstraintConflictException --> TransactionDependencyValidationException(InvalidTransactionReferenceException)
            // 7- DbUpdateConcurrencyException --> TransactionDependencyValidationException(LockedTransactionException)
            // 8- DbUpdateException --> TransactionDependencyException(FailedTransactionStorageException)
            // 9- Exception --> TransactionServiceException(FailedTransactionServiceException)
            
            // From service layer:
            // 1- TransactionValidationException --> BadRequest(TransactionValidationException)
            // 2- TransactionDependencyValidationException --> FailedDependency(TransactionDependencyValidationException)
            // 3- TransactionDependencyException --> InternalServerError(TransactionDependencyException)
            // 4- TransactionServiceException --> InternalServerError(TransactionServiceException)
        }
        catch (TransactionValidationException transactionValidationException)
        {
            _loggingBroker.LogError(transactionValidationException);
            
            return BadRequest(transactionValidationException);
        }
        catch (TransactionDependencyValidationException transactionValidationException)
            when (transactionValidationException.InnerException is InvalidTransactionReferenceException)
        {
            return FailedDependency(transactionValidationException.InnerException);
        }
        catch (TransactionDependencyValidationException transactionDependencyValidationException)
            when (transactionDependencyValidationException.InnerException is AlreadyExistsTransactionException)
        {
            return Conflict(transactionDependencyValidationException.InnerException);
        }
        catch (TransactionDependencyException transactionDependencyException)
        {
            return InternalServerError(transactionDependencyException);
        }
        catch (TransactionServiceException transactionServiceException)
        {
            return InternalServerError(transactionServiceException);
        }
    }

    // GET: api/transactions/all
    /// <summary>
    /// Gets all the transactions created by the authentified user in the database.
    /// </summary>
    /// <returns>List of transactions created by the authentified user</returns>
    /// <response code="200">Successfully returns all transactions found in the database.</response>
    /// <response code="204">If no transaction is found in the database.</response>
    /// <response code="500">An Internal Server Error prevented the request from being processed.</response>
    /// <exception cref="Exception">Throws exception if an error occurs while retrieving transactions.</exception>
    [HttpGet("all-user-transactions")]
    [Tags("Transactions")]
    [EndpointSummary("Obtains all transactions of the identified user.")]
    [EndpointDescription("Obtains all transactions from the database created by the identified user.")]
    [EndpointName("Get all Transactions")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IQueryable<Transaction>))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]    
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public ActionResult<IQueryable<TransactionResponse>> GetAllTransactions(
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10)
    {
        _loggingBroker.LogInformation("Fetching all transactions of the connected user...");
        
        try
        {
            var transactions = 
                _transactionService.RetrieveAllTransactions(pageNumber, pageSize);

            return Ok(transactions);
            
            // List of possible exceptions and their handling:
            // 1- NullUserAccountIdException --> TransactionValidationException(NullUserAccountIdException)
            // 2- SqlException --> TransactionDependencyException(FailedTransactionStorageException)
            // 3- Exception --> TransactionServiceException(FailedTransactionServiceException)
            
            // From service layer:
            // 1- TransactionValidationException --> Unauthorized(TransactionValidationException)
            // 2- TransactionDependencyException --> InternalServerError(TransactionDependencyException)
            // 3- TransactionServiceException --> InternalServerError(TransactionServiceException)
        }
        catch (TransactionDependencyException transactionDependencyException)
        {
            return InternalServerError(transactionDependencyException);
        }
        catch (TransactionServiceException transactionServiceException)
        {
            return InternalServerError(transactionServiceException);
        }
    }

    // GET: api/transactions/5/details
    /// <summary>
    /// Gets a single transaction by its unique ID from the database.
    /// </summary>
    /// <param name="transactionId">The unique ID of the transaction to retrieve.</param>
    /// <returns>The requested transaction if found, or NotFound if not available.</returns>
    /// <response code="200">Specified transaction found and returned successfully.</response>
    /// <response code="400">The provided payload is null or invalid.</response>   
    /// <response code="404">Specified transaction with unique ID not found.</response>
    /// <response code="500">An Internal Server Error occurred while processing the request.</response>
    /// <exception cref="Exception">Throws exception if an error occurs while retrieving the transaction.</exception>
    [HttpGet("{transactionId:guid}/details")]
    [Tags("Transactions")]
    [EndpointSummary("Get one transaction by ID from the database.")]
    [EndpointDescription("Fetches one specific transaction by its ID.")]
    [EndpointName("Transaction Details")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Transaction))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async ValueTask<IActionResult> GetTransactionById(Guid transactionId)
    {
        _loggingBroker.LogInformation($"Fetching transaction with ID: {transactionId}...");
        
        try
        {
            var transactionResponse = await _transactionService.RetrieveTransactionByIdAsync(transactionId);
            
            return Ok(transactionResponse);
        }
        catch (TransactionValidationException transactionValidationException)
            when (transactionValidationException.InnerException is InvalidTransactionIdException) // To check out
        {
            return BadRequest(transactionValidationException.InnerException);
        }
        catch (TransactionValidationException transactionValidationException)
            when (transactionValidationException.InnerException is NotFoundTransactionException)
        {
            return NotFound(transactionValidationException.InnerException);
        }
        catch (TransactionDependencyException transactionDependencyException)
        {
            return InternalServerError(transactionDependencyException);
        }
        catch (TransactionServiceException transactionServiceException)
        {
            return InternalServerError(transactionServiceException);
        }
    }

    // PUT: api/transactions/5/update
    /// <summary>
    /// Modifies the content of an existing transaction.
    /// </summary>
    /// <param name="transactionId">The unique identifier of transaction to modify.</param>
    /// <param name="updateRequest">A DTO object representing the transaction to modify</param>
    /// <returns>Transaction updated from a database.</returns>
    /// <response code="200">Transaction updated successfully.</response>
    /// <response code="400">The provided payload is null or invalid.</response>
    /// <response code="404">Transaction with specified ID not found.</response>
    /// <response code="409">Transaction with specified ID already exists.</response>
    /// <response code="424">Transaction dependency failed.</response>
    /// <response code="500">An error occurred while processing the request.</response>
    /// <exception cref="ArgumentNullException">Thrown when the provided payload is null.</exception>
    [HttpPut("{transactionId:guid}/update")]
    [Tags("Transactions")]
    [EndpointSummary("Updates an existing transaction.")]
    [EndpointDescription("PUT to update an existing transaction. Accepts a TransactionForUpdateDTO.")]
    [EndpointName("Update Transaction")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status424FailedDependency, Type = typeof(ProblemDetails) )]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ProblemDetails) )]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async ValueTask<IActionResult> UpdateTransaction(
        Guid transactionId, 
        [FromBody] TransactionUpdateRequest updateRequest)
    {
        _loggingBroker.LogInformation($"Updating transaction with ID:{transactionId}...");
        
        try
        {
            var modifiedTransaction = await _transactionService.ModifyTransactionAsync(transactionId, updateRequest);
            
            return Ok(modifiedTransaction);
        }
        catch (TransactionValidationException transactionValidationException)
            when (transactionValidationException.InnerException is NotFoundTransactionException)
        {
            return NotFound(transactionValidationException.InnerException);
        }
        catch (TransactionValidationException transactionValidationException)
            when (transactionValidationException.InnerException is InvalidTransactionIdException)
        {
            return BadRequest(transactionValidationException.InnerException);
        }
        catch (TransactionDependencyValidationException transactionDependencyValidationException) 
            when (transactionDependencyValidationException.InnerException is InvalidTransactionReferenceException)
        {
            return FailedDependency(transactionDependencyValidationException.InnerException);
        }
        catch (TransactionDependencyValidationException transactionDependencyValidationException) 
        when (transactionDependencyValidationException.InnerException is AlreadyExistsTransactionException)
        {
            return Conflict(transactionDependencyValidationException.InnerException);
        }
        catch (TransactionDependencyException transactionDependencyException)
        {
            return InternalServerError(transactionDependencyException);
        }
        catch (TransactionServiceException transactionServiceException)
        {
            return InternalServerError(transactionServiceException);
        }
    }

    // DELETE: api/v1/transactions/{id}/delete
    /// <summary>
    /// Deletes a transaction by its ID.
    /// </summary>
    /// <param name="transactionId">The unique identifier of transaction to delete.</param>
    /// <returns>No content if deletion is successful. Otherwise, an error is returned.</returns>
    /// <response code="204">Transaction deleted successfully.</response>
    /// <response code="404">Transaction with specified ID not found.</response>
    /// <response code="403">Forbidden. User does not have permission to delete the transaction.</response>
    /// <response code="500">An error occurred while processing the request.</response>
    /// <exception cref="Exception">Throws exception if an error occurs while deleting the transaction.</exception>
    [HttpDelete("{transactionId:guid}/delete")]
    [Tags("Transactions")]
    [EndpointSummary("Deletes an existing transaction with a specific identifier.")]
    [EndpointDescription("DELETE to remove an existing transaction by its identifier.")]
    [EndpointName("Delete Transaction")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TransactionResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails) )]
    [ProducesResponseType(StatusCodes.Status423Locked, Type = typeof(ProblemDetails) )]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async ValueTask<IActionResult> DeleteTransaction(Guid transactionId)
    {
        _loggingBroker.LogInformation($"Deleting transaction with ID: {transactionId}...");
        
        try
        {
            var deletedTransaction = await _transactionService.RemoveTransactionByIdAsync(transactionId);
            
            return Ok(deletedTransaction);
        }
        catch (TransactionValidationException transactionValidationException)
            when (transactionValidationException.InnerException is NotFoundTransactionException)
        {
            return NotFound(transactionValidationException.InnerException);
        }
        catch (TransactionValidationException transactionValidationException)
        {
            return BadRequest(transactionValidationException.InnerException);
        }
        catch (TransactionDependencyValidationException transactionDependencyValidationException)
            when (transactionDependencyValidationException.InnerException is LockedTransactionException)
        {
            return Locked(transactionDependencyValidationException.InnerException);
        }
        catch (TransactionDependencyValidationException transactionDependencyValidationException)
        {
            return BadRequest(transactionDependencyValidationException);
        }
        catch (TransactionDependencyException transactionDependencyException)
        {
            return InternalServerError(transactionDependencyException);
        }
        catch (TransactionServiceException transactionServiceException)
        {
            return InternalServerError(transactionServiceException);
        }
    }
    #endregion

    // Remain to do:
    // -Add authorization to ensure that users can only access and modify their own transactions. (Currently,
    //  there is no check to ensure that the user is only accessing their own transactions. We should add authorization
    //  checks to prevent unauthorized access). -->???
    // -Add filtering to Web API (Backend) and Transactions Page (Frontend) -->???
    // -Add sorting to Web API (Backend) and Transactions Page (Frontend) -->???
    // -Add pagination to Web API (Backend) and Transactions Page (Frontend) -->???
    // -Add unit tests for the controller and the service.
    // -Add integration tests for the controller and the service.
    // -Add more logging to capture important events and errors in the controller and the service. -->ok
    // -Add caching to improve performance for frequently accessed data.
    // -Add validation to ensure that the data being sent to the API is valid and meets the required criteria. -->ok
    // -Add error handling to gracefully handle exceptions and return appropriate error responses to the client. -->ok
    // -Add monitoring and logging to track the performance and health of the application. -->??
    // -Add security measures to protect against unauthorized access to sensitive data. -->??
    // -Add support for different data formats and media types to support different clients and devices. -->??
    // -Add support for different languages and cultures to provide localized content and user interfaces. -->??
    // -Add support for different authentication methods and protocols to secure the API. -->??
    // -Add support for different storage mechanisms to store and retrieve data efficiently. -->??
    // -Add support for different caching strategies to improve performance and reduce a load on the database. -->??
    // -Add support for different logging mechanisms to track and analyze application performance and errors. -->??
    // -Add support for different monitoring and alerting mechanisms to notify developers of potential issues. -->??
}