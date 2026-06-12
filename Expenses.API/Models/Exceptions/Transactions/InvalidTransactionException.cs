using Xeptions;

namespace Expenses.API.Models.Exceptions.Transactions;

/// <summary>
/// The exception that is thrown when transaction data/input is invalid.
/// </summary>
public class InvalidTransactionException : Xeption
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidTransactionException"/> class with a default error message.
    /// </summary>
    public InvalidTransactionException()
        : base(message: "Invalid transaction data provided. Please check your input and try again.")
    { }
}