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

    public Transaction? Add(PostTransactionDto transaction)
    {
        var newTransaction = new Transaction
        {
            Type = transaction.Type,
            Amount = transaction.Amount,
            Category = transaction.Category,
            CreatedAt = transaction.CreatedAt ?? DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        expensesDbContext.Transactions.Add(newTransaction);
        expensesDbContext.SaveChanges();
        
        var createdTransaction = expensesDbContext.Transactions.SingleOrDefault(t =>
            t.Type == transaction.Type &&
            t.Amount == transaction.Amount &&
            t.Category == transaction.Category &&
            t.CreatedAt == newTransaction.CreatedAt);

        return createdTransaction;
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

    public bool Delete(int id)
    {
        bool isDeleted = false;
        var existingTransaction = expensesDbContext.Transactions.Find(id);

        if (existingTransaction != null)
        {
            expensesDbContext.Transactions.Remove(existingTransaction);
            expensesDbContext.SaveChanges(); 
            isDeleted = true;
        }
        return isDeleted;
    }
}