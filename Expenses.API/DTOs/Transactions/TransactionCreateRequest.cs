using System.ComponentModel.DataAnnotations;

namespace Expenses.API.DTOs.Transactions;

/// <summary>
/// Class representing the data transfer object for creating a new transaction. It contains the necessary
/// properties to create a transaction, such as type, amount, category, and an optional createdAt timestamp.
/// </summary>
public class TransactionCreateRequest
{
    /// <summary>
    /// Type of the transaction (e.g., "income", "expense").
    /// </summary>
    public required string Type { get; set; }
    
    /// <summary>
    /// Amount of the transaction.  
    /// </summary>
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero!")]
    public double Amount { get; set; }
    
    /// <summary>
    /// Category of the transaction (e.g., "food", "salary").
    /// </summary>
    public required string Category { get; set; }
    
    /// <summary>
    /// Date and time when the transaction was created.
    /// </summary>
    public DateTimeOffset? CreatedAt { get; set; }
}