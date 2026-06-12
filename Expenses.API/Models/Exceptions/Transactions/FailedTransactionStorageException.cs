using Xeptions;

namespace Expenses.API.Models.Exceptions.Transactions;

/// <summary>
/// The exception that is thrown when there is an error while trying to store a transaction in the database.
/// </summary>
public class FailedTransactionStorageException : Xeption
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FailedTransactionStorageException"/> class with a specified
    /// inner exception.
    /// </summary>
    /// <param name="innerException">The underlying cause of the caught error</param>
    public FailedTransactionStorageException(Exception innerException)
        : base(message: "Failed transaction storage error occurred, contact support.", innerException)
    {
    }
}