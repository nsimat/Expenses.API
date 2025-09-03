using Expenses.API.Data;
using Expenses.API.Dtos;
using Expenses.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Expenses.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TransactionsController : ControllerBase
{
    private readonly ILogger<TransactionsController> _logger;
    private readonly ExpensesDbContext _expensesDbContext;

    public TransactionsController(ExpensesDbContext expensesDbContext, ILogger<TransactionsController> logger)
    {
        _expensesDbContext = expensesDbContext ?? throw new ArgumentNullException(nameof(expensesDbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    [HttpPost]
    public IActionResult CreateTransaction([FromBody] TransactionDto payload)
    {
        _logger.LogInformation("Creating a new transaction...");
        
        var newTransaction = new Transaction
        {
            Type = payload.Type,
            Amount = payload.Amount,
            Category = payload.Category,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        _expensesDbContext.Transactions.Add(newTransaction);
        _expensesDbContext.SaveChanges();
        
        return Ok("Transaction created!");
    }
}