using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Expenses.API.DTOs.UserAccounts;

/// <summary>
/// This DTO aims to receive the user's credentials for account creation from the client.
/// </summary>
public class UserRegisterRequest
{
    /// <summary>
    /// Email to be used as a username must be in a valid email format
    /// </summary>
    [EmailAddress(ErrorMessage = "Invalid email format!")]
    [Required(ErrorMessage = "Email is a required field!")]
    [JsonPropertyName("email")]
    public required string Email { get; set; } = string.Empty;

    /// <summary>
    /// Password to be used for authentication must be at least 6 characters long
    /// </summary>
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long!")]
    [Required(ErrorMessage = "Password is a required field!")]
    [JsonPropertyName("password")]
    public required string Password { get; set; } = string.Empty;

    /// <summary>
    /// A required field intended to confirm the user's password during the account creation process. The value must
    /// match the Password field.
    /// </summary>
    [Compare("Password", ErrorMessage = "Password confirmation does not match the password!")]
    [JsonPropertyName("password_confirm")]
    public required string PasswordConfirm { get; set; } = string.Empty;
}