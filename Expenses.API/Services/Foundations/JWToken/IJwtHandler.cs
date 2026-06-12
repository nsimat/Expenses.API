using Expenses.API.DTOs.UserAccounts;

namespace Expenses.API.Services.Foundations.JWToken;

/// <summary>
/// Contrat for generating a Web JSON Token
/// </summary>
public interface IJwtHandler
{
    /// <summary>
    /// Generates a JSON Web Token (JWT) for the specified user ID.
    /// </summary>
    /// <param name="userAccount">The user for whom the token is being generated.</param>
    /// <returns>A JWT as a string.</returns>
    string GenerateJwtToken(UserAccountResponse userAccount);
}