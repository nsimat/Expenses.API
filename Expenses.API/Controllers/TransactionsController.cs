using Expenses.API.Data;
using Expenses.API.Data.Services;
using Expenses.API.Dtos;
using Expenses.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Expenses.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TransactionsController : ControllerBase
{
    private readonly ILogger<TransactionsController> _logger;
    private readonly ITransactionsService _transactionsService;

    public TransactionsController(ITransactionsService transactionsService, ILogger<TransactionsController> logger)
    {
        _transactionsService = transactionsService ?? throw new ArgumentNullException(nameof(transactionsService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet("All")]
    public IActionResult GetAllTransactions()
    {
        _logger.LogInformation("Fetching all transactions...");
        return Ok(_transactionsService.GetAll());
    }

    [HttpGet("Details/{id}")]
    public IActionResult GetTransactionById(int id)
    {
        _logger.LogInformation("Fetching transaction with ID: {Id}...", id);
        var transaction = _transactionsService.GetById(id);
        return transaction != null ? Ok(transaction) : NotFound("Transaction not found!");
    }
    
    [HttpPost("Create")]
    public IActionResult CreateTransaction([FromBody] PostTransactionDto payload)
    {
        _logger.LogInformation("Creating a new transaction...");
        _transactionsService.Add(payload);
        return Ok("Transaction created!");
    }
    
    [HttpPut("Update/{id}")]
    public IActionResult UpdateTransaction(int id, [FromBody] PutTransactionDto payload)
    {
        _logger.LogInformation("Updating transaction with ID: {Id}...", id);
        
        return _transactionsService.Update(id, payload) != null ?
            Ok("Transaction updated!") : NotFound("Transaction not found!");
    }

    [HttpDelete("Delete/{id}")]
    public IActionResult DeleteTransaction(int id)
    {
        _logger.LogInformation("Deleting transaction with ID: {Id}...", id);
        _transactionsService.Delete(id);
        return Ok("Transaction deleted!");
    }
}