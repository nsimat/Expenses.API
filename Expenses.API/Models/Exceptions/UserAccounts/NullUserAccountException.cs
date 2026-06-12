using Xeptions;

namespace Expenses.API.Models.Exceptions.UserAccounts;

/// <summary>
/// Exception that is thrown when a null UserAccount object is encountered.
/// </summary>
/// <remarks>
/// This exception is typically used to signal that a UserAccount object is expected but was found to be null during
/// operations such as retrieval, creation, or update. It indicates a validation error where the absence of a
/// UserAccount object is not acceptable and requires attention to ensure that the necessary data is provided for the
/// operation to proceed successfully.
/// </remarks>
public class NullUserAccountException: Xeption
{
    /// <summary>
    /// Exception that is thrown when a null <see cref="UserAccount"/> object is encountered.
    /// </summary>
    /// <remarks>
    /// This exception is triggered during operations where a <see cref="UserAccount"/> object is expected but found to be null.
    /// It serves to identify validation errors related to the absence of a <see cref="UserAccount"/> instance,
    /// which is required for processes like data retrieval, creation, or update to ensure the functionality proceeds correctly.
    /// </remarks>
    public NullUserAccountException() : base(message: "UserAccount object is null.")
    { }
}