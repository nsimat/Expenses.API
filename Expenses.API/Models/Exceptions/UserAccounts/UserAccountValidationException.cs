using Xeptions;

namespace Expenses.API.Models.Exceptions.UserAccounts;

/// <summary>
/// Represents an exception that occurs during the validation of user account data.
/// </summary>
/// <remarks>
/// This exception is triggered when invalid user account data is provided. It encapsulates the details of the error
/// and serves as a wrapper for the inner exception that caused the validation failure.
/// </remarks>
public class UserAccountValidationException : Xeption
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserAccountValidationException"/> class with a specified
    /// inner exception.
    /// </summary>
    /// <param name="innerException">The exception that caused the current exception.</param>
    public UserAccountValidationException(Xeption innerException)
        : base(message: "Invalid user account data provided. Please check your input and try again.", innerException)
    { }
}