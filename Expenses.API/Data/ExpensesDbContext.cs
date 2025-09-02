using Expenses.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Expenses.API.Data;

public class ExpensesDbContext: DbContext
{
    public ExpensesDbContext(DbContextOptions<ExpensesDbContext> options): base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
}