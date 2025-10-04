using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Expenses.API.Data;
using Expenses.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Expenses.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(
        ExpensesDbContext expensesDbContext, 
        ILogger<AuthController> logger,
        IConfiguration configuration) : ControllerBase
    {
        
        [HttpPost("Register")]
        [EndpointSummary("Register a new user.")]
        [EndpointDescription("Registers a new user with email and password.")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> Register([FromBody] Dtos.UserCreationDto userCreationDto)
        {
            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid user registration attempt.");
                return BadRequest("Email and password are required.");
            }

            try
            {
                var existingUser = await expensesDbContext.Users.FirstOrDefaultAsync(u => u.Email == userCreationDto.Email);
                if (existingUser != null)
                {
                    logger.LogWarning("User with email {Email} already exists.", userCreationDto.Email);
                    return BadRequest("User with this email already exists.");
                }

                var newUser = new User
                {
                    Email = userCreationDto.Email,
                    Password = userCreationDto.Password, // In a real application, ensure to hash the password before storing it.
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                expensesDbContext.Users.Add(newUser);
                await expensesDbContext.SaveChangesAsync();

                logger.LogInformation("User with email {Email} registered successfully.", userCreationDto.Email);
                
                var token = GenerateJwtToken(newUser);
                
                return Ok(new {Token = token});
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while registering a new user.");
                return Problem(
                    detail:"An error occurred while processing your request.",
                    instance: HttpContext.TraceIdentifier,
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Internal Server Error"
               );
            }
        }
        /// <summary>
        ///  Generates a JWT token for the authenticated user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>String representing the token</returns>
        /// <exception cref="InvalidOperationException"></exception>
        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim( ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim( ClaimTypes.Email, user.Email)
            };
            
            // Implement JWT token generation logic
            var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                configuration["JwtSettings:SecurityKey"] ??
                throw new InvalidOperationException("JWT Secret Key not found in configuration."))); 
            
            var credentials = new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
            
            var token = new JwtSecurityToken(
                issuer: configuration["JwtSettings:Issuer"],
                audience: configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.
                    ToDouble(configuration["JwtSettings:ExpirationTimeInMinutes"] ?? "60")),
                signingCredentials: credentials
                );
            
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
