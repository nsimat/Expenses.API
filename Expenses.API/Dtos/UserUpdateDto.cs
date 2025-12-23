using System.ComponentModel.DataAnnotations;

namespace Expenses.API.Dtos;

/// <summary>
/// A DTO containing part of user data
/// </summary>
public class UserUpdateDto
{
    /// <summary>
    /// The email of the user
    /// </summary>
    [EmailAddress(ErrorMessage = "Invalid email format!")]
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// The first name of the user
    /// </summary>
    [MinLength(3, ErrorMessage = "First name must be at least 3 characters long!")]
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// The last name of the user
    /// </summary>
    [MinLength(3, ErrorMessage = "Last name must be at least 3 characters long!")]
    public string LastName { get; set; } = string.Empty;
}