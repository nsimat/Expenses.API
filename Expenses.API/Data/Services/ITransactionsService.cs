using Expenses.API.Dtos;
using Expenses.API.Models;

namespace Expenses.API.Data.Services;

/// <summary>
/// Service to handle transactions CRUD operations
/// </summary>
public interface ITransactionsService
{
    /// <summary>
    /// Retrieves all transactions
    /// </summary>
    /// <param name="userId">The unique ID of the authentified user.</param>
    /// <returns>A list of transactions</returns>
    Task<IQueryable<Transaction>> GetAllAsync(int userId);
    
    /// <summary>
    /// Retrieves a transaction by its ID
    /// </summary>
    /// <param name="id">ID of transaction to fetch</param>
    /// <returns>The unique transaction represented by the given ID</returns>
    Task<Transaction?> GetByIdAsync(int id);
    
    /// <summary>
    /// Creates a new transaction
    /// </summary>
    /// <param name="transactionForCreation">TDO object representing transaction info</param>
    /// <param name="userId">The unique ID of the authentified user.</param>
    /// <returns>Transaction created in the database or null if not created</returns>
    Task<Transaction?> AddAsync(TransactionForCreationDto transactionForCreation, int userId);
    
    /// <summary>
    /// Updates an existing transaction
    /// </summary>
    /// <param name="id">Unique ID of transaction to modify</param>
    /// <param name="transactionForUpdate">A DTO containing transaction data to modify</param>
    /// <returns>The transaction updated</returns>
    Task<Transaction?> UpdateAsync(int id, TransactionForUpdateDto transactionForUpdate);
    
    /// <summary>
    /// deletes a transaction by its ID
    /// </summary>
    /// <param name="id">The unique transaction to remove from database</param>
    /// <returns>True if deletion successful or false if the transaction does not exist in the system.</returns>
    Task<bool> Delete(int id);
}