using Expenses.API.Dtos;
using Expenses.API.Models;

namespace Expenses.API.Data.Services;

public class TransactionsService(ExpensesDbContext expensesDbContext): ITransactionsService
{
    public List<Transaction> GetAll()
    {
        var transactions = expensesDbContext.Transactions.ToList();
        return transactions;
    }

    public Transaction? GetById(int id)
    {
        var transaction = expensesDbContext.Transactions.Find(id);
        return transaction;
    }

    public void Add(PostTransactionDto transaction)
    {
        var newTransaction = new Transaction
        {
            Type = transaction.Type,
            Amount = transaction.Amount,
            Category = transaction.Category,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        expensesDbContext.Transactions.Add(newTransaction);
        expensesDbContext.SaveChanges();
    }

    public Transaction? Update(int id, PutTransactionDto transaction)
    {
        var existingTransaction = expensesDbContext.Transactions.Find(id);

        if (existingTransaction != null)
        {
            existingTransaction.Type = transaction.Type;
            existingTransaction.Amount = transaction.Amount;
            existingTransaction.Category = transaction.Category;
            existingTransaction.UpdatedAt = DateTime.UtcNow;

            expensesDbContext.Update(existingTransaction);
            expensesDbContext.SaveChanges();
        }
        return existingTransaction;
    }

    public void Delete(int id)
    {
        var existingTransaction = expensesDbContext.Transactions.Find(id);

        if (existingTransaction != null)
        {
            expensesDbContext.Transactions.Remove(existingTransaction);
            expensesDbContext.SaveChanges(); 
        }
    }
}