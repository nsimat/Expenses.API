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

    private void ValidateUserAccountOnModifyProfile(UserUpdateRequest userUpdateRequest)
    {
        Validate(
            (Rule: IsInvalidEmail(userUpdateRequest.Email), Parameter: nameof(userUpdateRequest.Email)),
            (Rule: IsInvalidName(userUpdateRequest.FirstName), Parameter: nameof(userUpdateRequest.FirstName)),
            (Rule: IsInvalidName(userUpdateRequest.LastName), Parameter: nameof(userUpdateRequest.LastName))
        );
    }

    private static dynamic IsInvalidName(string name) => new 
    {
        Condition = name == string.Empty || !string.IsNullOrWhiteSpace(name) || name.Length < 3,
        Message = "Name is required and should be at least 3 characters long."
    };

    private static dynamic IsNotRecent(DateTimeOffset date) => new
    {
        Condition = IsDateNotRecent(date),
        Message = "Date is not recent."
    };

    private static object IsDateNotRecent(DateTimeOffset date)// to be reviewed!!!
    {
        DateTimeOffset currentDateTime = DateTimeOffset.UtcNow;
        TimeSpan timeDifference = currentDateTime.Subtract(date);
        Console.WriteLine($"Time difference: {timeDifference}");
        return timeDifference.Duration() > TimeSpan.FromMinutes(1);
    }

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

    private static void ValidateUserAccountIdOrEmail(params object[] objectIds)
    {
        if(objectIds[0] is Guid userAccountId)
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

    private static void ValidateStorageUserAccount(UserAccount userAccount, params object[] objectIds)
    {
        switch (objectIds[0])
        {
            case Guid userAccountId:
                if (userAccount is null ||
                    userAccountId == Guid.Empty ||
                    !userAccount.Id.Equals(userAccountId))
                    throw new NotFoundUserAccountException(objectIds[0]);
                break;
            case string email:
                if (userAccount is null ||
                    string.IsNullOrWhiteSpace(email) ||
                    !userAccount.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
                    throw new NotFoundUserAccountException(email);
                break;
            default: return;
        }
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
    private void ValidateStorageUserAccountPassword(UserAccount userAccount, string storedPasswordHash,
        string enteredPassword)
    {
        if (userAccount is null ||
            string.IsNullOrWhiteSpace(storedPasswordHash) ||
            string.IsNullOrWhiteSpace(enteredPassword) ||
            _passwordHasher.VerifyHashedPassword(userAccount, storedPasswordHash, enteredPassword)
            == PasswordVerificationResult.Failed)

            throw new NotFoundUserAccountException("Invalid user account credentials (email or password).");
    }

    private static dynamic IsInvalid(Guid userAccountId) => new
    {
        Condition = userAccountId == Guid.Empty || !Guid.TryParse(userAccountId.ToString(), out _),
        Message = "User account ID is required and should be a valid GUID."
    };

    private static dynamic IsInvalidEmail(string userAccountEmail) => new
    {
        Condition = userAccountEmail == string.Empty
                    || string.IsNullOrWhiteSpace(userAccountEmail)
                    || !EmailRegex().IsMatch(userAccountEmail),
        Message = "User account email is required and should be in a valid format."
    };

    private static dynamic IsInvalidPassword(string userAccountPasswordHash) => new
    {
        Condition = userAccountPasswordHash == string.Empty,
        Message = "Password hash is required and should be in a valid format."
    };

    private static dynamic IsInvalidDate(DateTimeOffset userAccountDate) => new
    {
        Condition = userAccountDate == default,
        Message = "Date is required and should be in a valid format."
    };

    private static dynamic IsCreationDateLaterThanUpdate(DateTimeOffset firstDate, DateTimeOffset secondDate, string secondName) => new
    {
        Condition = firstDate >= secondDate,
        Message = $"Creation date cannot be later than update date, {secondName}."
    };


    private static void Validate(params (dynamic Rule, string Parameter)[] validations)
    {
        var invalidUserAccountException = new InvalidUserAccountException();

        foreach ((dynamic rule, string parameter) in validations)
        {
            if (rule.Condition)
            {
                invalidUserAccountException.UpsertDataList(
                    key: parameter,
                    value: rule.ToString());
                Console.WriteLine($"Validation failed for {parameter}: {rule.Message}!!!");
            }
        }

        invalidUserAccountException.ThrowIfContainsErrors();
    }
}