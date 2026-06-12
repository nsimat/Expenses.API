using AutoInject.Attributes.TransientAttributes;
using EFxceptions;
using Microsoft.EntityFrameworkCore;

namespace Expenses.API.Brokers.Storages;

/// <summary>
/// Class responsible for interacting with the database using Entity Framework Core.
/// </summary>
[Transient(typeof(IStorageBroker))]
public partial class StorageBroker: EFxceptionsContext, IStorageBroker
{
    /// <summary>
    /// Represents the application's configuration settings.
    /// Used to access configuration, such as connection strings, at runtime.
    /// </summary>
    private readonly IConfiguration _configuration;
    
    /// <summary>
    /// Constructor for the StorageBroker class
    /// </summary>
    /// <param name="configuration">Configuration settings for the application</param>
    public StorageBroker(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Configures the entity models and establishes relationships between them
    /// as part of the Entity Framework Core model building process.
    /// </summary>
    /// <param name="modelBuilder">Provides a simple API surface for configuring the model for the context.</param>
    protected  override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);        
    }

    /// <summary>
    /// Configures the database context with specific options such as connection string and retry logic.
    /// </summary>
    /// <param name="optionsBuilder">A builder used to configure database context options.</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = _configuration.GetConnectionString("ExpensesApiConnection");
        
        optionsBuilder.UseSqlServer(connectionString);
    }
    
    /// <summary>
    /// Inserts an object asynchronously into the database.
    /// </summary>
    /// <typeparam name="T">The type of the object to be inserted.</typeparam>
    /// <param name="object">The object instance to be inserted.</param>
    /// <returns>The inserted object.</returns>
    private async ValueTask<T> InsertAsync<T>(T @object)
    {        
        this.Entry(@object).State = EntityState.Added;
        await this.SaveChangesAsync();

        return @object;
    }

    /// <summary>
    /// Retrieves all entities of type T from the database.
    /// </summary>
    /// <typeparam name="T">The type of entity to retrieve.</typeparam>
    /// <returns>An IQueryable representing all entities of the specified type.</returns>
    private IQueryable<T> SelectAll<T>() where T : class
    {
        return this.Set<T>();
    }

    /// <summary>
    /// Retrieves an entity of the specified type using the provided object identifiers.
    /// </summary>
    /// <typeparam name="T">The type of the entity to retrieve.</typeparam>
    /// <param name="objectIds">The identifiers of the entity to be retrieved.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the retrieved entity.</returns>
    private async ValueTask<T?> SelectAsync<T>(params object[] objectIds) where T : class
    {
        return await this.FindAsync<T>(objectIds);
    }

    /// <summary>
    /// Updates the specified object in the storage and saves changes asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the object to be updated.</typeparam>
    /// <param name="object">The object to be updated in the storage.</param>
    /// <returns>Returns the updated object.</returns>
    private async ValueTask<T> UpdateAsync<T>(T @object)
    {
        this.Entry(@object).State |= EntityState.Modified;
        await this.SaveChangesAsync();

        return @object;
    }

    /// <summary>
    /// Deletes the specified object asynchronously from the database.
    /// </summary>
    /// <typeparam name="T">The type of the object to delete.</typeparam>
    /// <param name="object">The instance of the object to delete.</param>
    /// <returns>The deleted object.</returns>
    private async ValueTask<T> DeleteAsync<T>(T @object)
    {
        this.Entry(@object).State = EntityState.Deleted;
        await this.SaveChangesAsync();

        return @object;
    }
}