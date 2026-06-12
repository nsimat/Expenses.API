using Xeptions;

namespace Expenses.API.Models.Exceptions.Transactions;

/// <summary>
/// The exception that is thrown when a transaction validation error occurs.
/// This exception is used to indicate that the ModelState of a transaction is invalid and requires corrections.
/// </summary>
public class TransactionValidationException : Xeption
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FailedTransactionStorageException"/> object with a specified
    /// inner exception and a message that describes the error.
    /// </summary>
    /// <param name="innerException">The underlying cause of the caught error</param>
    public TransactionValidationException(Xeption innerException)
         : base(message : "Transaction validation errors occurred. Please check your input and try again.", innerException)
    { }
}