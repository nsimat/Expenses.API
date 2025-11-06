using Expenses.API.Dtos;
using Expenses.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Expenses.API.Data.Services;

/// <summary>
/// Interface for account-related operations and user management
/// </summary>
public interface IAccountService
{
    /// <summary>
    /// Find user by email and password
    /// </summary>
    /// <param name="apiLoginRequest">A DTO representing user's credentials</param>
    /// <returns>The user represents by the email entered if found, or NULL otherwise</returns>
    Task<User?> FindUserAsync(ApiLoginRequestDto apiLoginRequest);
    
    /// <summary>
    /// checks if an email is already registered in the system
    /// </summary>
    /// <param name="email">Email to check availability in the database</param>
    /// <returns>True if email found already in the system, or false otherwise</returns>
    Task<bool> IsEmailAvailableAsync(string email);
    
    /// <summary>
    /// Add a new user to the system
    /// </summary>
    /// <param name="userCreateDto">A DTO representing user's data</param>
    /// <returns>A DTO containing the result of the creation operation</returns>
    Task<ApiLoginResultDto> AddUserAsync(UserCreateDto userCreateDto);
    
    /// <summary>
    /// Identify user by email and password, 
    /// </summary>
    /// <param name="apiLoginRequest">A DTO representing user's credentials to identify</param>
    /// <returns>the user entity if found, or NULL otherwise.</returns>
    Task<ApiLoginResultDto> IdentifyUserAsync(ApiLoginRequestDto apiLoginRequest);

    /// <summary>
    /// Obtain the profile of a user from his email
    /// </summary>
    /// <param name="email">The unique email of a user</param>
    /// <returns>The specified user identified by the given email</returns>
    Task<User?> GetUserProfile(string email);

    /// <summary>
    /// Updates the user data in the database
    /// </summary>
    /// <param name="id">The unique ID of a user in the database</param>
    /// <param name="userUpdateDto">A DTO containing modified user data</param>
    /// <returns>The newly modified data in the database</returns>
    Task<User?> UpdateUserProfile(int id, [FromBody] UserUpdateDto userUpdateDto);
}