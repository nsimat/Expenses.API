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

    public async Task<Transaction?> AddAsync(TransactionCreateDto transactionCreate)
    {
        var newTransaction = new Transaction
        {
            Type = transactionCreate.Type,
            Amount = transactionCreate.Amount,
            Category = transactionCreate.Category,
            CreatedAt = transactionCreate.CreatedAt ?? DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await expensesDbContext.Transactions.AddAsync(newTransaction);
        await expensesDbContext.SaveChangesAsync();

        var createdTransaction = await expensesDbContext.Transactions.SingleOrDefaultAsync(t =>
            t.Type == transactionCreate.Type &&
            t.Amount == transactionCreate.Amount &&
            t.Category == transactionCreate.Category &&
            t.CreatedAt == newTransaction.CreatedAt);

        return createdTransaction;
    }

    public async Task<Transaction?> UpdateAsync(int id, TransactionUpdateDto transactionUpdate)
    {
        var existingTransaction = await expensesDbContext.Transactions.FindAsync(id);

        if (existingTransaction != null)
        {
            existingTransaction.Type = transactionUpdate.Type;
            existingTransaction.Amount = transactionUpdate.Amount;
            existingTransaction.Category = transactionUpdate.Category;
            existingTransaction.UpdatedAt = DateTime.UtcNow;

            expensesDbContext.Update(existingTransaction);
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