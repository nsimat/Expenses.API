using Xeptions;

namespace Expenses.API.Models.Exceptions.UserAccounts;

/// <summary>
/// Exception that is thrown when a user account with the specified identifier is not found.
/// </summary>
public class NotFoundUserAccountException: Xeption
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundUserAccountException"/> class with a default error message
    /// that includes the identifier of the user account that was not found. 
    /// </summary>
    /// <param name="objectIds">
    /// The identifier of the user account that was not found. That could be a Guid identifier or
    /// an email string.
    /// </param>
    public NotFoundUserAccountException(params object[] objectIds)
        : base(message: $"Couldn't find user account with identifier: {objectIds[0]}.")
    { }
}