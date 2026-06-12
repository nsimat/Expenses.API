using Xeptions;

namespace Expenses.API.Models.Exceptions.Transactions;

/// <summary>
/// The exception that is thrown when a transaction with a specified identifier is not found.
/// </summary>
public class NotFoundTransactionException : Xeption
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundTransactionException"/> class with a specified transaction
    /// ID that was not found. 
    /// </summary>
    /// <param name="transactionId">The underlying cause of the caught error</param>
    public NotFoundTransactionException(Guid transactionId)
        : base(message: $"Couldn't find transaction with ID: {transactionId}.")
    { }
}