using AutoInject.Attributes.ScopedAttributes;
using Expenses.API.Brokers.DateTimes;
using Expenses.API.Brokers.Loggings;
using Expenses.API.Brokers.Storages;
using Expenses.API.DTOs.Mapping;
using Expenses.API.DTOs.Transactions;
using Expenses.API.Models;
using Microsoft.EntityFrameworkCore;


namespace Expenses.API.Services.Foundations.Transactions;

/// <summary>
/// Service for managing transactions in the Expenses application.
/// </summary>
[Scoped(typeof(ITransactionService))]
public partial class TransactionService : ITransactionService
{
    # region Fields
    
    /// <summary>
    /// The storage broker for interacting with the database.
    /// </summary>
    private readonly IStorageBroker _storageBroker;

    /// <summary>
    /// The date time broker for handling date and time operations.
    /// </summary>
    private readonly IDateTimeBroker _dateTimeBroker;

    /// <summary>
    /// The logging broker for logging information and errors.
    /// </summary>
    private readonly ILoggingBroker _loggingBroker;

    /// <summary>
    /// Provides access to the current HTTP context for retrieving user-specific information, such as user claims and
    /// request metadata.
    /// </summary>
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    #endregion

    # region Constructors: Called when using new to instantiate a TransactionService type
    
    /// <summary>
    /// Constructor for the TransactionService class, initializing the storage broker, date time broker, and logging
    /// broker.
    /// </summary>
    /// <param name="storageBroker">The Storage Broker for all interactions with the database</param>
    /// <param name="dateTimeBroker">The DateTime Broker to create time corresponding to the local zone</param>
    /// <param name="loggingBroker">The logging Broker to manage logging across layers</param>
    /// <param name="httpContextAccessor">The accessor to the current HttpContext, if one is available</param>
    /// <exception cref="ArgumentNullException">Exception thrown if an argument is null during creation</exception>
    public TransactionService(
        IStorageBroker storageBroker,
        IDateTimeBroker dateTimeBroker,
        ILoggingBroker loggingBroker,
        IHttpContextAccessor httpContextAccessor)
    {
        _storageBroker = storageBroker ?? throw new ArgumentNullException(nameof(storageBroker));
        _dateTimeBroker = dateTimeBroker ?? throw new ArgumentNullException(nameof(dateTimeBroker));
        _loggingBroker = loggingBroker ?? throw new ArgumentNullException(nameof(loggingBroker));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }
    
    #endregion
    
    # region CRUD Operations: Called by the controller to perform transactions operations (e.g., add, retrieve, modify, delete)
    
    /// <inheritdoc/>
    public ValueTask<TransactionResponse> AddTransactionAsync(TransactionCreateRequest transactionCreate) =>
        TryCatch(async () =>
        {
            // Step 0. Checks if the client is the current user initiating the creation of the transaction
            var currentUserId = ValidateUserAccountIdIsNotNull(_httpContextAccessor);

            _loggingBroker.LogInformation($"Attempting to add a new transaction for user with ID: {currentUserId}...");

            // Step 1. Maps the DTO parameter to a new transaction object to be persisted in the database
            var newTransaction = new Transaction
            {
                Id = Guid.CreateVersion7(),
                Type = transactionCreate.Type,
                Amount = transactionCreate.Amount,
                Category = transactionCreate.Category,
                UserId = currentUserId,
                CreatedAt = transactionCreate.CreatedAt ?? _dateTimeBroker.GetCurrentDateTimeOffset(),
                UpdatedAt = _dateTimeBroker.GetCurrentDateTimeOffset()
            };

            // Step 2. Validates the new transaction object 
            ValidateTransactionOnAdd(newTransaction);

            // Step 3. Process, persist the new transaction and return the newly inserted data (i.e., transaction)
            var insertedTransaction = await _storageBroker.InsertTransactionAsync(newTransaction);

            // Return the newly inserted transaction mapped to a transaction DTO
            return insertedTransaction.ToTransactionDto();
        });
    
    /// <inheritdoc/>
    public IQueryable<TransactionResponse> RetrieveAllTransactions() =>
        TryCatch(() =>
        {
            // Step 0. Checks if the client is the current user initiating the retrieval of transactions
            var currentUserId = ValidateUserAccountIdIsNotNull(_httpContextAccessor);

            // Step 1. Retrieves all transactions for the current user from the storage and returns them as a queryable collection
            var transactions = _storageBroker
                .SelectAllTransactions()
                .AsNoTracking()
                .Where(t => t.UserId == currentUserId);

            // Set 2. Maps all the transactions to transactionDtos
            var transactionDtos = transactions.ToTransactionDtos();

            // DTOs are mapped in the service layer to avoid unnecessary mapping of transactions
            // that do not belong to the current user
            return transactionDtos;      
        });
    
    /// <inheritdoc/>
    public IQueryable<TransactionResponse> RetrieveAllTransactions(int pageNumber, int pageSize) =>
        TryCatch(() =>
        {
            _loggingBroker.LogInformation($"Attempting to retrieve all transactions with pagination...");
            
            // Step 0. Validates if the request comes from the current user initiating the retrieval of transactions
            var currentUserId = ValidateUserAccountIdIsNotNull(_httpContextAccessor);

            // Step 1. Retrieves all transactions for the current user from the storage and returns them as a queryable
            // collection
            //var skip = (pageNumber - 1) * pageSize; ???? <--- to review
            var transactions = _storageBroker.SelectAllTransactions()
                .AsNoTracking() 
                .Where(t => t.UserId == currentUserId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToTransactionDtos();
            
            return transactions;
        });

    /// <inheritdoc/>
    public ValueTask<TransactionResponse> RetrieveTransactionByIdAsync(Guid transactionId) =>
        TryCatch(async () =>
        {
            _loggingBroker.LogInformation($"Attempting to retrieve transaction with ID: {transactionId}...");

            // Step 0. Validates the given transaction ID
            ValidateTransactionId(transactionId);

            // Step 1. Retrieves the transaction with the given ID
            var maybeTransaction = await _storageBroker.SelectTransactionByIdAsync(transactionId);

            // Step 2. Validates the retrieved transaction
            ValidateStorageTransaction(maybeTransaction, transactionId);

            // Returns the transaction mapped to a transaction DTO
            return maybeTransaction.ToTransactionDto();
        });
    
    /// <inheritdoc/>
    public ValueTask<TransactionResponse> ModifyTransactionAsync(Guid transactionId, // à revoir
        TransactionUpdateRequest transactionUpdate) =>
        TryCatch(async () =>
        {
            _loggingBroker.LogInformation($"Attempting to update transaction with ID: {transactionId}...");
            
            // Step 0. Validates the given transaction ID
            ValidateTransactionId(transactionId);
            
            // Step 1. Checks if the transaction to be updated exists in the storage
            var maybeTransaction = await _storageBroker.SelectTransactionByIdAsync(transactionId);

            // Step 2. Validates the retrieved transaction
            ValidateStorageTransaction(maybeTransaction, transactionId);
            
            // Step 3. Updates the transaction with the given transaction data
            maybeTransaction.Type = transactionUpdate.Type;
            maybeTransaction.Amount = transactionUpdate.Amount;
            maybeTransaction.Category = transactionUpdate.Category;
            maybeTransaction.UpdatedAt = _dateTimeBroker.GetCurrentDateTimeOffset();
            
            // Step 4. Validates the updated transaction
            ValidateTransactionOnModify(maybeTransaction);

            // Step 5. Processes and persists the updated transaction and return the updated transaction
            var updatedTransaction = await _storageBroker.UpdateTransactionAsync(maybeTransaction);
            
            // Return the updated transaction mapped to a transaction DTO
            return updatedTransaction.ToTransactionDto();
        });
    
    /// <inheritdoc/>
    public ValueTask<TransactionResponse> RemoveTransactionByIdAsync(Guid transactionId) =>
        TryCatch(async () =>
        {
            _loggingBroker.LogInformation($"Attempting to delete transaction with ID: {transactionId}...");

            // Step 0. Validates the transaction ID
            ValidateTransactionId(transactionId);

            // Step 1. Validates if the transaction to be deleted exists in the storage
            var maybeTransaction = await _storageBroker.SelectTransactionByIdAsync(transactionId);

            // Step 2. Validates the existence of the retrieved transaction in the storage
            ValidateStorageTransaction(maybeTransaction, transactionId);

            // Step 3. Deletes the transaction from the storage and return the deleted transaction
            var deletedTransaction = await _storageBroker.DeleteTransactionAsync(maybeTransaction);
            
            // Return the deleted transaction mapped to a transaction DTO
            return deletedTransaction.ToTransactionDto();
        });
    
    #endregion
}