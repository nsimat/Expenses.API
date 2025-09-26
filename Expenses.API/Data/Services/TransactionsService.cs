using Expenses.API.Dtos;
using Expenses.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Expenses.API.Data.Services;

public class TransactionsService(ExpensesDbContext expensesDbContext) : ITransactionsService
{
    public async Task<IEnumerable<Transaction>> GetAllAsync()
    {
        var transactions = await expensesDbContext.Transactions.ToListAsync();
        return transactions;
    }

    public async Task<Transaction?> GetByIdAsync(int id)
    {
        var transaction = await expensesDbContext.Transactions.FindAsync(id);
        return transaction;
    }

    public async Task<Transaction?> AddAsync(TransactionForCreationDto transactionForCreation)
    {
        var newTransaction = new Transaction
        {
            Type = transactionForCreation.Type,
            Amount = transactionForCreation.Amount,
            Category = transactionForCreation.Category,
            CreatedAt = transactionForCreation.CreatedAt ?? DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
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