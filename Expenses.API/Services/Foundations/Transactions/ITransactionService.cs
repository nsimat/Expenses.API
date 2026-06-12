using Expenses.API.DTOs.Transactions;

namespace Expenses.API.Services.Foundations.Transactions;

/// <summary>
/// Service contract for handling transactions CRUD operations
/// </summary>
public interface ITransactionService
{
    /// <summary>
    /// Creates and adds a new transaction for an authenticated user
    /// </summary>
    /// <param name="transactionCreate">TDO object representing transaction info</param>
    /// <returns>Transaction created in the database or null if not created</returns>
    ValueTask<TransactionResponse> AddTransactionAsync(TransactionCreateRequest transactionCreate);

    /// <summary>
    /// Retrieves all transactions for an authenticated user
    /// </summary>
    /// <returns>List of transactions created by the current authenticated user</returns>
    IQueryable<TransactionResponse> RetrieveAllTransactions();

    /// <summary>
    /// Retrieve all transactions 
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve</param>
    /// <param name="pageSize">The number of transactions per page</param>
    /// <returns>Paginated list of transactions created by the current authenticated user</returns>
    IQueryable<TransactionResponse> RetrieveAllTransactions(int pageNumber, int pageSize);

    /// <summary>
    /// Retrieves a transaction by its ID
    /// </summary>
    /// <param name="transactionId">ID of transaction to fetch</param>
    /// <returns>The unique transaction represented by the given ID</returns>
    ValueTask<TransactionResponse> RetrieveTransactionByIdAsync(Guid transactionId);

    /// <summary>
    /// Updates an existing transaction
    /// </summary>
    /// <param name="transactionId">Unique ID of transaction to modify</param>
    /// <param name="transactionUpdate">A DTO containing transaction data to modify</param>
    /// <returns>The transaction updated</returns>
    ValueTask<TransactionResponse> ModifyTransactionAsync(Guid transactionId, TransactionUpdateRequest transactionUpdate);

    /// <summary>
    /// Deletes a transaction with a given ID
    /// </summary>
    /// <param name="transactionId">The unique ID of the transaction to be removed from the database</param>
    /// <returns>Deleted transaction if deletion is successful or null if the transaction does not exist in the system.</returns>
    ValueTask<TransactionResponse> RemoveTransactionByIdAsync(Guid transactionId);
}