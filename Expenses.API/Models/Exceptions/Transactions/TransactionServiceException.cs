using Xeptions;

namespace Expenses.API.Models.Exceptions.Transactions;

/// <summary>
/// This exception is thrown when an error occurs within the Transaction Service, indicating a failure in processing
/// transactions. It serves as a wrapper for underlying exceptions, providing a user-friendly message while preserving
/// the original exception details for debugging purposes.
/// </summary>
public class TransactionServiceException : Xeption
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TransactionServiceException"/> class with a specified inner exception.
    /// </summary>
    /// <param name="innerException">The underlying cause of the caught error</param>
    public TransactionServiceException(Xeption innerException)
    : base(message: "Transaction service error occurred, please contact support.", innerException)
    { }
}