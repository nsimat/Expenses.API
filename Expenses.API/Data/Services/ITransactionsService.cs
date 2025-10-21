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
    /// <returns></returns>
    Task<IEnumerable<Transaction>> GetAllAsync();
    
    /// <summary>
    /// Retrieves a transaction by its ID
    /// </summary>
    /// <param name="id">ID of transaction to fetch</param>
    /// <returns></returns>
    Task<Transaction?> GetByIdAsync(int id);
    
    /// <summary>
    /// Creates a new transaction
    /// </summary>
    /// <param name="transactionForCreation">TDO object representing transaction info</param>
    /// <returns></returns>
    Task<Transaction?> AddAsync(TransactionForCreationDto transactionForCreation);
    
    /// <summary>
    /// Updates an existing transaction
    /// </summary>
    /// <param name="id">Unique ID of transaction to modify</param>
    /// <param name="transactionForUpdate"></param>
    /// <returns>The transaction updated</returns>
    Task<Transaction?> UpdateAsync(int id, TransactionForUpdateDto transactionForUpdate);
    
    /// <summary>
    /// deletes a transaction by its ID
    /// </summary>
    /// <param name="id">The unique transaction to remove from database</param>
    /// <returns>True if deletion successful or false if the transaction does not exist in the system.</returns>
    Task<bool> Delete(int id);
}