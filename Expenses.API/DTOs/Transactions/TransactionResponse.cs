namespace Expenses.API.DTOs.Transactions;

/// <summary>
/// DTO representing a transaction, used for transferring transaction data between layers of the application and to
/// clients. It includes properties such as Id, Type, Amount, Category, UserId, CreatedAt, and UpdatedAt to encapsulate
/// all relevant information about a transaction. 
/// </summary>
public class TransactionResponse
{
    public required Guid Id { get; set; } = Guid.Empty;
    public string Type { get; set; } = string.Empty;
    public double Amount { get; set; } = double.NaN;
    public string Category { get; set; } =  string.Empty;
    public Guid UserId { get; set; } =  Guid.Empty;
    public DateTimeOffset CreatedAt { get; set; } =  DateTimeOffset.MinValue;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.MinValue;
}