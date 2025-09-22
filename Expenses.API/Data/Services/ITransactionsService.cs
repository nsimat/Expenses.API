using Expenses.API.Dtos;
using Expenses.API.Models;

namespace Expenses.API.Data.Services;

public interface ITransactionsService
{
    Task<IEnumerable<Transaction>> GetAllAsync();
    Task<Transaction?> GetByIdAsync(int id);
    Task<Transaction?> AddAsync(TransactionCreateDto transactionCreate);
    Task<Transaction?> UpdateAsync(int id, TransactionUpdateDto transactionUpdate);
    Task<bool> Delete(int id);
}