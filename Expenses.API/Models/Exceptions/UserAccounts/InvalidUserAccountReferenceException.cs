using Xeptions;

namespace Expenses.API.Models.Exceptions.UserAccounts;

/// <summary>
/// Represents an exception thrown when a user account contains invalid or incorrect reference details that violate
/// database constraints or integrity rules.
/// </summary>
/// <remarks>
/// This exception typically occurs in scenarios involving foreign key conflicts or invalid reference relationships
/// during user account operations. The exception is used to signal dependency validation issues, allowing the
/// application to handle such problems gracefully and ensure proper error reporting.
/// </remarks>
public class InvalidUserAccountReferenceException : Xeption
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidUserAccountReferenceException"/> class with a specified inner
    /// exception that provides details about the underlying cause of the error.
    /// </summary>
    /// <param name="innerException">The exception that caused the current exception.</param>
    public InvalidUserAccountReferenceException(Exception innerException)
        : base(message: "Invalid user account reference error occurred. Please contact support.", innerException)
    { }
}