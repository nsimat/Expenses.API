using Xeptions;

namespace Expenses.API.Models.Exceptions.UserAccounts;

/// <summary>
/// Exception that is thrown when a user account is locked and cannot be accessed. This exception typically occurs in
/// scenarios where database-level concurrency or locking conflicts prevent the user account from being updated or
/// accessed. It signals that further attempts to interact with the user account should be deferred until the lock is
/// resolved, or additional actions should be taken by contacting support.
/// </summary>
public class LockedUserAccountException : Xeption
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LockedUserAccountException"/> class with a specified inner exception.
    /// </summary>
    public LockedUserAccountException(Exception innerException)
        : base(message: "User account record is locked. Please try again later or contact support.", innerException)
    { }
}