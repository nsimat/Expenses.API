using Xeptions;

namespace Expenses.API.Models.Exceptions.UserAccounts;

/// <summary>
/// Represents an exception thrown when invalid user account data is provided.
/// </summary>
/// <remarks>
/// This exception is used to indicate that the user account data does not meet the required validation criteria.
/// It is thrown when operations on user accounts involve invalid input or parameters.
/// </remarks>
public class InvalidUserAccountException : Xeption
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidUserAccountException"/> class with a default error message.
    /// </summary>
    public InvalidUserAccountException()
        : base(message: "Invalid user account data provided. Please check your input and try again.")
    { }
}