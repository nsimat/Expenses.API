using Xeptions;

namespace Expenses.API.Models.Exceptions.UserAccounts;

/// <summary>
/// Represents an exception thrown when a dependency validation error occurs in the user account process.
/// </summary>
/// <remarks>
/// This exception is typically used to indicate issues related to
/// invalid inputs or validation errors in dependent components or services
/// during operations with user accounts.
/// </remarks>
public class UserAccountDependencyValidationException: Xeption
{
    /// <summary>
    /// Represents an exception that occurs when a dependency validation error is encountered
    /// during operations involving user accounts.
    /// </summary>
    /// <param name="innerException">The underlying exception that caused the dependency validation error.</param>
    /// <remarks>
    /// This exception is used to signal issues resulting from validation errors
    /// in dependent components or services, such as input validation in the user account flow.
    /// </remarks>
    public UserAccountDependencyValidationException(Xeption innerException)
        : base(message: "User account dependency validation error occurred. Please fix the errors and try again.", innerException)
    { }
}