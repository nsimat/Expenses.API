namespace Expenses.API.DTOs.UserAccounts;

/// <summary>
/// Strongly typed result class to inform the client of a login attempt result, sending it the JWT if successful.
/// </summary>
public class UserLoginResponse
{
    /// <summary>
    /// Boolean variable indicating the success of the login attempt.
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// Login attempt result message.
    /// </summary>
    public required string Message { get; set; }
    
    /// <summary>
    /// The JWT token generated for the user if the login attempt is successful, or NULL otherwise.
    /// </summary>
    public string? Token { get; set; }
}