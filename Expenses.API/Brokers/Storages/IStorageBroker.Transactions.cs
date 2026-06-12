using Expenses.API.Models;

namespace Expenses.API.Brokers.Storages;

public partial interface IStorageBroker
{
    /// <summary>
    /// Inserts a new transaction into the storage
    /// </summary>
    /// <param name="transaction">Transaction to be inserted</param>
    /// <returns>The instance of the inserted transaction</returns>
    ValueTask<Transaction> InsertTransactionAsync(Transaction transaction);
    
    /// <summary>
    /// Retrieves all transactions from the storage
    /// </summary>
    /// <returns>All transactions</returns>
    IQueryable<Transaction> SelectAllTransactions();

    /// <summary>
    /// Retrieves a transaction by its unique ID from the storage
    /// </summary>
    /// <param name="transactionId">The ID of the transaction to retrieve</param>
    /// <returns>The transaction with the specified ID</returns>
    ValueTask<Transaction?> SelectTransactionByIdAsync(Guid transactionId); 
    
    /// <summary>
    /// Updates an existing transaction in the storage
    /// </summary>
    /// <param name="transaction">The updated transaction data</param>
    /// <returns>The updated transaction</returns>
    ValueTask<Transaction> UpdateTransactionAsync(Transaction transaction);
    
    /// <summary>
    /// Deletes a transaction from the storage
    /// </summary>
    /// <param name="transaction">The transaction to delete</param>
    /// <returns>The deleted transaction</returns>
    ValueTask<Transaction> DeleteTransactionAsync(Transaction transaction);
}