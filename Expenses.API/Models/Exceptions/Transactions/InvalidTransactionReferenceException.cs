using Xeptions;

namespace Expenses.API.Models.Exceptions.Transactions;

/// <summary>
/// This exception is thrown when an invalid transaction reference is encountered, indicating that the provided
/// reference does not correspond to any existing transaction. This could occur due to a typo, an outdated reference,
/// or an attempt to access a transaction that has been deleted. 
/// </summary>
public class InvalidTransactionReferenceException : Xeption
{
    /// <summary>
    /// Initializes a new instance of the InvalidTransactionReferenceException class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="innerException">The underlying cause of the caught error</param>
    public InvalidTransactionReferenceException(Exception innerException)
        : base("Invalid transaction reference error occurred, contact support.", innerException)
    { }
}