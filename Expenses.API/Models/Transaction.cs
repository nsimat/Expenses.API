using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Expenses.API.Models.Base;

namespace Expenses.API.Models;

/// <summary>
/// Transaction entity representing a financial transaction, which can be either an income or an expense.
/// Each transaction is associated with a user and contains details such as type, amount, and category.
/// </summary>
public class Transaction: BaseEntity
{
    /// <summary>
    /// Type of the transaction, e.g., "income" or "expense"
    /// </summary>
    [StringLength(50, MinimumLength = 5, ErrorMessage = "Type must be between 5 and 50 characters long!")]
    public required string Type { get; set; }
    
    /// <summary>
    /// Amount of the transaction
    /// </summary>
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero!")]
    public double Amount { get; set; }
    
    /// <summary>
    /// Category of the transaction, e.g., "food", "salary", etc.
    /// </summary>
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Category must be between 3 and 100 characters long!")]
    public required string Category { get; set; }

    /// <summary>
    /// Unique ID of the user who creates the transaction
    /// </summary>
    [Required(ErrorMessage = "User ID is a required field!")]
    [Display(Name = "Creator User ID")]
    [Column("Creator User ID")]
    public Guid UserId { get; set; }
    
    /// <summary>
    /// The user entity used for navigation
    /// </summary>
    public UserAccount? User { get; set; }
}