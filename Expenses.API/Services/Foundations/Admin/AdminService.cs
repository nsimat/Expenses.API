using AutoInject.Attributes.ScopedAttributes;
using AutoInject.Attributes.TransientAttributes;
using Expenses.API.Brokers.Loggings;
using Expenses.API.Brokers.Storages;
using Expenses.API.DTOs.Mapping;
using Expenses.API.DTOs.Transactions;
using Expenses.API.DTOs.UserAccounts;
using Expenses.API.Services.Foundations.Accounts;
using Microsoft.EntityFrameworkCore;

namespace Expenses.API.Services.Foundations.Admin;

/// <summary>
/// Provides implementation for administrative functionalities including management of user accounts
/// and retrieval of transaction data.
/// </summary>
[Scoped(typeof(IAdminService))]
public partial class AdminService : IAdminService
{
    private readonly IStorageBroker _storageBroker;
    private readonly ILoggingBroker _loggingBroker;

    public AdminService(IStorageBroker storageBroker, ILoggingBroker loggingBroker)
    {
        _storageBroker = storageBroker ?? throw new ArgumentNullException(nameof(storageBroker));
        _loggingBroker = loggingBroker ?? throw new ArgumentNullException(nameof(loggingBroker));
    }

    /// <inheritdoc/>
    public ValueTask<UserAccountResponse> RetrieveUserAccountAsync(Guid userId) =>
        TryCatch(async () =>
        {
            _loggingBroker.LogInformation($"Attempting to find user with ID: {userId}...");

            // Step 0. Validates the user ID used to retrieve the user account.
            // If the user ID is invalid (e.g., empty or not a valid GUID), it throws an InvalidUserAccountException.
            ValidateUserAccountId(userId);

            // Step 1. Retrieves the user account from the database using the provided user ID.
            // If no user account is found, it throws a NotFoundUserAccountException.
            // If a user account is found, it returns the user account details as a UserAccountDto.
            var maybeUserAccount = await _storageBroker.SelectUserAccountByIdAsync(userId);

            // Step 2. Validates the retrieved user account.
            // If the user account is null, it throws a NotFoundUserAccountException.
            // If the user account is found but has invalid data, it throws an InvalidUserAccountException.    
            ValidateStorageUserAccount(maybeUserAccount, userId);

            // Step 3. Returns the user account details as a UserAccountDto if the retrieval and validation are successful.
            return maybeUserAccount.ToUserAccountDto();
        });

    /// <summary>
    /// Retrieves all user accounts with pagination support.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve starting from 1.</param>
    /// <param name="pageSize">The number of user accounts to include in a single page.</param>
    /// <returns>
    /// An <see cref="IQueryable{T}"/> collection of <see cref="UserAccountResponse"/> instances representing the user
    /// accounts.
    /// </returns>
    public IQueryable<UserAccountResponse> RetrieveAllUserAccounts(int pageNumber, int pageSize) =>
        TryCatch(() =>
        {
            _loggingBroker.LogInformation(
                @"Retrieving user accounts from the database with pagination with page Number: 
               {pageNumber} & page size: {pageSize}...");

            var skip = (pageNumber - 1) * pageSize;

            return _storageBroker
                .SelectAllUserAccounts()
                .AsNoTracking() 
                .Skip(skip)
                .Take(pageSize)
                .ToUserAccountDtos();
        });

    /// <summary>
    /// Deletes a user account based on the specified user account ID.
    /// </summary>
    /// <param name="userAccountId">The unique identifier of the user account to be deleted.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the deleted user account details.
    /// </returns>
    public ValueTask<UserAccountResponse> DeleteUserAccountAsync(Guid userAccountId) =>
        TryCatch(async () =>
        {
                _loggingBroker.LogInformation($"Attempting to delete user account with ID: {userAccountId}...");

                // Step 0. Validates the user account ID used to delete the user account.
                // If the user account ID is invalid (e.g., empty or not a valid GUID), it throws an InvalidUserAccountException.
                ValidateUserAccountId(userAccountId);

                // Step 1. Retrieves the user account from the database using the provided user account ID.
                // If no user account is found, it throws a NotFoundUserAccountException.
                var maybeUserAccount = await _storageBroker.SelectUserAccountByIdAsync(userAccountId);

                // Step 2. Validates the retrieved user account.
                // If the user account is null, it throws a NotFoundUserAccountException.
                // If the user account is found but has invalid data, it throws an InvalidUserAccountException.    
                ValidateStorageUserAccount(maybeUserAccount, userAccountId);

                // Step 3. Deletes the user account from the database if the retrieval and validation are successful.
                var deletedUserAccount = await _storageBroker.DeleteUserAccountAsync(maybeUserAccount);

                // Step 4. Returns the details of the deleted user account as a UserAccountDto if the deletion is successful.
                return deletedUserAccount.ToUserAccountDto();
        });


    public async ValueTask<UserAccountResponse> LockUserAccountAsync(Guid userAccountId)
    {
        throw new NotImplementedException();
    }

    public async ValueTask<UserAccountResponse> UnlockUserAccountAsync(Guid userAccountId)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Retrieves all transactions from the database with pagination support.
    /// </summary>
    /// <param name="pageNumber">The number of the page to retrieve, starting from 1.</param>
    /// <param name="pageSize">The number of transactions per page.</param>
    /// <returns>
    /// An <see cref="IQueryable{TransactionDto}"/> containing the transactions on the specified page.
    /// </returns>
    public IQueryable<TransactionResponse> RetrieveAllTransactions(int pageNumber, int pageSize) =>
        TryCatch(() =>
        {
            _loggingBroker.LogInformation(
                @"Retrieving transactions from the database with pagination with page Number: {pageNumber} & page size: 
              {pageSize}...");

            var skip = (pageNumber - 1) * pageSize;
            return _storageBroker.SelectAllTransactions()
                .AsNoTracking()
                .Skip(skip)
                .Take(pageSize)
                .ToTransactionDtos();
        });

    /// <summary>
    /// Retrieves a specific transaction by its unique identifier.
    /// </summary>
    /// <param name="transactionId">The unique identifier of the transaction to retrieve.</param>
    /// <returns>A <see cref="TransactionResponse"/> representing the details of the retrieved transaction.</returns>
    public ValueTask<TransactionResponse> RetrieveTransactionByIdAsync(Guid transactionId) =>
        TryCatch(async () =>
        {
            _loggingBroker.LogInformation($"Attempting to retrieve transaction with ID: {transactionId}...");

            // Step 0. Validates the given transaction ID used to retrieve the transaction.
            // If the transaction ID is invalid (e.g., empty or not a valid GUID), it throws an InvalidTransactionException.
            ValidateTransactionId(transactionId);

            // Step 1. Retrieves the transaction from the database using the provided transaction ID.
            var maybeTransaction = await _storageBroker.SelectTransactionByIdAsync(transactionId);

            // Step 2. Validates the retrieved transaction. 
            ValidateStorageTransaction(maybeTransaction, transactionId);

            // Step 3. Returns the details of the retrieved transaction as a TransactionDto if the retrieval and validation
            // are successful. 
            return maybeTransaction.ToTransactionDto();
        });
}