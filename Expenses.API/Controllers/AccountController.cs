using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Expenses.API.Data;
using Expenses.API.Dtos;
using Expenses.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Expenses.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(
        ExpensesDbContext expensesDbContext, 
        ILogger<AccountController> logger,
        IConfiguration configuration,
        PasswordHasher<User> passwordHasher) : ControllerBase
    {
        
        /// <summary>
        /// Authenticates a user and returns a JWT token if successful.
        /// </summary>
        /// <param name="userLogin">A DTO object containing the user's credentials.</param>
        /// <returns>An ActionResult of type LoginResultDto</returns>
        /// <response code="200">Returns a LoginResultDto object containing the success status, message, and JWT token.</response>
        /// <response code="400">If the request is invalid, e.g., missing email or password.</response>
        /// <response code="401">If the credentials are invalid.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [EndpointSummary("Performs a user login.")]
        [EndpointDescription("Authenticates a user with email and password.")]
        [EndpointName("Login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResultDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(LoginResultDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        [HttpPost("Login")]
        public async Task<ActionResult<LoginResultDto>> Login([FromBody] UserLoginDto userLogin)
        {
            logger.LogInformation("Attempting to log in user with email: {Email}...", userLogin.Email);
            
            // Validate the incoming user data
            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid login attempt.");
                return BadRequest("Email and password are required.");
            }

            try
            {
                // Check if the user exists
                var user = await expensesDbContext.Users.FirstOrDefaultAsync(u => u.Email == userLogin.Email);
                
                if (user == null)
                {
                    logger.LogWarning("Invalid credentials for email: {Email}.", userLogin.Email);
                    return Unauthorized(new LoginResultDto()
                    {
                        Success = false,
                        Message = "Invalid Email or Password."
                    });
                }

                // Verify the password matches
                var passwordVerificationResult = passwordHasher
                    .VerifyHashedPassword(user, user.Password, userLogin.Password);

                if (passwordVerificationResult == PasswordVerificationResult.Failed)
                {
                    logger.LogWarning("Invalid credentials for email: {Email}.", userLogin.Email);
                    return Unauthorized(new LoginResultDto()
                    {
                        Success = false,
                        Message = "Invalid Email or Password."
                    });
                }
                
                logger.LogInformation("User with email {Email} logged in successfully.", userLogin.Email);
                
                // Create a JWT as the given user credentials are valid.
                var jwtToken = GenerateJwtToken(user);
                
                var loginResult = new
                {
                    Success = true,
                    Message = "Login successful.",
                    Token = jwtToken
                };
                // Return a JSON result containing the success status, message, and JWT in the response.
                return Ok(loginResult);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while logging in!");
                return Problem(
                    detail:"An error occurred while processing your request.",
                    instance: HttpContext.TraceIdentifier,
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Internal Server Error"
               );
            }
        }
        
        /// <summary>
        /// Registers a new user with email and password.
        /// </summary>
        /// <param name="userCreationDto">A DTO object that can be used to create a new user account.</param>
        /// <returns>An object containing the token created</returns>
        /// <response code="201">Returns a JWT token if the registration is successful.</response>
        /// <response code="400">If the request is invalid, e.g., missing email or password, or if the user already exists.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpPost("Register")]
        [EndpointSummary("Registers a new user.")]
        [EndpointDescription("Registers a new user with email and password in the database.")]
        [EndpointName("Register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> Register([FromBody] UserCreationDto userCreationDto)
        {
            logger.LogInformation("Registering a new user with email: {Email}...", userCreationDto.Email);
            
            // Validate the incoming user data
            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid user registration attempt.");
                return BadRequest("Email and password are required.");
            }

            // Check if the user already exists and create a new user
            try
            {
                var existingUser = await expensesDbContext.Users.FirstOrDefaultAsync(u => u.Email == userCreationDto.Email);
                if (existingUser != null)
                {
                    logger.LogWarning("User with email {Email} already exists!", userCreationDto.Email);
                    return BadRequest("User with this email already exists!");
                }

                // Create a new user and save to the database
                var hashedPassword = passwordHasher.HashPassword(null!, userCreationDto.Password);
                
                var newUser = new User
                {
                    Email = userCreationDto.Email,
                    Password = hashedPassword,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                expensesDbContext.Users.Add(newUser);
                await expensesDbContext.SaveChangesAsync();

                logger.LogInformation("User with email {Email} registered successfully.", userCreationDto.Email);
                
                // Create a JWT as the given user credentials are valid.
                var token = GenerateJwtToken(newUser);
                
                // Return a JSON result containing the JWT in the response or a client-readable error message.
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
