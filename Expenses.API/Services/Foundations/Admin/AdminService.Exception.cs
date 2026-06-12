using EFxceptions.Models.Exceptions;
using Expenses.API.DTOs.Transactions;
using Expenses.API.DTOs.UserAccounts;
using Expenses.API.Models.Exceptions.Transactions;
using Expenses.API.Models.Exceptions.UserAccounts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace Expenses.API.Services.Foundations.Admin;

public partial class AdminService
{
    private delegate ValueTask<UserAccountResponse> ReturningUserAccountFunction();

    private delegate IQueryable<UserAccountResponse> ReturningUserAccountsFunction();

    private delegate ValueTask<TransactionResponse> ReturningTransactionFunction(); // to be reviewed

    private delegate IQueryable<TransactionResponse> ReturningTransactionsFunction(); // to be reviewed

    #region UserAccount Exceptions

    /// <summary>
    /// Executes a function that returns a <see cref="UserAccountResponse"/> and handles known exceptions.
    /// </summary>
    /// <param name="returningUserAccountFunction">
    /// The delegate function that returns a value task of <see cref="UserAccountResponse"/>.
    /// </param>
    /// <returns>
    /// A value task containing the result of the provided function.
    /// </returns>
    /// <exception cref="NullUserAccountException">
    /// Thrown when the provided delegate results in a null user account.
    /// </exception>
    /// <exception cref="InvalidUserAccountException">
    /// Thrown when the provided delegate results in an invalid user account.
    /// </exception>
    /// <exception cref="NotFoundUserAccountException">
    /// Thrown when the provided delegate cannot find the requested user account.
    /// </exception>
    /// <exception cref="AlreadyExistsUserAccountException">
    /// Thrown when the provided delegate detects a duplicate user account.
    /// </exception>
    /// <exception cref="InvalidUserAccountReferenceException">
    /// Thrown when the provided delegate detects an invalid user account reference.
    /// </exception>
    /// <exception cref="LockedUserAccountException">
    /// Thrown when the provided delegate detects a concurrency conflict.
    /// </exception>
    /// <exception cref="FailedUserAccountStorageException">
    /// Thrown when the provided delegate encounters an issue with database storage.
    /// </exception>
    /// <exception cref="FailedUserAccountServiceException">
    /// Thrown for any general error that occurs during the execution of the delegate.
    /// </exception>
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
    /// Executes a function that returns an <see cref="IQueryable{UserAccountDto}"/>
    /// and handles expected exceptions during its execution.
    /// </summary>
    /// <param name="returningUserAccountsFunction">
    /// The delegate function that returns an <see cref="IQueryable{UserAccountDto}"/>.
    /// </param>
    /// <returns>
    /// An <see cref="IQueryable{UserAccountDto}"/> containing the result of the function execution.
    /// </returns>
    /// <exception cref="FailedUserAccountStorageException">
    /// Thrown when a database-related issue occurs during the function execution.
    /// </exception>
    /// <exception cref="FailedUserAccountServiceException">
    /// Thrown when a general error occurs during the function execution.
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

            throw CreateAndLogValidationException(failedUserAccountServiceException);
        }
    }

    /// <summary>
    /// Creates a <see cref="UserAccountValidationException"/> using the given <see cref="Xeption"/>
    /// and logs the error.
    /// </summary>
    /// <param name="exception">
    /// The <see cref="Xeption"/> that caused the validation failure.
    /// </param>
    /// <returns>
    /// A <see cref="UserAccountValidationException"/> representing the validation error.
    /// </returns>
    private UserAccountValidationException CreateAndLogValidationException(Xeption exception)
    {
        var userAccountValidationException = new UserAccountValidationException(exception);

        _loggingBroker.LogError(userAccountValidationException);

        return userAccountValidationException;
    }

    /// <summary>
    /// Creates and logs a critical dependency exception for a user account operation.
    /// </summary>
    /// <param name="exception">
    /// The original exception that triggered the critical dependency error.
    /// </param>
    /// <returns>
    /// A <see cref="UserAccountDependencyException"/> representing the critical dependency exception.
    /// </returns>
    private UserAccountDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
    {
        var userAccountDependencyException = new UserAccountDependencyException(exception);

        _loggingBroker.LogCritical(userAccountDependencyException);

        return userAccountDependencyException;
    }

    /// <summary>
    /// Creates and logs a <see cref="UserAccountDependencyValidationException"/> based on the provided exception.
    /// </summary>
    /// <param name="exception">
    /// The <see cref="Xeption"/> instance to wrap in a <see cref="UserAccountDependencyValidationException"/>.
    /// </param>
    /// <returns>
    /// A <see cref="UserAccountDependencyValidationException"/> containing the provided exception.
    /// </returns>
    private UserAccountDependencyValidationException CreateAndLogDependencyValidationException(Xeption exception)
    {
        var userAccountDependencyValidationException = new UserAccountDependencyValidationException(exception);

        _loggingBroker.LogError(userAccountDependencyValidationException);

        return userAccountDependencyValidationException;
    }

    private UserAccountDependencyException CreateAndLogDependencyException(Xeption exception)
    {
        var userAccountDependencyException = new UserAccountDependencyException(exception);

        _loggingBroker.LogError(userAccountDependencyException);

        return userAccountDependencyException;
    }

    private UserAccountServiceException CreateAndLogServiceException(Xeption exception)
    {
        var userAccountServiceException = new UserAccountServiceException(exception);

        _loggingBroker.LogError(userAccountServiceException);

        return userAccountServiceException;
    }

    #endregion

    #region Transaction Exceptions!!! // To be reviewed

    private ValueTask<TransactionResponse> TryCatch(ReturningTransactionFunction returningTransactionFunction)
    {
        try
        {
            return returningTransactionFunction();
        }
        catch (NullTransactionException nullTransactionException)
        {
            throw CreateAndLogValidationException(nullTransactionException);
        }
        catch (InvalidTransactionException invalidTransactionException)
        {
            throw CreateAndLogValidationException(invalidTransactionException);
        }
        catch (SqlException sqlException)
        {
            var failedTransactionStorageException = new FailedTransactionStorageException(sqlException);

            throw CreateAndLogCriticalDependencyException(failedTransactionStorageException);
        }
        catch (NotFoundTransactionException notFoundTransactionException)
        {
            throw CreateAndLogValidationException(notFoundTransactionException);
        }
        catch (DuplicateKeyException duplicateKeyException)
        {
            var alreadyExistsTransactionException = new AlreadyExistsTransactionException(duplicateKeyException);

            throw CreateAndLogDependencyValidationException(alreadyExistsTransactionException);
        }
        catch (ForeignKeyConstraintConflictException foreignKeyConstraintConflictException)
        {
            var invalidTransactionReferenceException =
                new InvalidTransactionReferenceException(foreignKeyConstraintConflictException);

            throw CreateAndLogDependencyValidationException(invalidTransactionReferenceException);
        }
        catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
        {
            var lockedTransactionException = new LockedTransactionException(dbUpdateConcurrencyException);

            throw CreateAndLogDependencyValidationException(lockedTransactionException);
        }
        catch (DbUpdateException databaseUpdateException)
        {
            var failedTransactionStorageException = new FailedTransactionStorageException(databaseUpdateException);

            throw CreateAndLogDependencyException(failedTransactionStorageException);
        }
        catch (Exception exception)
        {
            var failedTransactionServiceException = new FailedTransactionServiceException(exception);
            throw CreateAndLogServiceException(failedTransactionServiceException);
        }
    }

    private IQueryable<TransactionResponse> TryCatch(ReturningTransactionsFunction returningTransactionsFunction)
    {
        try
        {
            return returningTransactionsFunction();
        }
        catch (SqlException sqlException)
        {
            var failedTransactionStorageException = new FailedTransactionStorageException(sqlException);

            throw CreateAndLogCriticalDependencyException(failedTransactionStorageException);
        }
        catch (Exception exception)
        {
            var failedTransactionServiceException = new FailedTransactionServiceException(exception);

            throw CreateAndLogServiceException(failedTransactionServiceException);
        }
    }

    #endregion
}