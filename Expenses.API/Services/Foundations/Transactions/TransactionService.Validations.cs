using System.Security.Claims;
using Expenses.API.Models;
using Expenses.API.Models.Exceptions.Transactions;
using Expenses.API.Models.Exceptions.UserAccounts;
using Xeptions;


namespace Expenses.API.Services.Foundations.Transactions;

public partial class TransactionService
{
    /// <summary>
    /// Validates the provided transaction, before insertion in the database, by checking if it is null and then validating
    /// its properties such as ID, Type, Category, Amount, CreatedAt, and UpdatedAt. It also checks if the UpdatedAt
    /// date is the same as the CreatedAt date and if the CreatedAt date is recent. If any of the validations fail,
    /// it throws an InvalidTransactionException with the corresponding error messages. This method is used to ensure
    /// that the transaction being added to the system meets the required criteria and prevents invalid data from being
    /// processed or stored in the database. 
    /// </summary>
    /// <param name="transaction">Transaction to be validated</param>
    private void ValidateTransactionOnAdd(Transaction transaction)
    {
        ValidateTransactionIsNotNull(transaction);

        Validate(
            (Rule: IsInvalid(transaction.Id), Parameter: nameof(Transaction.Id)),
            (Rule: IsInvalid(transaction.Type), Parameter: nameof(Transaction.Type)),
            (Rule: IsInvalid(transaction.Category), Parameter: nameof(Transaction.Category)),
            (Rule: IsInvalid(transaction.Amount), Parameter: nameof(Transaction.Amount)),
            (Rule: IsInvalid(transaction.UserId), Parameter: nameof(Transaction.UserId)),
            (Rule: IsInvalid(transaction.CreatedAt), Parameter: nameof(Transaction.CreatedAt)),
            (Rule: IsInvalid(transaction.UpdatedAt), Parameter: nameof(Transaction.UpdatedAt)),
            (Rule: IsCreationDateLaterThanUpdate(
                firstDate: transaction.CreatedAt,
                secondDate: transaction.UpdatedAt,
                secondDateName: nameof(Transaction.UpdatedAt)),
                Parameter: nameof(Transaction.CreatedAt)),
            (Rule: IsNotRecent(transaction.UpdatedAt), Parameter: nameof(Transaction.UpdatedAt))
        );
    }

    /// <summary>
    /// Validates the provided transaction, before update in the database, by checking if it is null and then validating
    /// its properties such as ID, Type, Category, Amount, CreatedAt, and UpdatedAt. It also checks if the UpdatedAt
    /// date is the same as the CreatedAt date and if the UpdatedAt date is recent. If any of the validations fail, it
    /// throws an InvalidTransactionException with the corresponding error messages. This method is used to ensure that
    /// the transaction being modified in the system meets the required criteria and prevents invalid data from being
    /// processed or stored in the database. 
    /// </summary>
    /// <param name="transaction">Transaction to be validated</param>
    private void ValidateTransactionOnModify(Transaction transaction)
    {
        ValidateTransactionIsNotNull(transaction);
        
        Validate(
            (Rule: IsInvalid(transaction.Id), Parameter: nameof(Transaction.Id)),
            (Rule: IsInvalid(transaction.Type), Parameter: nameof(Transaction.Type)),
            (Rule: IsInvalid(transaction.Category), Parameter: nameof(Transaction.Category)),
            (Rule: IsInvalid(transaction.Amount), Parameter: nameof(Transaction.Amount)),
            (Rule: IsInvalid(transaction.UserId), Parameter: nameof(Transaction.UserId)),
            (Rule: IsInvalid(transaction.CreatedAt), Parameter: nameof(Transaction.CreatedAt)),
            (Rule: IsInvalid(transaction.UpdatedAt), Parameter: nameof(Transaction.UpdatedAt)),
            (Rule: IsCreationDateLaterThanUpdate(
                firstDate: transaction.CreatedAt,
                secondDate: transaction.UpdatedAt,
                secondDateName: nameof(Transaction.UpdatedAt)),
                Parameter: nameof(Transaction.CreatedAt)),
            (Rule: IsNotRecent(transaction.UpdatedAt), Parameter: nameof(Transaction.UpdatedAt))
       );
    }

    /// <summary>
    /// Validates if the provided transaction ID is invalid by checking if it is equal to an empty GUID. An empty GUID
    /// is considered invalid because it does not represent a valid transaction ID. This validation is used to ensure
    /// that a transaction has a valid identifier before it is processed or stored in the database. 
    /// </summary>
    /// <param name="transactionId">ID of a transaction</param>
    private void ValidateTransactionId(Guid transactionId) =>
        Validate((Rule: IsInvalid(transactionId), Parameter: nameof(Transaction.Id)));

    /// <summary>
    /// Validates if the provided transaction exists in the storage by checking if it is null. If the transaction is null,
    /// it throws a NotFoundTransactionException with the provided transaction ID. This validation is used to ensure
    /// that a transaction with the specified ID exists in the database before performing any operations on it, such
    /// as updating or deleting. It helps to prevent operations on non-existent transactions and ensures that the
    /// system behaves correctly when a transaction is not found. 
    /// </summary>
    /// <param name="maybeTransaction">Transaction</param>
    /// <param name="transactionId">The transaction ID of the transaction to be updated or deleted</param>
    /// <exception cref="NotFoundTransactionException">Thrown when the transaction with the specified ID was not found.</exception>
    private static void ValidateStorageTransaction(Transaction maybeTransaction, Guid transactionId)
    {
        if (maybeTransaction is null || !maybeTransaction.Id.Equals(transactionId))
        {
            throw new NotFoundTransactionException(transactionId);    
        }
    }
    
    // Checks if the connected user is the one processing the creation of the transaction
    /// <summary>
    /// Validates that the user initiating the transaction creation or retrieval process has a non-null UserId.
    /// This method extracts the user identifier from the claims in the HTTP context. If the HTTP context or
    /// the claims identifier is null, or if the extracted UserId is invalid, it throws exceptions to indicate
    /// the issue.
    /// </summary>
    /// <param name="httpContextAccessor">
    /// The accessor to the current HTTP context, which provides access to the user's claims and session details
    /// necessary for validation.
    /// </param>
    /// <returns>The GUID representing the valid UserId of the initiating user.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the provided HTTP context accessor is null.</exception>
    /// <exception cref="NullUserAccountIdException">
    /// Thrown when the UserId is missing or cannot be parsed as a valid GUID.
    /// </exception>
    private static Guid ValidateUserAccountIdIsNotNull(IHttpContextAccessor httpContextAccessor)
    {
        if (httpContextAccessor is null)
        {
            throw new TransactionServiceException(new Xeption(message: "HTTP context accessor is null."));
        }
        //ArgumentNullException.ThrowIfNull(httpContextAccessor);

        var claimsIdentifier = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(claimsIdentifier) || !Guid.TryParse(claimsIdentifier, out Guid userId))
        {
            throw new NullUserAccountIdException();
        }
        
        return userId;
    }
    
    /// <summary>
    /// Validates if the provided transaction is null. If the transaction is null, it throws a NullTransactionException.
    /// This validation is used to ensure that a transaction object is not null before it is processed or stored in the
    /// database. It helps to prevent null reference exceptions and ensures that the system behaves correctly when a
    /// transaction is expected but not provided. 
    /// </summary>
    /// <param name="transaction">The transaction object to validate for null.</param>
    /// <exception cref="NullTransactionException"></exception>
    private static void ValidateTransactionIsNotNull(Transaction transaction)
    {
        if (transaction is null)
        {
            throw new NullTransactionException();
        }
    }

    /// <summary>
    /// Validates if the provided transaction ID is invalid by checking if it is equal to an empty GUID. An empty GUID is
    /// considered invalid because it does not represent a valid transaction ID. This validation is used to ensure that
    /// a transaction has a valid identifier before it is processed or stored in the database.  
    /// </summary>
    /// <param name="transactionId">The transaction identifier to validate.</param>
    /// <returns></returns>
    private static dynamic IsInvalid(Guid transactionId) => new
    {
        Condition = transactionId == Guid.Empty || !Guid.TryParse(transactionId.ToString(), out _),
        Message = "Transaction Id is required or is not valid."
    };

    /// <summary>
    /// Validates if the provided transaction type is invalid by checking if it is equal to the default value of the
    /// TransactionType enum. An invalid transaction type is considered to be the default value because it does not
    /// represent a valid transaction type. This validation is used to ensure that a transaction has a valid type
    /// before it is processed or stored in the database. 
    /// </summary>
    /// <param name="transactionAmount">Amount of the transaction to be validated</param>
    /// <returns></returns>
    private static dynamic IsInvalid(double transactionAmount) => new
    {
        Condition = transactionAmount == 0,
        Message = "Amount is required."
    };
    
    /// <summary>
    /// Validates if the provided transaction date is invalid by checking if it is equal to the default value of
    /// DateTimeOffset. An invalid transaction date is considered to be the default value because it does not represent
    /// a valid date and time. This validation is used to ensure that a transaction has a valid date before it is
    /// processed or stored in the database. 
    /// </summary>
    /// <param name="transactionDate">Date to be validated</param>
    /// <returns></returns>
    private static dynamic IsInvalid(DateTimeOffset transactionDate) => new
    {
        Condition = transactionDate == default,
        Message = "Date is required."
    };

    /// <summary>
    /// Determines if the provided input is invalid based on specific business rule checks.
    /// This method is overloaded to handle different types of inputs, including Guid, double, and string.
    /// </summary>
    /// <param name="typeCategory">A category of a transaction</param>
    /// <returns>
    /// A dynamic object containing a boolean condition indicating whether the input is invalid,
    /// as well as an error message highlighting the validation failure.
    /// </returns>
    private static dynamic IsInvalid(string typeCategory) => new
    {
        Condition = typeCategory == string.Empty,
        Message = "Category is required."
    };

    /// <summary>
    /// Validates that the specified dates are different. This method compares a given date to a second date
    /// and ensures that they are different. If both dates are the same, the validation passes; otherwise, it
    /// returns a validation failure with an appropriate error message.
    /// </summary>
    /// <param name="firstDate">The first date to compare.</param>
    /// <param name="secondDate">The second date to compare against the first date.</param>
    /// <param name="secondDateName">The name of the second date parameter, used in the error message.</param>
    /// <returns>
    /// A dynamic object containing a validation condition and an error message. The condition is true if the
    /// dates are the same, indicating a validation failure, and the message provides details about the failure.
    /// </returns>
    private static dynamic IsCreationDateLaterThanUpdate(
        DateTimeOffset firstDate, 
        DateTimeOffset secondDate, 
        string secondDateName) => new
    {
        Condition = firstDate >= secondDate,
        Message = $"The creation date of a transaction must be earlier than the update date."
    };
    
    /// <summary>
    /// Validates if the provided date is not recent by checking if the difference between the current date and the
    /// provided date is greater than one minute. This validation is used to ensure that the transaction date is
    /// within a reasonable time frame, preventing the creation of transactions with dates that are too far in
    /// the past or future.
    /// </summary>
    /// <param name="date">Date to be validated</param>
    /// <returns></returns>
    private dynamic IsNotRecent(DateTimeOffset date) => new
    {
        Condition = IsDateNotRecent(date),
        Message = "Date is not recent."
    };

    /// <summary>
    /// Checks if the provided userId is not null or empty by verifying if it equals Guid.Empty.
    /// Constructs and returns a result indicating whether the validation failed and provides an
    /// appropriate error message.
    /// </summary>
    /// <param name="userId">The identifier of the user to validate</param>
    /// <returns>
    /// A dynamic object containing a Condition indicating whether the userId is invalid,
    /// and a Message with a descriptive validation error if applicable.
    /// </returns>
    private static dynamic IsNotNull(Guid userId) => new // to review this method!!!!
    {
        Condition = userId == Guid.Empty,
        Message = "User Id is required."
    };

    /// <summary>
    /// Checks if the provided date is not recent by comparing it to the current date and time. A date is considered
    /// not recent if the difference between the current date and the provided date is greater than one minute.
    /// </summary>
    /// <param name="date">Date to be checked</param>
    /// <returns>return true if the date is not recent, false otherwise</returns>
    private bool IsDateNotRecent(DateTimeOffset date)// to be reviewed!!!
    {
        DateTimeOffset currentDateTime = _dateTimeBroker.GetCurrentDateTimeOffset();
        TimeSpan timeDifference = currentDateTime.Subtract(date);
        return timeDifference.Duration() > TimeSpan.FromMinutes(1);
    }

    /// <summary>
    /// Validates the provided rules and parameters by iterating through each rule and checking if the condition is
    /// true. If a condition is true, it adds the corresponding message to an InvalidTransactionException.
    /// After validating all rules, it throws the InvalidTransactionException if it contains any errors.
    /// This method is used to centralize the validation logic for transactions and ensure that all necessary
    /// validations are performed before processing a transaction.    
    /// </summary>
    /// <param name="validations">An array of rules to be validated</param>
    /// <exception cref="InvalidTransactionException">Thrown if any of the validation rules fail.</exception>
    private static void Validate(params (dynamic Rule, string Parameter)[] validations)
    {
        var invalidTransactionException = new InvalidTransactionException();

        foreach ((dynamic rule, string parameter) in validations)
        {
            if (rule.Condition)
            {
                invalidTransactionException.UpsertDataList(
                    key: parameter,
                    value: rule.Message
                    ?? throw new InvalidOperationException("Rule is not a valid dynamic object.")
                );
                Console.WriteLine($"Transaction validation failed for {parameter}: {rule.Message}!!!");
            }
        }
        invalidTransactionException.ThrowIfContainsErrors();
    }
}