using Xeptions;

namespace Expenses.API.Models.Exceptions.UserAccounts;

/// <summary>
/// Represents an exception thrown when a dependency-related error occurs during operations involving user accounts.
/// </summary> <remarks>
/// This exception is intended to be used for scenarios where an underlying dependency,
/// such as a database or external service, encounters an error while processing user account operations. It serves as
/// a wrapper for exceptions that may arise from dependencies, providing a consistent error message to the caller while
/// preserving the original exception details for debugging purposes.
/// </remarks>
public class UserAccountDependencyException : Xeption
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserAccountDependencyException"/> class with a default error
    /// message and an inner exception. 
    /// </summary>
    /// <param name="innerException">The exception that caused the current exception.</param>
    public UserAccountDependencyException(Xeption innerException)
        : base(message: "User account dependency error occurred. Please contact support.", innerException)
    { }
}