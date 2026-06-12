using Xeptions;

namespace Expenses.API.Models.Exceptions.UserAccounts;

/// <summary>
/// Exception that is thrown when attempting to create or add a user account with an ID that already exists in the system.
/// </summary>
/// <remarks>
/// This exception is typically used to signal a conflict when a duplicate key or identifier for a user account is
/// detected during operations such as creation or addition. It wraps an inner exception that provides additional
/// context about the root cause of the issue.
/// </remarks>
public class AlreadyExistsUserAccountException: Xeption
{
    /// <summary>
    /// Exception that is thrown when attempting to create or add a user account with an ID that already exists in the system.
    /// </summary>
    /// <param name="innerException">The underlying cause of the caught error</param>
    public AlreadyExistsUserAccountException(Exception innerException)
        : base(message: "User account with the same ID already exists.",  innerException)
    { }
}