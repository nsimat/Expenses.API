using Xeptions;

namespace Expenses.API.Models.Exceptions.Transactions;

/// <summary>
/// This exception is thrown when an operation attempts to access or modify a transaction record that is currently
/// locked, indicating that the record is being used by another process or user. The message provides guidance to the
/// user to try the operation again later.
/// </summary>
public class LockedTransactionException : Xeption
{
    /// <summary>
    /// Initializes a new instance of the LockedTransactionException class with a specified inner exception that
    /// caused this exception to be thrown.
    /// </summary>
    /// <param name="innerException">The underlying cause of the caught error</param>
    public LockedTransactionException(Exception innerException) 
        : base(message: "Locked transaction record error occurred. Please try again later.", innerException)
    { }
}