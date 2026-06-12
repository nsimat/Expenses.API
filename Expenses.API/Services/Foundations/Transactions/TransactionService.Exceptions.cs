using EFxceptions.Models.Exceptions;
using Expenses.API.DTOs.Transactions;
using Expenses.API.Models.Exceptions.Transactions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace Expenses.API.Services.Foundations.Transactions;

public partial class TransactionService
{
    /// <summary>
    /// Delegate that defines a function returning a single Transaction, used for centralizing exception handling logic
    /// in the service methods.
    /// </summary>
    private delegate ValueTask<TransactionResponse> ReturningTransactionFunction();
    
    /// <summary>
    /// Delegate that defines a function returning a collection of Transactions, used for centralizing exception
    /// handling logic.
    /// </summary>
    private delegate IQueryable<TransactionResponse> ReturningTransactionsFunction();
    
    /// <summary>
    /// Centralized method for handling exceptions that may occur during service operations that return a single
    /// Transaction.
    /// </summary>
    /// <param name="returningTransactionFunction">A delegate that returns a transaction object</param>
    /// <returns>The instance of the transaction</returns>
    /// <exception cref="TransactionValidationException"></exception>
    /// <exception cref="TransactionDependencyException"></exception>
    /// <exception cref="TransactionDependencyValidationException"></exception>
    /// <exception cref="TransactionServiceException"></exception>
    private async ValueTask<TransactionResponse> TryCatch(ReturningTransactionFunction returningTransactionFunction)
    {
        try
        {
            return await returningTransactionFunction();
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

    /// <summary>
    /// Centralized method for handling exceptions that may occur during service operations that return a collection
    /// of Transactions.
    /// </summary>
    /// <param name="returningTransactionsFunction">A delegate that returns a collection of transactions</param>
    /// <returns>A list of TransactionDto objects</returns>
    /// <exception cref="TransactionDependencyException">#</exception>
    /// <exception cref="TransactionServiceException">#</exception>
    private IQueryable<TransactionResponse> TryCatch(ReturningTransactionsFunction returningTransactionsFunction)
    {
        try
        {
            return returningTransactionsFunction();
        }
        catch (SqlException exception)
        {
            var  failedTransactionStorageException = new FailedTransactionStorageException(exception);
            
            throw CreateAndLogCriticalDependencyException(failedTransactionStorageException);
        }
        catch(Exception exception)
        {
            var  failedTransactionServiceException = new FailedTransactionServiceException(exception);
            
            throw CreateAndLogServiceException(failedTransactionServiceException);
        }
    }

    /// <summary>
    /// Creates a <see cref="TransactionValidationException"/> and logs the exception using the logging broker.
    /// </summary>
    /// <param name="exception">The exception causing the validation error.</param>
    /// <returns>
    /// A new instance of <see cref="TransactionValidationException"/> encapsulating the provided exception.
    /// </returns>
    private TransactionValidationException CreateAndLogValidationException(Xeption exception)
    {
        var transactionValidationException = new TransactionValidationException(exception);
        
        _loggingBroker.LogError(transactionValidationException);
        
        return transactionValidationException;
    }

    /// <summary>
    /// Creates and logs a critical dependency exception based on the provided exception and returns the newly created
    /// exception.
    /// </summary>
    /// <param name="exception">The exception that caused the critical dependency error.</param>
    /// <returns>A <see cref="TransactionDependencyException"/> that encapsulates the original exception.</returns>
    private TransactionDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
    {
        var transactionDependencyException = new TransactionDependencyException(exception);
        
        _loggingBroker.LogCritical(transactionDependencyException);
        
        return transactionDependencyException;
    }

    /// <summary>
    /// Creates and logs a <see cref="TransactionDependencyValidationException"/> based on the provided exception.
    /// </summary>
    /// <param name="exception">
    /// The exception that triggered the creation of the dependency validation exception.
    /// </param>
    /// <returns>
    /// An instance of <see cref="TransactionDependencyValidationException"/> containing the logged error details.
    /// </returns>
    private TransactionDependencyValidationException CreateAndLogDependencyValidationException(Xeption exception)
    {
        var transactionDependencyValidationException = new TransactionDependencyValidationException(exception);
        
        _loggingBroker.LogError(transactionDependencyValidationException);
        
        return transactionDependencyValidationException;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="TransactionDependencyException"/> with the specified exception,
    /// logs the error using the logging broker, and returns the exception instance.
    /// </summary>
    /// <param name="exception">The exception that triggered the dependency exception.</param>
    /// <returns>An instance of <see cref="TransactionDependencyException"/>.</returns>
    private TransactionDependencyException CreateAndLogDependencyException(Xeption exception)
    {
        var transactionDependencyException = new TransactionDependencyException(exception);
        
        _loggingBroker.LogError(transactionDependencyException);
        
        return transactionDependencyException;
    }

    /// <summary>
    /// Creates and logs a service exception specific to TransactionService by wrapping the provided exception
    /// and logging its details for diagnostic purposes.
    /// </summary>
    /// <param name="exception">The exception to be wrapped and logged</param>
    /// <returns>A new instance of <see cref="TransactionServiceException"/> containing the wrapped exception</returns>
    private TransactionServiceException CreateAndLogServiceException(Xeption exception)
    {
        var transactionServiceException = new TransactionServiceException(exception);
        
        _loggingBroker.LogError(transactionServiceException);
        
        return transactionServiceException;
    }
}