using System.ComponentModel.DataAnnotations;
using Expenses.API.Models.Base;

namespace Expenses.API.Models;

/// <summary>
/// User entity representing an application user
/// </summary>
public class User: BaseEntity
{
 /// <summary>
 /// Email of the user
 /// </summary>
 [EmailAddress]
 [Required]
 public string Email { get; set; }
 
 /// <summary>
 /// Password of the user
 /// </summary>
 [Required]
 public string Password { get; set; }
 
/// <summary>
/// The first name of the user
/// </summary>
 [MinLength(3)]
 public string? FirstName { get; set; }

/// <summary>
/// The last name of the user
/// </summary>
 [MinLength(3)]
 public string? LastName { get; set; }

/// <summary>
/// List of transactions associated with the user
/// </summary>
 public List<Transaction> Transactions { get; set; }
}