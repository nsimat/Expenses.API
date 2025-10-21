namespace Expenses.API.Dtos;

public class TransactionForCreationDto
{
    /// <summary>
    /// Type of the transaction (e.g., "income", "expense").
    /// </summary>
    public string Type { get; set; }
    
    /// <summary>
    /// Amount of the transaction.  
    /// </summary>
    public double Amount { get; set; }
    
    /// <summary>
    /// Category of the transaction (e.g., "food", "salary").
    /// </summary>
    public string Category { get; set; }
    
    /// <summary>
    /// Date and time when the transaction was created.
    /// </summary>
    public DateTime? CreatedAt { get; set; }
}