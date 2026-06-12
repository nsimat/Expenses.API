using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Expenses.API.Models.Base;
using Microsoft.EntityFrameworkCore;

namespace Expenses.API.Models;

/// <summary>
/// UserAccount entity representing an application user
/// </summary>
[Index(nameof(Email), IsUnique = true)]
public class UserAccount: BaseEntity
{
 /// <summary>
 /// Email of the user
 /// </summary>
 [EmailAddress]
 [StringLength(50, ErrorMessage = "Email must be at most 50 characters long!")]
 [Required(ErrorMessage = "Email is a required field!")]
 public required string Email { get; set; } = string.Empty;

 /// <summary>
 /// Stores the normalized version of the user's email address.
 /// This is typically used for case-insensitive comparisons or lookups.
 /// </summary>
 [StringLength(50, ErrorMessage = "Normalized email must be at most 50 characters long!")]
 [Required(ErrorMessage = "Normalized email is a required field!")]
 [Display(Name = "Normalized Email")]
 [Column("Normalized Email")]
 public required string NormalizedEmail { get; set; }
 
 /// <summary>
 /// Password of the user, stored as a hash for security reasons. The actual password should never be stored in plain
 /// text. The length requirement ensures that the password is of enough complexity to provide security, while also
 /// allowing for a reasonable maximum length to prevent excessively long passwords that could cause performance issues. 
 /// </summary>
 [StringLength(500, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 500 characters long!")]
 [Required(ErrorMessage = "Password is a required field!")]
 public required string PasswordHash { get; set; } // change type to byte[]???
 
/// <summary>
/// The first name of the user
/// </summary>
[StringLength(50, MinimumLength = 3, ErrorMessage = "First name must be between 3 and 50 characters long!")]
[RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "First name must contain only letters!")]
[Display(Name = "First Name")]
[Column("First Name")]
 public string? FirstName { get; set; }

/// <summary>
/// The last name of the user
/// </summary>
[StringLength(50, MinimumLength = 3, ErrorMessage = "Last name must be between 3 and 50 characters long!")]
[RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Last name must contain only letters!")]
[Display(Name = "Last Name")]
[Column("Last Name")] 
 public string? LastName { get; set; }

 /// <summary>
 /// Normalized representation of the user's full name combining first and last name.
 /// Typically used for consistency and searches.
 /// </summary>
 [StringLength(100, ErrorMessage = "Normalized full name must be at most 100 characters long!")]
 [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Normalized full name must contain only letters and spaces!")]
 [Display(Name = "Full Name")]
 [Column("Normalized Full Name")]
 public string? NormalizedFullName { get; set; }

/// <summary>
/// The date of birth of the user
/// </summary>
[Column("Date Of Birth")]
[Display(Name = "Date of Birth"), DataType(DataType.Date)]
[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
 public DateTimeOffset? DateOfBirth { get; set; }

 /// <summary>
 /// Indicates whether the user has administrative privileges.
 /// </summary>
 public bool IsAdmin { get; set; } = false;

 /// <summary>
 /// List of timestamps representing the most recent login times of the user.
 /// </summary>
 public List<DateTimeOffset>? RecentLoginTimes { get; set; }

 // Defines the navigation property for transactions associated with the user
 /// <summary>
 /// List of transactions associated with the user
 /// </summary>
 public ICollection<Transaction> Transactions { get; set; } = new HashSet<Transaction>();
}