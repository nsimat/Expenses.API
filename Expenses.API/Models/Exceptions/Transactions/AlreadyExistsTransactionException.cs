using Xeptions;

namespace Expenses.API.Models.Exceptions.Transactions;

/// <summary>
/// The exception that is thrown when a transaction with the same ID already exists in the storage, indicating
/// a conflict during creation or update operations.
/// </summary>
public class AlreadyExistsTransactionException : Xeption
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AlreadyExistsTransactionException"/> class with a specified
    /// inner exception.
    /// </summary>
    /// <param name="innerException">The underlying cause of the caught error</param>
    public AlreadyExistsTransactionException(Exception innerException)
        : base(message: "Transaction with the same ID already exists.", innerException)
    { }
}