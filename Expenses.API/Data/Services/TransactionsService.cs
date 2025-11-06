using Expenses.API.Dtos;
using Expenses.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Expenses.API.Data.Services;

/// <summary>
/// Service for managing transactions in the Expenses application.
/// </summary>
public class TransactionsService(ExpensesDbContext expensesDbContext) : ITransactionsService
{
    /// <summary>
    /// Retrieves all transactions
    /// </summary>
    /// <returns>List of transactions</returns>
    public async Task<IEnumerable<Transaction>> GetAllAsync(int userId)
    {
        var transactions = await expensesDbContext.Transactions
            .Where(t => t.UserId == userId)
            .ToListAsync();
        return transactions;
    }

    /// <summary>
    /// Retrieves a transaction by its ID
    /// </summary>
    /// <param name="id">ID of transaction to fetch</param>
    /// <returns>The unique transaction if found, or null otherwise</returns>
    public async Task<Transaction?> GetByIdAsync(int id)
    {
        var transaction = await expensesDbContext.Transactions.FindAsync(id);
        return transaction;
    }

    /// <summary>
    /// Creates a new transaction
    /// </summary>
    /// <param name="transactionForCreation">TDO object representing transaction info</param>
    /// <returns>The created transaction</returns>
    public async Task<Transaction?> AddAsync(TransactionForCreationDto transactionForCreation, int userId)
    {
        var newTransaction = new Transaction
        {
            Type = transactionForCreation.Type,
            Amount = transactionForCreation.Amount,
            Category = transactionForCreation.Category,
            CreatedAt = transactionForCreation.CreatedAt ?? DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            UserId = userId
        };

        await expensesDbContext.Transactions.AddAsync(newTransaction);
        await expensesDbContext.SaveChangesAsync();

        var createdTransaction = await expensesDbContext.Transactions.SingleOrDefaultAsync(t =>
            t.Type == transactionForCreation.Type &&
            t.Amount == transactionForCreation.Amount &&
            t.Category == transactionForCreation.Category &&
            t.CreatedAt == newTransaction.CreatedAt);

        return createdTransaction;
    }

    /// <summary>
    /// Updates an existing transaction
    /// </summary>
    /// <param name="id">Unique ID of transaction to modify</param>
    /// <param name="transactionForUpdate">TDO object representing updated transaction info</param>
    /// <returns>The transaction updated</returns>
    public async Task<Transaction?> UpdateAsync(int id, TransactionForUpdateDto transactionForUpdate)
    {
        var existingTransaction = await expensesDbContext.Transactions.FindAsync(id);

        if (existingTransaction != null)
        {
            existingTransaction.Type = transactionForUpdate.Type;
            existingTransaction.Amount = transactionForUpdate.Amount;
            existingTransaction.Category = transactionForUpdate.Category;
            existingTransaction.UpdatedAt = DateTime.UtcNow;

            expensesDbContext.Update(existingTransaction);
            expensesDbContext.Entry(existingTransaction).State = EntityState.Modified;
            await expensesDbContext.SaveChangesAsync();
        }

        return existingTransaction;
    }

    /// <summary>
    /// deletes a transaction by its ID
    /// </summary>
    /// <param name="id">The unique transaction to remove from database</param>
    /// <returns>True if deletion successful or false if the transaction does not exist in the system.</returns>
    public async Task<bool> Delete(int id)
    {
        var isDeleted = false;
        var existingTransaction = await expensesDbContext.Transactions.FindAsync(id);

        if (existingTransaction == null) return isDeleted;

        expensesDbContext.Transactions.Remove(existingTransaction);
        await expensesDbContext.SaveChangesAsync();
        isDeleted = true;

        return isDeleted;
    }
}