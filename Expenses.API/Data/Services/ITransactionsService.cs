using Expenses.API.Dtos;
using Expenses.API.Models;

namespace Expenses.API.Data.Services;

public interface ITransactionsService
{
    Task<IEnumerable<Transaction>> GetAllAsync();
    Task<Transaction?> GetByIdAsync(int id);
    Task<Transaction?> AddAsync(TransactionForCreationDto transactionForCreation);
    Task<Transaction?> UpdateAsync(int id, TransactionForUpdateDto transactionForUpdate);
    Task<bool> Delete(int id);
}