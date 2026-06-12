using Expenses.API.DTOs.Transactions;
using Expenses.API.DTOs.UserAccounts;
using Expenses.API.Models;

namespace Expenses.API.DTOs.Mapping;

/// <summary>
/// Provides extension methods for mapping domain models to Data Transfer Objects (DTOs) in the Expenses API.
/// </summary>
public static class DomainToDtoMapper
{
    extension(Transaction value)
    {
        /// <summary>
        /// Converts a domain-level <see cref="Transaction"/> model to a DTO-level <see cref="TransactionResponse"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="TransactionResponse"/> object containing the mapped data from the <see cref="Transaction"/> model.
        /// Includes properties such as Id, Type, Amount, Category, UserId, CreatedAt, and UpdatedAt.
        /// </returns>
        public TransactionResponse ToTransactionDto()
        {
            return new TransactionResponse
            {
                Id = value.Id,
                Type = value.Type,
                Amount = value.Amount,
                Category = value.Category,
                CreatedAt = value.CreatedAt,
                UpdatedAt = value.UpdatedAt,
                UserId = value.UserId
            };
        }
    }

    extension(IQueryable<Transaction> transactions)
    {
        /// <summary>
        /// Converts a collection of domain-level <see cref="Transaction"/> models to a collection of DTO-level
        /// <see cref="TransactionResponse"/> objects.
        /// </summary>
        /// <returns>
        /// An <see cref="IQueryable{TransactionResponse}"/> containing the mapped data from the <see cref="Transaction"/> models.
        /// Each <see cref="TransactionResponse"/> includes properties such as Id, Type, Amount, Category, UserId,
        /// CreatedAt, and UpdatedAt.
        /// </returns>
        public IQueryable<TransactionResponse> ToTransactionDtos()
        {
            return transactions.Select(t => new TransactionResponse
            {
                Id = t.Id,
                Type = t.Type,
                Amount = t.Amount,
                Category = t.Category,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                UserId = t.UserId
            });
        }
    }

    extension(UserAccount value)
    {
        /// <summary>
        /// Converts a domain-level <see cref="UserAccount"/> model to a DTO-level <see cref="UserAccountResponse"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="UserAccountResponse"/> object containing the mapped data from the <see cref="UserAccount"/> model.
        /// Includes properties such as UserId, Email, FirstName, LastName, DateOfBirth, CreatedAt, UpdatedAt,
        /// RecentLoginTimes, and Transactions.
        /// </returns>
        public UserAccountResponse ToUserAccountDto()
        {
            return new UserAccountResponse()
            {
                UserId = value.Id,
                Email = value.Email,
                FirstName = value.FirstName,
                LastName = value.LastName,
                DateOfBirth = value.DateOfBirth,
                CreatedAt = value.CreatedAt,
                UpdatedAt = value.UpdatedAt,
                RecentLoginTimes = value.RecentLoginTimes,
                Transactions = value.Transactions.Select(t => t.ToTransactionDto()).ToList()
            };
        }   
    }

    extension(IQueryable<UserAccount> userAccounts)
    {
        /// <summary>
        /// Maps a queryable collection of domain-level <see cref="UserAccount"/> models to a queryable collection of
        /// DTO-level <see cref="UserAccountResponse"/> objects.
        /// </summary>
        /// <returns>
        /// A queryable collection of <see cref="UserAccountResponse"/> objects containing the mapped data from the
        /// <see cref="UserAccount"/> models.
        /// Includes properties such as UserId, Email, FirstName, LastName, DateOfBirth, CreatedAt, UpdatedAt, and
        /// RecentLoginTimes.
        /// </returns>
        public IQueryable<UserAccountResponse> ToUserAccountDtos()
        {
            return userAccounts.Select(u => new UserAccountResponse
            {
                UserId = u.Id,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                DateOfBirth = u.DateOfBirth,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
                RecentLoginTimes = u.RecentLoginTimes,
            });
        }   
    }
}