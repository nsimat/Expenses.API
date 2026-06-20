using System.Text.RegularExpressions;
using EFxceptions.Models.Exceptions;
using Expenses.API.DTOs.UserAccounts;
using Expenses.API.Models;
using Expenses.API.Models.Exceptions.UserAccounts;
using Microsoft.AspNetCore.Identity;


namespace Expenses.API.Services.Foundations.Accounts;

public partial class UserAccountService
{
    /// <summary>
    /// Generates a regular expression to validate email addresses based on the specified pattern.
    /// </summary>
    /// <returns>
    /// A partial <see cref="Regex"/> instance compiled from the provided regular expression pattern,
    /// making use of case-insensitive comparison and localized to the "fr-BE" culture.
    /// </returns>
    [GeneratedRegex(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$", RegexOptions.IgnoreCase)]
    private static partial Regex EmailRegex();

    /// <summary>
    /// Validates the provided user account object during the addition process to ensure all required properties are valid.
    /// </summary>
    /// <param name="userAccount">
    /// The <see cref="UserAccount"/> instance to validate, containing details such as
    /// the user account's unique identifier, email, password hash, and creation/update timestamps.
    /// </param>
    private void ValidateUserAccountOnAdd(UserAccount userAccount)
    {
        ValidateUserAccountIsNotNull(userAccount);

        Validate(
            (Rule: IsInvalid(userAccount.Id), Parameter: nameof(userAccount.Id)),
            (Rule: IsInvalidEmail(userAccount.Email), Parameter: nameof(userAccount.Email)),
            (Rule: IsInvalidPassword(userAccount.PasswordHash), Parameter: nameof(userAccount.PasswordHash)),
            (Rule: IsInvalidDate(userAccount.CreatedAt), Parameter: nameof(userAccount.CreatedAt)),
            (Rule: IsInvalidDate(userAccount.UpdatedAt), Parameter: nameof(userAccount.UpdatedAt)),
            (Rule: IsCreationDateLaterThanUpdate(
                    firstDate: userAccount.CreatedAt,
                    secondDate: userAccount.UpdatedAt,
                    secondName: nameof(userAccount.UpdatedAt)),
                Parameter: nameof(userAccount.CreatedAt)),
            (Rule: IsNotRecent(userAccount.UpdatedAt), Parameter: nameof(userAccount.UpdatedAt))
        );
    }

    /// <summary>
    /// Validates the user account during a profile modification request by checking the validity of the provided fields.
    /// </summary>
    /// <param name="userUpdateRequest">An instance of <see cref="UserUpdateRequest"/> containing the updated user information to be validated.</param>
    private void ValidateUserAccountOnModifyProfile(UserUpdateRequest userUpdateRequest)
    {
        Validate(
            (Rule: IsInvalidEmail(userUpdateRequest.Email), Parameter: nameof(userUpdateRequest.Email)),
            (Rule: IsInvalidName(userUpdateRequest.FirstName), Parameter: nameof(userUpdateRequest.FirstName)),
            (Rule: IsInvalidName(userUpdateRequest.LastName), Parameter: nameof(userUpdateRequest.LastName)),
            (Rule: IsInvalidDateOfBirth(userUpdateRequest.DateOfBirth), Parameter: nameof(userUpdateRequest.DateOfBirth))
        );
    }

    /// <summary>
    /// Evaluates whether the provided date of birth is invalid based on the application's requirements.
    /// </summary>
    /// <param name="dateOfBirth">The date of birth to be validated.</param>
    /// <returns>
    /// A dynamic object containing a boolean condition indicating if the date of birth is invalid,
    /// and an associated error message describing the validation failure.
    /// </returns>
    private static dynamic IsInvalidDateOfBirth(DateTimeOffset dateOfBirth) => new
    {
        Condition = dateOfBirth == default || dateOfBirth > DateTimeOffset.UtcNow.AddYears(-18),
        Message = "User must be at least 18 years old and the date of birth should be a valid past date."
    };

    /// <summary>
    /// Evaluates whether the provided name is invalid based on specific conditions.
    /// </summary>
    /// <param name="name">The name to validate.</param>
    /// <returns>
    /// An anonymous object containing a <c>Condition</c> property, which evaluates to <c>true</c> if the name is invalid,
    /// and a <c>Message</c> property that provides a description of the validation rule.
    /// </returns>
    private static dynamic IsInvalidName(string name) => new
    {
        Condition = name == string.Empty || string.IsNullOrWhiteSpace(name) || name.Length < 3,
        Message = "Name is required and should be at least 3 characters long."
    };

    /// <summary>
    /// Determines whether the specified date is not recent based on a predefined threshold.
    /// </summary>
    /// <param name="date">The date to be validated.</param>
    /// <returns>
    /// A dynamic object containing:
    /// <c>Condition</c> - A boolean indicating whether the date is not recent.
    /// <c>Message</c> - A string specifying why the validation failed.
    /// </returns>
    private static dynamic IsNotRecent(DateTimeOffset date) => new
    {
        Condition = IsDateNotRecent(date),
        Message = "Date is not recent."
    };

    /// <summary>
    /// Checks whether the specified date is not considered recent based on a predefined time span.
    /// </summary>
    /// <param name="date">The date to be evaluated.</param>
    /// <returns>
    /// A boolean value indicating whether the specified date is not recent.
    /// Returns <c>true</c> if the date is older than the predefined threshold; otherwise, <c>false</c>.
    /// </returns>
    private static object IsDateNotRecent(DateTimeOffset date) // to be reviewed!!!
    {
        DateTimeOffset currentDateTime = DateTimeOffset.UtcNow;
        TimeSpan timeDifference = currentDateTime.Subtract(date);
        Console.WriteLine($"Time difference: {timeDifference}");// check pertinence of this line!!!
        return timeDifference.Duration() > TimeSpan.FromMinutes(1);
    }

    /// <summary>
    /// Validates that the provided <see cref="UserAccount"/> instance is not null.
    /// </summary>
    /// <param name="userAccount">The <see cref="UserAccount"/> instance to be validated.</param>
    /// <exception cref="NullUserAccountException">
    /// Thrown when the <paramref name="userAccount"/> is null.
    /// </exception>
    private static void ValidateUserAccountIsNotNull(UserAccount userAccount)
    {
        if (userAccount is null)
        {
            throw new NullUserAccountException();
        }
    }

    /// <summary>
    /// Validates that a user account does not already exist in the system. If the provided user account is not null,
    /// it indicates that a user account with the same identifier already exists, and an
    /// <see cref="AlreadyExistsUserAccountException"/> is thrown to signal this condition.    
    /// </summary>
    /// <param name="userAccount"></param>
    /// <exception cref="AlreadyExistsUserAccountException"></exception>
    private static void ValidateUserAccountNotAlreadyExists(UserAccount userAccount)
    {
        if (userAccount is not null)
        {
            throw new AlreadyExistsUserAccountException(new DuplicateKeyException("User account already exists."));
        }
    }

    /// <summary>
    /// Validates whether the provided user account identifier or email is valid.
    /// </summary>
    /// <param name="objectIds">
    /// A collection of objects where the first item must be either a <see cref="Guid"/> representing the user account ID
    /// or a <see cref="string"/> containing the user account email.
    /// </param>
    /// <exception cref="InvalidUserAccountException">
    /// Thrown when the provided user account ID is empty, invalid, or not in a valid Guid format,
    /// or when the email is null, empty, whitespace, or does not match the expected email pattern.
    /// </exception>
    private static void ValidateUserAccountIdOrEmail(params object[] objectIds)
    {
        if (objectIds[0] is Guid userAccountId)
        {
            if (userAccountId == Guid.Empty || !Guid.TryParse(userAccountId.ToString(), out _))
                throw new InvalidUserAccountException();
        }
        else if (objectIds[0] is string userAccountEmail)
        {
            if (string.IsNullOrWhiteSpace(userAccountEmail) || !EmailRegex().IsMatch(userAccountEmail))
                throw new InvalidUserAccountException();
        }
    }

    /// <summary>
    /// Validates the given user account by matching its details against
    /// specified object identifiers, such as user account IDs or emails.
    /// </summary>
    /// <param name="userAccount">
    /// The user account instance to validate.
    /// </param>
    /// <param name="objectIds">
    /// A variadic parameter containing identifiers for the validation, such as a GUID for
    /// user account ID or a string for email.
    /// </param>
    private static void ValidateStorageUserAccount(UserAccount userAccount, params object[] objectIds)
    {
        switch (objectIds[0])
        {
            case Guid userAccountId:
                ValidateStorageUserAccountById(userAccount, userAccountId, objectIds[0]);
                break;
            case string email:
                ValidateStorageUserAccountByEmail(userAccount, email);
                break;
            default: return;
        }
    }

    /// <summary>
    /// Validates the existence of a user account in the storage by matching its unique identifier.
    /// </summary>
    /// <param name="userAccount">The user account object retrieved from the storage.</param>
    /// <param name="userAccountId">The unique identifier of the user account being validated.</param>
    /// <param name="objectId">The original identifier input used for lookup.</param>
    /// <exception cref="NotFoundUserAccountException">
    /// Thrown when the user account is null, the provided user account ID is empty,
    /// or the retrieved user account's ID does not match the specified user account ID.
    /// </exception>
    private static void ValidateStorageUserAccountById(
        UserAccount userAccount,
        Guid userAccountId,
        object objectId)
    {
        if (userAccount is null ||
            userAccountId == Guid.Empty ||
            !userAccount.Id.Equals(userAccountId))
            throw new NotFoundUserAccountException(objectId);
    }

    /// <summary>
    /// Validates the existence of a user account in storage by comparing the provided email
    /// with the email of the given user account. Throws an exception if validation fails.
    /// </summary>
    /// <param name="userAccount">
    /// The <see cref="UserAccount"/> object whose email is to be validated.
    /// </param>
    /// <param name="email">
    /// The email address to be matched against the user account's email for validation.
    /// </param>
    /// <exception cref="NotFoundUserAccountException">
    /// Thrown when the user account is null or when the provided email does not match
    /// the email of the user account.
    /// </exception>
    private static void ValidateStorageUserAccountByEmail(
        UserAccount userAccount,
        string email)
    {
        if (userAccount is null ||
            string.IsNullOrWhiteSpace(email) ||
            !userAccount.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
            throw new NotFoundUserAccountException(email);
    }

    /// <summary>
    /// Validates the provided user account password against the stored password hash.
    /// Ensures the entered password matches the hashed password in storage.
    /// </summary>
    /// <param name="userAccount">
    /// The user account object to validate, containing account-related details.
    /// </param>
    /// <param name="storedPasswordHash">
    /// The hashed password previously stored for the specified user account.
    /// </param>
    /// <param name="enteredPassword">
    /// The password entered by the user during the authentication process.
    /// </param>
    /// <exception cref="NotFoundUserAccountException">
    /// Thrown when the user account is null, the stored password hash or entered password is not provided,
    /// or when the entered password fails to match the stored password hash.
    /// </exception>
    private void ValidateStorageUserAccountPassword(
        UserAccount userAccount, 
        string storedPasswordHash,
        string enteredPassword)
    {
        ValidateUserAccountPasswordMatchesStoredHash(userAccount, storedPasswordHash, enteredPassword);
    }

    /// <summary>
    /// Validates that the entered password matches the stored password hash
    /// associated with a given user account.
    /// </summary>
    /// <param name="userAccount">
    /// The user account for which the password validation is performed.
    /// </param>
    /// <param name="storedPasswordHash">
    /// The hashed password stored in the system for the specified user account.
    /// </param>
    /// <param name="enteredPassword">
    /// The plaintext password entered by the user during authentication.
    /// </param>
    /// <exception cref="NotFoundUserAccountException">
    /// Thrown when the provided user account information or credentials do not
    /// match the stored records.
    /// </exception>
    private void ValidateUserAccountPasswordMatchesStoredHash(
        UserAccount userAccount,
        string storedPasswordHash,
        string enteredPassword)
    {
        if (userAccount is null ||
            string.IsNullOrWhiteSpace(storedPasswordHash) ||
            string.IsNullOrWhiteSpace(enteredPassword) ||
            _passwordHasher.VerifyHashedPassword(userAccount, storedPasswordHash, enteredPassword)
            == PasswordVerificationResult.Failed)
        {
            throw new NotFoundUserAccountException("Invalid user account credentials (email or password).");
        }
    }

    /// <summary>
    /// Determines whether the given user account ID is invalid.
    /// </summary>
    /// <param name="userAccountId">The identifier of the user account to validate.</param>
    /// <returns>
    /// An object containing a condition that evaluates whether the ID is invalid
    /// and an associated error message if the condition is true.
    /// </returns>
    private static dynamic IsInvalid(Guid userAccountId) => new
    {
        Condition = userAccountId == Guid.Empty || !Guid.TryParse(userAccountId.ToString(), out _),
        Message = "User account ID is required and should be a valid GUID."
    };

    /// <summary>
    /// Validates whether the provided email address is in a valid format and meets the required conditions.
    /// </summary>
    /// <param name="userAccountEmail">The email address of the user account to validate.</param>
    /// <returns>
    /// A dynamic object containing a validation condition and an associated error message.
    /// The condition indicates whether the email is invalid, and the error message provides details on the issue.
    /// </returns>
    private static dynamic IsInvalidEmail(string userAccountEmail) => new
    {
        Condition = userAccountEmail == string.Empty
                    || string.IsNullOrWhiteSpace(userAccountEmail)
                    || !EmailRegex().IsMatch(userAccountEmail),
        Message = "User account email is required and should be in a valid format."
    };

    /// <summary>
    /// Validates whether the provided password hash is invalid based on specific rules.
    /// </summary>
    /// <param name="userAccountPasswordHash">
    /// The hash of the user account password to validate.
    /// </param>
    /// <returns>
    /// A dynamic object containing a condition that indicates whether the password hash is invalid
    /// and an associated validation message.
    /// </returns>
    private static dynamic IsInvalidPassword(string userAccountPasswordHash) => new
    {
        Condition = userAccountPasswordHash == string.Empty,
        Message = "Password hash is required and should be in a valid format."
    };

    /// <summary>
    /// Validates whether the given date is invalid by checking if it is assigned a default or non-valid value.
    /// </summary>
    /// <param name="userAccountDate">The date to validate, typically representing a creation or update timestamp for a user account.</param>
    /// <returns>
    /// A dynamic object containing two properties:
    /// - <c>Condition</c>: A boolean value indicating if the date is invalid.
    /// - <c>Message</c>: A string providing a description of the validation error if the date is deemed invalid.
    /// </returns>
    private static dynamic IsInvalidDate(DateTimeOffset userAccountDate) => new
    {
        Condition = userAccountDate == default,
        Message = "Date is required and should be in a valid format."
    };

    /// <summary>
    /// Determines if the creation date of a user account is later than its update date.
    /// </summary>
    /// <param name="firstDate">
    /// The creation date of the user account to validate.
    /// </param>
    /// <param name="secondDate">
    /// The update date of the user account to compare against.
    /// </param>
    /// <param name="secondName">
    /// The name of the update date parameter used in the error message.
    /// </param>
    /// <returns>
    /// A dynamic object that specifies the validation condition and an error message
    /// if the creation date is later than or equal to the update date.
    /// </returns>
    private static dynamic IsCreationDateLaterThanUpdate(
        DateTimeOffset firstDate,
        DateTimeOffset secondDate,
        string secondName) => new
    {
        Condition = firstDate >= secondDate,
        Message = $"The creation date of a user account must be earlier than the update date {secondName}."
    };

    /// <summary>
    /// Validates a set of dynamic rules against specified parameters and applies associated error handling if any rule is violated.
    /// </summary>
    /// <param name="validations">
    /// A collection of dynamic validation rules and their corresponding parameter names.
    /// Each rule must provide a `Condition` indicating if it has been violated and a `Message` describing the validation error.
    /// </param>
    /// <exception cref="InvalidUserAccountException">
    /// Thrown when one or more validation rules have been violated. The exception contains the details of all validation errors.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when a validation rule is not properly structured or fails to include a descriptive message.
    /// </exception>
    private static void Validate(params (dynamic Rule, string Parameter)[] validations)
    {
        var invalidUserAccountException = new InvalidUserAccountException();

        foreach ((dynamic rule, string parameter) in validations)
        {
            if (rule.Condition)
            {
                invalidUserAccountException.UpsertDataList(
                    key: parameter,
                    value: rule.Message
                    ?? throw new InvalidOperationException("Rule is not a valid dynamic object.") 
                );
                Console.WriteLine($"User account validation failed for {parameter}: {rule.Message}!!!");
            }
        }

        invalidUserAccountException.ThrowIfContainsErrors();
    }
}