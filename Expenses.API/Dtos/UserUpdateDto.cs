namespace Expenses.API.Dtos;

/// <summary>
/// A DTO containing part of user data
/// </summary>
public class UserUpdateDto
{
    // The user email
    public string email { get; set; }
    
    // User first name
    public string FirstName { get; set; }
    
    // User last name
    public string LastName { get; set; }
}