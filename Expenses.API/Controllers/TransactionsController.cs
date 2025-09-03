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

    [HttpGet("All")]
    public IActionResult GetAllTransactions()
    {
        _logger.LogInformation("Fetching all transactions...");
        
        var transactions = _expensesDbContext.Transactions.ToList();
        return Ok(transactions);
    }

    [HttpGet("Details/{id}")]
    public IActionResult GetTransactionById(int id)
    {
        _logger.LogInformation("Fetching transaction with ID: {Id}...", id);
        
        var transaction = _expensesDbContext.Transactions.Find(id);
        
        if (transaction == null)
            return NotFound("Transaction not found!");
        
        return Ok(transaction);
    }
    
    [HttpPost("Create")]
    public IActionResult CreateTransaction([FromBody] PostTransactionDto payload)
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
    
    [HttpPut("Update/{id}")]
    public IActionResult UpdateTransaction(int id, [FromBody] PutTransactionDto payload)
    {
        _logger.LogInformation("Updating transaction with ID: {Id}...", id);
        
        var existingTransaction = _expensesDbContext.Transactions.Find(id);
        
        if (existingTransaction == null)
            return NotFound("Transaction not found!");
        
        existingTransaction.Type = payload.Type;
        existingTransaction.Amount = payload.Amount;
        existingTransaction.Category = payload.Category;
        existingTransaction.UpdatedAt = DateTime.UtcNow;

        _expensesDbContext.Update(existingTransaction);
        _expensesDbContext.SaveChanges();
        
        return Ok("Transaction updated!");
    }

    [HttpDelete("Delete/{id}")]
    public IActionResult DeleteTransaction(int id)
    {
        _logger.LogInformation("Deleting transaction with ID: {Id}...", id);
        var existingTransaction = _expensesDbContext.Transactions.Find(id);
        
        if (existingTransaction == null)
            return NotFound("Transaction not found!");
        
        _expensesDbContext.Transactions.Remove(existingTransaction);
        _expensesDbContext.SaveChanges();
        
        return Ok("Transaction deleted!");
    }
}