using System.Text.Json.Serialization;
using Expenses.API.DTOs.Transactions;

namespace Expenses.API.DTOs.UserAccounts;

/// <summary>
/// Data transfer object for representing user account details.
/// </summary>
/// <remarks>
/// This class is designed to facilitate the transfer of user account information
/// between layers of the application. It includes key user details such as email,
/// name, date of birth, and associated transactions.
/// </remarks>
public class UserAccountResponse
{
    /// <summary>
    /// Unique identifier for the user.
    /// </summary>
    [JsonPropertyName("id")]
    public Guid UserId { get; set; } = Guid.Empty;
    
    /// <summary>
    /// Email of the user
    /// </summary>
    [JsonPropertyName("email")]
    public required string Email { get; set; } = string.Empty;
 
    /// <summary>
    /// The first name of the user
    /// </summary>
    [JsonPropertyName("first_name")]
    public string? FirstName { get; set; }

    /// <summary>
    /// The last name of the user
    /// </summary>
    [JsonPropertyName("last_name")]
    public string? LastName { get; set; }

    /// <summary>
    /// Stores the hashed representation of the user's password.
    /// </summary>
    /// <remarks>
    /// This property is intended to securely store the password in its hashed form,
    /// ensuring that plain-text passwords are not stored or exposed.
    /// </remarks>
    [JsonIgnore]
    public string PasswordHashed { get; set; }

    /// <summary>
    /// The date of birth of the user
    /// </summary>
    [JsonPropertyName("date_of_birth")]
    public DateTimeOffset? DateOfBirth { get; set; }

    /// <summary>
    /// The date and time when the account was created.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// The timestamp indicating when the record was last updated.
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// List of timestamps representing the most recent login times of the user.
    /// </summary>
    [JsonPropertyName("recent_login_times")]
    public List<DateTimeOffset>? RecentLoginTimes { get; set; }

    /// <summary>
    /// List of transactions associated with the user
    /// </summary>
    [JsonPropertyName("user_transactions")]
    public ICollection<TransactionResponse> Transactions { get; set; } = new HashSet<TransactionResponse>();
}