using Xeptions;

namespace Expenses.API.Models.Exceptions.Transactions;

/// <summary>
/// The exception that is thrown when a transaction is null, indicating that the expected transaction object was not
/// provided or found. This exception is used to signal issues related to null references in transaction operations,
/// such as when attempting to retrieve, update, or delete a transaction that does not exist in the storage.
/// </summary>
public class NullTransactionException : Xeption
{
    /// <summary>
    /// Initializes a new instance of the NullTransactionException with a default error message indicating that
    /// the transaction is null.
    /// </summary>
    public NullTransactionException() : base(message: "Transaction object is null.")
    {}
}