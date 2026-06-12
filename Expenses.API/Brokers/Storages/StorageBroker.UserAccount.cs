using Expenses.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Expenses.API.Brokers.Storages;

/// <summary>
/// Class responsible for interacting with the database using Entity Framework Core.
/// </summary>
public partial class StorageBroker 
{
    /// <summary>
    /// DbSet representing the collection of User entities within the storage context.
    /// </summary>
    public DbSet<UserAccount> UserAccounts { get; set; }

    /// <inheritdoc/>
    public IQueryable<UserAccount> SelectAllUserAccounts() => SelectAll<UserAccount>();

    /// <inheritdoc/>
    public async ValueTask<UserAccount?> SelectUserAccountByIdAsync(Guid userAccountId) 
        => await SelectAsync<UserAccount>(userAccountId);
    
    /// <inheritdoc/>
    public async ValueTask<UserAccount?> SelectUserAccountByEmailAsync(string userAccountEmail) 
        => await SelectAll<UserAccount>().FirstOrDefaultAsync(u => u.Email == userAccountEmail);

    /// <inheritdoc/>
    public async ValueTask<UserAccount> InsertUserAccountAsync(UserAccount userAccount) 
        => await InsertAsync(userAccount);
    
    /// <inheritdoc/>
    public async ValueTask<UserAccount> UpdateUserAccountAsync(UserAccount userAccount) 
        => await UpdateAsync(userAccount);
    
    /// <inheritdoc/>
    public async ValueTask<UserAccount> DeleteUserAccountAsync(UserAccount userAccount) 
        => await DeleteAsync(userAccount);
}