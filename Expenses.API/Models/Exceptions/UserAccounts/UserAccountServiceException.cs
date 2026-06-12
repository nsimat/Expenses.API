using Xeptions;

namespace Expenses.API.Models.Exceptions.UserAccounts;

/// <summary>
/// Represents an exception thrown to indicate a failure during the execution of user account service operations.
/// </summary>
/// <remarks>
/// This exception is designed to encapsulate errors/issues occuring within a user account service-related functionality.
/// Common scenarios for throwing this exception include failures in service-layer operations (often due to dependency
/// failures, incorrect logic) or unhandled exceptions propagated from dependent services or components. It serves as a
/// uniform way to propagate and log such errors, providing a consistent error message to the caller while preserving
/// the original exception details for debugging purposes.    
/// </remarks>
public class UserAccountServiceException : Xeption
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserAccountServiceException"/> class with a specified inner exception.
    /// </summary>
    /// <param name="innerException">The underlying exception that caused the failure in the user account service.</param>
    public UserAccountServiceException(Xeption innerException)
        : base(message: "User account service error occurred. Please contact support.", innerException)
    { }
}