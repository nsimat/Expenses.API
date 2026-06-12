using Xeptions;

namespace Expenses.API.Models.Exceptions.Transactions;

/// <summary>
/// The exception that is thrown when a transaction dependency error occurs, such as issues with the database or
/// external services that the transaction relies on. 
/// </summary>
public class TransactionDependencyException : Xeption
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TransactionDependencyException"/> class with a specified
    /// inner exception.
    /// </summary>
    /// <param name="innerException">The underlying cause of the caught error</param>
    public TransactionDependencyException(Xeption innerException) : 
        base(message: "Transaction dependency error occurred. Please contact support.", innerException)
    { }
}