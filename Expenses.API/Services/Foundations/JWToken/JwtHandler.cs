using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoInject.Attributes.ScopedAttributes;
using Expenses.API.DTOs.UserAccounts;
using Microsoft.IdentityModel.Tokens;

namespace Expenses.API.Services.Foundations.JWToken;

/// <summary>
/// Service class for handling JWT operations
/// </summary>
[Scoped(typeof(IJwtHandler))]
public class JwtHandler : IJwtHandler
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtHandler> _logger;

    /// <summary>
    /// JWT Handler constructor
    /// </summary>
    /// <param name="configuration">Configuration object to inject in the constructor</param>
    /// <param name="logger">Logger object to inject to the constructor</param>
    /// <exception cref="ArgumentNullException">Exception generated if object injected is null.</exception>
    public JwtHandler(IConfiguration configuration, ILogger<JwtHandler> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    /// <summary>
    ///  Generates a JWT token for the authenticated user.
    /// </summary>
    /// <param name="userAccount">User for whom JWT token is generated</param>
    /// <returns>String representing the token</returns>
    public string GenerateJwtToken(UserAccountResponse userAccount)
    {
        _logger.LogInformation("Generating JWT token for user with email: {Email}", userAccount.Email);
        
        var claims = new[]
        {
            new Claim( ClaimTypes.NameIdentifier, userAccount.UserId.ToString()),
            new Claim( ClaimTypes.Email, userAccount.Email)
        };
            
        // Implement JWT token generation logic
        var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["JwtSettings:SecurityKey"] ??
            throw new InvalidOperationException("JWT Secret Key not found in configuration."))); 
            
        var credentials = new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
            
        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.
                ToDouble(_configuration["JwtSettings:ExpirationTimeInMinutes"] ?? "60")),
            signingCredentials: credentials
        );
            
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}