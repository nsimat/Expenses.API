using Expenses.API.DTOs.UserAccounts;
using Microsoft.AspNetCore.Mvc;

namespace Expenses.API.Services.Foundations.Accounts;

/// <summary>
/// Represents the service contract for managing user accounts.
/// </summary>
/// <remarks>
/// This interface defines the operations related to user account management, including retrieval, creation,
/// identification, updating, and deletion of user accounts. It serves as a contract for implementing the
/// business logic associated with user accounts in the Expenses API.
/// </remarks>
public interface IUserAccountService
{
    /// <summary>
    /// Checks if an email is already registered in the storage system
    /// </summary>
    /// <param name="email">Email to check availability in the database</param>
    /// <returns>The user account associated with the given email, indicating that the email is already registered.
    /// Or null if no user account is found, indicating that the email is available for registration.
    /// </returns>
    ValueTask<UserAccountResponse> IsEmailAvailableAsync(string email);
    
    /// <summary>
    /// Adds a new user to the system
    /// </summary>
    /// <param name="userRegisterRequest">A DTO representing user's data</param>
    /// <returns>The resulting user account created by the creation operation</returns>
    ValueTask<UserAccountResponse> AddUserAccountAsync(UserRegisterRequest userRegisterRequest);
    
    /// <summary>
    /// Identifies the user by email and password. 
    /// </summary>
    /// <param name="userLogin">A DTO representing a user's credentials to identify</param>
    /// <returns>the user entity if found, or NULL otherwise.</returns>
    ValueTask<UserAccountResponse> IdentifyUserAccountAsync(UserLoginRequest userLogin);

    /// <summary>
    /// Gets the profile of a user from his email
    /// </summary>
    /// <param name="email">The unique email of a user</param>
    /// <returns>The specified user identified by the given email</returns>
    ValueTask<UserAccountResponse> RetrieveUserProfileAsync(string email);

    /// <summary>
    /// Updates the user data in the database
    /// </summary>
    /// <param name="userId">The unique ID of a user in the database</param>
    /// <param name="userUpdateRequest">A DTO containing modified user data</param>
    /// <returns>The newly modified data in the database</returns>
    ValueTask<UserAccountResponse> ModifyUserProfileAsync(Guid userId, [FromBody] UserUpdateRequest userUpdateRequest);
}