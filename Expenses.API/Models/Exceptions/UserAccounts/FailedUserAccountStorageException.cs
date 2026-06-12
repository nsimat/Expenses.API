using Xeptions;

namespace Expenses.API.Models.Exceptions.UserAccounts;

/// <summary>
/// Represents an exception thrown when there is a failure in storing a user account in the storage system.
/// </summary>
/// <remarks>
/// This exception is typically used to wrap issues caused by critical dependencies, such as database-related errors,
/// or issues with the underlying storage system. It provides encapsulation for unexpected errors that occur during
/// user account storage operations and should be logged or handled appropriately.
/// </remarks>
public class FailedUserAccountStorageException : Xeption
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FailedUserAccountStorageException"/> class with a specified inner
    /// exception.
    /// </summary>
    /// <paramref name="innerException">The underlying exception that caused the failure to store the user account.</paramref>
    public FailedUserAccountStorageException(Exception innerException)
        : base(message: "Failed to store user account in the storage. Please contact support", innerException)
    { }
}