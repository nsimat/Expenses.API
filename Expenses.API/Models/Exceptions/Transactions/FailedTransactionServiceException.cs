using Xeptions;

namespace Expenses.API.Models.Exceptions.Transactions;

/// <summary>
/// This exception is thrown when a transaction service operation fails, indicating an issue that requires attention
/// and support. It encapsulates the underlying exception that caused the failure, providing a clear message for
/// troubleshooting.
/// </summary>
public class FailedTransactionServiceException : Xeption
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FailedTransactionServiceException"/> class with a specified
    /// inner exception.
    /// </summary>
    /// <param name="innerException">The underlying cause of the caught error</param>
    public FailedTransactionServiceException(Exception innerException)
        : base(message: "Failed transaction service error occurred, please contact support.", innerException)
    { }
}