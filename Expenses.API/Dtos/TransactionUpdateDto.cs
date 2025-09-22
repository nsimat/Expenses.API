namespace Expenses.API.Dtos;

public class TransactionUpdateDto
{
    public string Type { get; set; }
    public double Amount { get; set; }
    public string Category { get; set; }
}