using Xeptions;

namespace Expenses.API.Models.Exceptions.Transactions;

/// <summary>
/// The exception that is thrown when a transaction dependency validation error occurs, such as when a related entity
/// is not found or when there is a conflict with existing data. It indicates that the transaction cannot be processed
/// due to issues with its dependencies, and the user should try again after resolving the underlying issues.
/// </summary>
public class TransactionDependencyValidationException : Xeption
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TransactionDependencyValidationException"/> class with a specified
    /// inner exception.
    /// </summary>
    /// <param name="innerException">The underlying cause of the caught error.</param>
    public TransactionDependencyValidationException(Xeption innerException)
        : base(message: "Transaction dependency validation error occurred. Please try again.", innerException)
    { }
}