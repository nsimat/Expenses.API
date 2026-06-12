using Expenses.API.Models;

namespace Expenses.API.Brokers.Storages;

/// <summary>
/// Represents a contract for storage operations.
/// </summary>
/// <remarks>
/// This interface defines the methods for interacting with the storage layer of the application, specifically for user
/// accounts and transactions. It includes methods for selecting, inserting, updating, and deleting user accounts and
/// transactions in the storage system.
/// </remarks>  
public partial interface IStorageBroker
{
    /// <summary>
    /// Inserts a new user account into the storage
    /// </summary>
    /// <param name="userAccount">The user account to be inserted</param>
    /// <returns>The inserted user</returns>
    ValueTask<UserAccount> InsertUserAccountAsync(UserAccount userAccount);
    
    /// <summary>
    /// Retrieves all user's accounts from the storage
    /// </summary>
    /// <returns>A list of all users</returns>
    IQueryable<UserAccount> SelectAllUserAccounts();

    /// <summary>
    /// Retrieves a user by their ID from the storage
    /// </summary>
    /// <param name="userAccountId">The user account ID to search for</param>
    /// <returns>The user with the specified ID, or null if not found</returns>
    ValueTask<UserAccount?> SelectUserAccountByIdAsync(Guid userAccountId);

    /// <summary>
    /// Retrieves a user account by their email address from the storage.
    /// </summary>
    /// <param name="userAccountEmail">The email address of the user account to search for.</param>
    /// <returns>The user account with the specified email address, or null if not found.</returns>
    ValueTask<UserAccount?> SelectUserAccountByEmailAsync(string userAccountEmail);

    /// <summary>
    /// Updates an existing user account in the storage
    /// </summary>
    /// <param name="userAccount">The user account to be updated</param>
    /// <returns>The updated user</returns>
    ValueTask<UserAccount> UpdateUserAccountAsync(UserAccount userAccount);
    
    /// <summary>
    /// Deletes a user from the storage
    /// </summary>
    /// <param name="userAccount">The user to be deleted</param>
    /// <returns>The deleted user</returns>
    ValueTask<UserAccount> DeleteUserAccountAsync(UserAccount userAccount);
}