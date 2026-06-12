using Xeptions;

namespace Expenses.API.Models.Exceptions.Transactions;

/// <summary>
/// Represents an exception thrown when a transaction ID is invalid.
/// </summary>
/// <remarks>
/// This exception should be used to indicate issues related to invalid input for transaction IDs,
/// such as when the format or content of the provided ID does not meet the expected requirements.
/// </remarks>
/// <example>
/// This exception might be used in scenarios such as validating user input or database queries
/// when a transaction ID is expected to be a valid GUID but fails to meet that expectation.
/// </example>
public class InvalidTransactionIdException: Xeption
{
    /// <summary>
    /// Represents an exception thrown when a transaction ID is invalid.
    /// </summary>
    /// <remarks>
    /// This exception is intended to be used for scenarios where the provided
    /// transaction ID does not meet expected requirements, such as format or validity.
    /// </remarks>
    public InvalidTransactionIdException(): 
        base(message: "Invalid transaction ID. Please check your input and try again.")
    { }
}