namespace Expenses.API.Dtos;

/// <summary>
/// Strongly typed result class to inform the client of a login attempt result, sending it the JWT if successful.
/// </summary>
public class LoginResultDto
{
    /// <summary>
    /// TRUE if the login attempt is successful, FALSE otherwise.
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// Login attempt result message.
    /// </summary>
    public required string Message { get; set; }
    
    /// <summary>
    /// The JWT token generated for the user if the login attempt is successful, NULL otherwise.
    /// </summary>
    public string? Token { get; set; }
}