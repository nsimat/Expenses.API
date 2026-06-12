using Expenses.API.Models;
using Expenses.API.Models.Exceptions.Transactions;
using Expenses.API.Models.Exceptions.UserAccounts;

namespace Expenses.API.Services.Foundations.Admin;

public partial class AdminService
{
    public void ValidateUserAccountId(Guid userId)
    {
        if (userId == Guid.Empty || !Guid.TryParse(userId.ToString(), out _))
        {
            throw new InvalidUserAccountException();
        }
    }
    
    private void ValidateStorageUserAccount(UserAccount userAccount, Guid userId)
    {
        if (userAccount is null || userAccount.Id != userId)
        {
            throw new NotFoundUserAccountException(userId);
        }
    }
    
    private void ValidateTransactionId(Guid transactionId)
    {
        if (transactionId == Guid.Empty || !Guid.TryParse(transactionId.ToString(), out _))
        {
            throw new InvalidTransactionException();
        }
    }
    
    private void ValidateStorageTransaction(Transaction transaction, Guid transactionId)
    {
        if (transaction is null || transaction.Id != transactionId)
        {
            throw new NotFoundTransactionException(transactionId);
        }
    }
}