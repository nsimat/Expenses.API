using Xeptions;

namespace Expenses.API.Models.Exceptions.UserAccounts;

/// <summary>
/// Represents an exception thrown when a failure occurs in the user account service.
/// This exception typically indicates issues within the service that are not related  to validation or dependency
/// errors and requires further attention or investigation from support personnel for resolution beyond validation or
/// dependency concerns.
/// </summary>
public class FailedUserAccountServiceException : Xeption
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FailedUserAccountServiceException"/> class with a specified inner
    /// exception.
    /// </summary>
    /// <param name="innerException">The underlying exception that caused the failure in the user account service.</param>   
    public FailedUserAccountServiceException(Exception innerException)
        : base(message: "Failed user account service error occurred. Please contact support.", innerException)
    { }
}