using System.ComponentModel.DataAnnotations;

namespace Expenses.API.Dtos;

/// <summary>
/// This DTO aims to receive the user's credentials for account creation from the client.
/// </summary>
public class UserCreationDto
{
    [EmailAddress(ErrorMessage = "Invalid email format!")]
    [Required(ErrorMessage = "Email is required.")]
    public required string Email { get; set; }
    
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long!")]
    [Required(ErrorMessage = "Password is required.")]
    public required string Password { get; set; }
}