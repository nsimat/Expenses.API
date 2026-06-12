namespace Expenses.API.DTOs.Transactions;

/// <summary>
/// A data transfer object for updating transaction details.
/// </summary>
public class TransactionUpdateRequest
{
    /// <summary>
    /// Type of the transaction, e.g., "income" or "expense"
    /// </summary>
    public required string Type { get; set; }
    
    /// <summary>
    /// Amount of the transaction
    /// </summary>
    public double Amount { get; set; }
    
    /// <summary>
    /// Category of the transaction, e.g., "food", "salary", etc.
    /// </summary>
    public required string Category { get; set; }
}