namespace Expenses.API.Dtos;

public class TransactionCreateDto
{
    public string Type { get; set; }
    public double Amount { get; set; }
    public string Category { get; set; }
    public DateTime? CreatedAt { get; set; }
}