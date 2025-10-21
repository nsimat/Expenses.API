using Expenses.API.Dtos;
using Expenses.API.Models;

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
    /// <param name="email"></param>
    /// <returns>True if email found already in the system, or false otherwise</returns>
    Task<bool> IsEmailAvailableAsync(string email);
    
    /// <summary>
    /// Add a new user to the system
    /// </summary>
    /// <param name="userCreationDto">A DTO representing user's data</param>
    /// <returns>A DTO containing the result of the creation operation</returns>
    Task<ApiLoginResultDto> AddUserAsync(UserCreationDto userCreationDto);
    
    /// <summary>
    /// Identify user by email and password, 
    /// </summary>
    /// <param name="apiLoginRequest">A DTO representing user's credentials to identify</param>
    /// <returns>the user entity if found, or NULL otherwise.</returns>
    Task<ApiLoginResultDto> IdentifyUserAsync(ApiLoginRequestDto apiLoginRequest);
}