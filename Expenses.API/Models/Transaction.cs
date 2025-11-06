using Expenses.API.Models.Base;

namespace Expenses.API.Models;

public class Transaction: BaseEntity
{
    /// <summary>
    /// Type of the transaction, e.g., "income" or "expense"
    /// </summary>
    public string Type { get; set; }
    
    /// <summary>
    /// Amount of the transaction
    /// </summary>
    public double Amount { get; set; }
    
    /// <summary>
    /// Category of the transaction, e.g., "food", "salary", etc.
    /// </summary>
    public string Category { get; set; }

    /// <summary>
    /// Unique ID of the user who creates the transaction
    /// </summary>
    public int? UserId { get; set; }
    
    /// <summary>
    /// The user entity used for navigation
    /// </summary>
    public virtual User? User { get; set; }
}