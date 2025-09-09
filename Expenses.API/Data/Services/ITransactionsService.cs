using Expenses.API.Dtos;
using Expenses.API.Models;

namespace Expenses.API.Data.Services;

public interface ITransactionsService
{
    List<Transaction> GetAll();
    Transaction? GetById(int id);
    Transaction? Add(PostTransactionDto transaction);
    Transaction? Update(int id, PutTransactionDto transaction);
    bool Delete(int id);
}