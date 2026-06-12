using EFxceptions.Models.Exceptions;
using Expenses.API.DTOs.UserAccounts;
using Expenses.API.Models.Exceptions.UserAccounts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace Expenses.API.Services.Foundations.Accounts;

public partial class UserAccountService
{
    /// <summary>
    /// A delegate representing a function that retrieves a user account
    /// and returns its details encapsulated within a <see cref="UserAccountResponse"/> object.
    /// </summary>
    /// <remarks>
    /// This delegate is designed to handle asynchronous operations for retrieving
    /// user account information, which may include details such as user email, name,
    /// date of birth, recent login times, and associated transactions.
    /// </remarks>
    private delegate ValueTask<UserAccountResponse> ReturningUserAccountFunction();

    /// <summary>
    /// A delegate that defines a function for retrieving a collection of user accounts,
    /// represented as an <see cref="IQueryable{T}"/> of <see cref="UserAccountResponse"/> objects.
    /// </summary>
    /// <remarks>
    /// This delegate is intended to encapsulate logic for retrieving user account data, allowing
    /// for flexible and reusable querying of accounts. The returned collection may include
    /// user details such as email, name, date of birth, account creation dates, recent login times,
    /// and associated transactions.
    /// </remarks>
    private delegate IQueryable<UserAccountResponse> ReturningUserAccountsFunction();

    /// <summary>
    /// Executes the given function within a try-catch block to handle exceptions
    /// related to user account operations.
    /// </summary>
    /// <param name="returningUserAccountFunction">The delegate function to execute, which returns a <see cref="UserAccountResponse"/>.</param>
    /// <returns>A <see cref="UserAccountResponse"/> resulting from the delegate function.</returns>
    /// <exception cref="UserAccountValidationException">Thrown when a validation error occurs.</exception>
    /// <exception cref="UserAccountDependencyException">Thrown when a critical dependency error occurs.</exception>
    /// <exception cref="UserAccountDependencyValidationException">Thrown when a dependency validation error occurs.</exception>
    /// <exception cref="UserAccountServiceException">Thrown when a general service error occurs.</exception>
    private async ValueTask<UserAccountResponse> TryCatch(ReturningUserAccountFunction returningUserAccountFunction)
    {
        try
        {
            return await returningUserAccountFunction();
        }
        catch (NullUserAccountException nullUserAccountException)
        {
            throw CreateAndLogValidationException(nullUserAccountException);
        }
        catch (InvalidUserAccountException invalidUserAccountException)
        {
            throw CreateAndLogValidationException(invalidUserAccountException);
        }
        catch (SqlException sqlException)
        {
            var failedUserAccountStorageException = new FailedUserAccountStorageException(sqlException);

            throw CreateAndLogCriticalDependencyException(failedUserAccountStorageException);
        }
        catch (NotFoundUserAccountException notFoundUserAccountException)
        {
            throw CreateAndLogValidationException(notFoundUserAccountException);
        }
        catch (DuplicateKeyException duplicateKeyException)
        {
            var alreadyExistsUserAccountException = new AlreadyExistsUserAccountException(duplicateKeyException);

            throw CreateAndLogDependencyValidationException(alreadyExistsUserAccountException);
        }
        catch (ForeignKeyConstraintConflictException foreignKeyConstraintConflictException)
        {
            var invalidUserAccountReferenceException =
                new InvalidUserAccountReferenceException(foreignKeyConstraintConflictException);

            throw CreateAndLogDependencyValidationException(invalidUserAccountReferenceException);
        }
        catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
        {
            var lockedUserAccountException = new LockedUserAccountException(dbUpdateConcurrencyException);

            throw CreateAndLogDependencyValidationException(lockedUserAccountException);
        }
        catch (DbUpdateException databaseUpdateException)
        {
            var failedUserAccountStorageException = new FailedUserAccountStorageException(databaseUpdateException);

            throw CreateAndLogDependencyException(failedUserAccountStorageException);
        }
        catch (Exception exception)
        {
            var failedUserAccountServiceException = new FailedUserAccountServiceException(exception);
            
            throw CreateAndLogServiceException(failedUserAccountServiceException);
        }
    }

    /// <summary>
    /// Executes the provided delegate within a try-catch block to handle exceptions
    /// related to querying user account data.
    /// </summary>
    /// <param name="returningUserAccountsFunction">
    /// The delegate function to execute, which returns an <see cref="IQueryable{UserAccountDto}"/>.
    /// </param>
    /// <returns>
    /// A queryable collection of <see cref="UserAccountResponse"/> returned by the provided delegate.
    /// </returns>
    /// <exception cref="UserAccountDependencyException">
    /// Thrown when a critical dependency error occurs, such as a database connectivity issue.
    /// </exception>
    /// <exception cref="UserAccountServiceException">
    /// Thrown when a general service error occurs during the execution of the delegate.
    /// </exception>
    private IQueryable<UserAccountResponse> TryCatch(ReturningUserAccountsFunction returningUserAccountsFunction)
    {
        try
        {
            return returningUserAccountsFunction();
        }
        catch (SqlException sqlException)
        {
            var failedUserAccountStorageException = new FailedUserAccountStorageException(sqlException);

            throw CreateAndLogCriticalDependencyException(failedUserAccountStorageException);
        }
        catch (Exception exception)
        {
            var failedUserAccountServiceException = new FailedUserAccountServiceException(exception);
            
            throw CreateAndLogServiceException(failedUserAccountServiceException);
        }
    }

    /// <summary>
    /// Creates and logs a service exception based on the provided exception.
    /// </summary>
    /// <param name="exception">The exception that caused the service error.</param>
    /// <returns>A <see cref="UserAccountServiceException"/> representing the service error.</returns>
    private UserAccountServiceException CreateAndLogServiceException(Xeption exception)
    {
        var userAccountServiceException = new UserAccountServiceException(exception);

        _loggingBroker.LogError(userAccountServiceException);

        return userAccountServiceException;
    }

    /// <summary>
    /// Creates a dependency exception for user account operations and logs the exception details.
    /// </summary>
    /// <param name="exception">The original exception that caused the dependency error.</param>
    /// <returns>A <see cref="UserAccountDependencyException"/> that wraps the provided exception.</returns>
    private UserAccountDependencyException CreateAndLogDependencyException(Xeption exception)
    {
        var userAccountDependencyException = new UserAccountDependencyException(exception);

        _loggingBroker.LogError(userAccountDependencyException);

        return userAccountDependencyException;
    }

    /// <summary>
    /// Creates and logs a dependency validation exception based on the given exception.
    /// </summary>
    /// <param name="exception">The exception that caused the dependency validation failure.</param>
    /// <returns>A <see cref="UserAccountDependencyValidationException"/> representing the dependency validation error.</returns>
    private UserAccountDependencyValidationException CreateAndLogDependencyValidationException(Xeption exception)
    {
        var userAccountDependencyValidationException = new UserAccountDependencyValidationException(exception);
        
        _loggingBroker.LogError(userAccountDependencyValidationException);
        
        return userAccountDependencyValidationException;
    }

    /// <summary>
    /// Creates and logs a critical dependency exception based on the given exception.
    /// </summary>
    /// <param name="exception">The exception that triggered this critical dependency exception.</param>
    /// <returns>A <see cref="UserAccountDependencyException"/> representing the critical dependency failure.</returns>
    private UserAccountDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
    {
        var userAccountDependencyException = new UserAccountDependencyException(exception);
        
        _loggingBroker.LogCritical(userAccountDependencyException);
        
        return userAccountDependencyException;
    }

    /// <summary>
    /// Creates and logs a validation exception based on the given exception.
    /// </summary>
    /// <param name="exception">The exception that triggered this validation exception.</param>
    /// <returns>A <see cref="UserAccountValidationException"/> representing the validation failure.</returns>
    private UserAccountValidationException CreateAndLogValidationException(Xeption exception)
    {
        var userAccountValidationException = new UserAccountValidationException(exception);
        
        _loggingBroker.LogError(userAccountValidationException);
        
        return userAccountValidationException;
    }
}