using Expenses.API.DTOs.Transactions;
using Expenses.API.DTOs.UserAccounts;

namespace Expenses.API.Services.Foundations.Admin;

/// <summary>
/// Provides administrative functionalities for managing user accounts.
/// </summary>
public interface IAdminService
{
 /// <summary>
 /// Find the user account by its unique identifier
 /// </summary>
 /// <param name="userId">The unique user account identifier</param>
 /// <returns>The user account represented by the identifier entered if found, or NULL otherwise</returns>
 ValueTask<UserAccountResponse> RetrieveUserAccountAsync(Guid userId);

 /// <summary>
 /// Retrieve a paginated collection of all user accounts.
 /// </summary>
 /// <param name="pageNumber">The page number to retrieve.</param>
 /// <param name="pageSize">The number of user accounts to include in a single page.</param>
 /// <returns>An IQueryable collection of user account data transfer objects.</returns>
 IQueryable<UserAccountResponse> RetrieveAllUserAccounts(int pageNumber, int pageSize);

 /// <summary>
 /// Removes a user profile by the specified user ID.
 /// </summary>
 /// <param name="userAccountId">The unique identifier of the user whose profile is to be removed.</param>
 /// <returns>The user account details of the removed profile if successful, or NULL if the profile does not exist.</returns>
 ValueTask<UserAccountResponse> DeleteUserAccountAsync(Guid userAccountId);

 /// <summary>
 /// Locks a user account to prevent further access.
 /// </summary>
 /// <param name="userAccountId">The unique identifier of the user account to be locked.</param>
 /// <returns>
 /// The locked user account data transfer object if the lock operation is successful, or NULL otherwise.
 /// </returns>
 ValueTask<UserAccountResponse> LockUserAccountAsync(Guid userAccountId);

 /// <summary>
 /// Unlocks a user account that was previously locked.
 /// </summary>
 /// <param name="userAccountId">The unique identifier of the user account to be unlocked.</param>
 /// <returns>The updated user account details after being unlocked.</returns>
 ValueTask<UserAccountResponse> UnlockUserAccountAsync(Guid userAccountId);

 /// <summary>
 /// Retrieves all transactions with pagination support.
 /// </summary>
 /// <param name="pageNumber">The page number for paginated results.</param>
 /// <param name="pageSize">The number of transactions to retrieve per page.</param>
 /// <returns>An <see cref="IQueryable{T}"/> containing the transactions for the specified page and size.</returns>
 IQueryable<TransactionResponse> RetrieveAllTransactions(int pageNumber, int pageSize);

 /// <summary>
 /// Find the transaction by its unique identifier.
 /// </summary>
 /// <param name="transactionId">The unique transaction identifier.</param>
 /// <returns>The transaction represented by the identifier entered if found, or NULL otherwise.</returns>
 ValueTask<TransactionResponse> RetrieveTransactionByIdAsync(Guid transactionId);
}