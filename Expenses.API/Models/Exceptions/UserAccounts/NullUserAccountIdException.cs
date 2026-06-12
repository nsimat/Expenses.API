using Xeptions;

namespace Expenses.API.Models.Exceptions.UserAccounts;

/// <summary>
/// The exception that is thrown when the user ID context is null, indicating that the current user is not
/// authenticated or authorized to perform certain actions. 
/// </summary>
public class NullUserAccountIdException: Xeption
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NullUserAccountIdException"/> class with a default error message
    /// indicating that the user ID context is null and access is forbidden. 
    /// </summary>
    public NullUserAccountIdException()
        : base(message: "The current context for the user ID is null. Access forbidden.")
    { }
}