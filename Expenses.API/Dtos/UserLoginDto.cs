using System.ComponentModel.DataAnnotations;

namespace Expenses.API.Dtos;

/// <summary>
/// A DTO to receive user credentials for login from the client.
/// </summary>
public class UserLoginDto
{
    /// <summary>
    /// A valid email address is required for username authentication.
    /// </summary>
    [EmailAddress(ErrorMessage = "Invalid email format!")]
    [Required(ErrorMessage = "Email is required.")]
    public required string Email { get; set; }
    
    /// <summary>
    /// A password with a minimum length of 6 characters is required for password authentication.
    /// </summary>
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long!")]
    [Required(ErrorMessage = "Password is required.")]
    public required string Password { get; set; }
}