using System.ComponentModel.DataAnnotations;

namespace Expenses.API.Dtos;

/// <summary>
/// This DTO aims to receive the user's credentials for account creation from the client.
/// </summary>
public class UserCreateDto
{
    /// <summary>
    /// Email to be used as a username must be in a valid email format
    /// </summary>
    [EmailAddress(ErrorMessage = "Invalid email format!")]
    [Required(ErrorMessage = "Email is required.")]
    public required string Email { get; set; }
    
    /// <summary>
    /// Password to be used for authentication, must be at least 6 characters long
    /// </summary>
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long!")]
    [Required(ErrorMessage = "Password is required.")]
    public required string Password { get; set; }
}