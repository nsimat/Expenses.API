using Expenses.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Expenses.API.Brokers.Storages;

/// <summary>
/// Class responsible for interacting with the database using Entity Framework Core.
/// </summary>
public partial class StorageBroker
{
    /// <summary>
    /// DbSet representing the collection of financial transactions in the storage.
    /// </summary>
    public DbSet<Transaction> Transactions { get; set; }
    
    /// <summary>
    /// Saves a new transaction
    /// </summary>
    /// <param name="transaction">The new transaction to save.</param>
    /// <returns>The created instance of transaction</returns>
    public async ValueTask<Transaction> InsertTransactionAsync(Transaction transaction) 
        => await InsertAsync(transaction);

    /// <inheritdoc/>
    public IQueryable<Transaction> SelectAllTransactions() => SelectAll<Transaction>();

    /// <inheritdoc/>
    public async ValueTask<Transaction?> SelectTransactionByIdAsync(Guid transactionId) 
        => await SelectAsync<Transaction>(transactionId);

    /// <inheritdoc/>
    public async ValueTask<Transaction> UpdateTransactionAsync(Transaction transaction) 
        => await UpdateAsync(transaction);

    /// <inheritdoc/>
    public async ValueTask<Transaction> DeleteTransactionAsync(Transaction transaction) 
        => await DeleteAsync(transaction);
}