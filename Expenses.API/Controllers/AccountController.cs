using Expenses.API.Data;
using Expenses.API.Data.Services;
using Expenses.API.Dtos;
using Expenses.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Expenses.API.Controllers
{
    /// <summary>
    /// Controller responsible for handling user account-related operations such as login and registration.
    /// </summary>
    /// <param name="expensesDbContext">The EF Core database context property</param>
    /// <param name="accountService">Service for handling account-related operations</param>
    /// <param name="logger">An ILogger property for capturing valuable information during runtime.</param>
    /// <param name="configuration">Application configuration property</param>
    /// <param name="passwordHasher">Identity password hashing property.</param>
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(
        ExpensesDbContext expensesDbContext, 
        IAccountService accountService,
        ILogger<AccountController> logger,
        IConfiguration configuration,
        PasswordHasher<User> passwordHasher) : ControllerBase
    {
        #region Endpoints for Accounts
        
        /// <summary>
        /// Checks if an email is already registered in the system.
        /// </summary>
        /// <param name="email">Email to check use of</param>
        /// <returns>True if email already taken, false otherwise.</returns>
        /// <response code="200">Returns true if email is already taken, false otherwise.</response>
        /// <response code="400">If the request is invalid, e.g., missing email.</response>
        /// <response code="500">If an internal server error occurs.</response>
        /// <exception cref="Exception">Throws exception if an error occured during processing email check</exception>
        [EndpointSummary("Checks if an email is already registered.")]
        [EndpointDescription("Checks if an email is already registered in the system.")]
        [EndpointName("IsEmailAlreadyTaken")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        [HttpGet(("IsEmailAlreadyTaken"))]
        public async Task<ActionResult<bool>> IsEmailAlreadyTaken(string email)
        {
            logger.LogInformation("Checking if email {Email} is already taken...", email);

            try
            {
                var result = await accountService.IsEmailAvailableAsync(email);
                return Ok(result);
            }
            catch (Exception exception)
            {
                logger.LogError("An error occurred while checking email availability: {Message}", exception.Message);
                return Problem(
                    detail:"An error occurred while processing your request.",
                      instance: HttpContext.TraceIdentifier,
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Internal Server Error"
               );
            }
        }
        
        /// <summary>
        /// Authenticates a user and returns a JWT token if successful.
        /// </summary>
        /// <param name="apiLoginRequest">A DTO object containing the user's credentials.</param>
        /// <returns>A DTO object of type LoginResultDto</returns>
        /// <response code="200">Returns a LoginResultDto object containing the success status, message, and JWT token.</response>
        /// <response code="400">If the request is invalid, e.g., missing email or password.</response>
        /// <response code="401">If the credentials are invalid.</response>
        /// <response code="500">If an internal server error occurs.</response>
        /// <exception cref="Exception">Throws exception if an error occured during processing login</exception>
        [EndpointSummary("Performs a user login.")]
        [EndpointDescription("Authenticates a user with email and password.")]
        [EndpointName("Login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiLoginResultDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiLoginResultDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        [HttpPost("Login")]
        public async Task<ActionResult<ApiLoginResultDto>> Login([FromBody] ApiLoginRequestDto apiLoginRequest)
        {
            logger.LogInformation("Attempting to log in user with email: {Email}...", apiLoginRequest.Email);
            
            // Check if the model is valid or not
            if (!ModelState.IsValid)
            {
                // Something failed with the incoming data model!
                logger.LogWarning("Invalid login attempt.");
                // Redisplay the validation errors to the client.
                ModelState.AddModelError("message", " Valid email and password are required.");
                return BadRequest(ModelState);
            }

            try
            {
                // Process to identify the user by email and password
                var loginResult = await accountService.IdentifyUserAsync(apiLoginRequest);

                if (!loginResult.Success)
                    // Return a JSON result containing the success status and message only.
                    return Unauthorized(loginResult);
                
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
        /// <exception cref="Exception">Throws exception if an error occured during processing registration</exception>
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
            
            // Check if the model is valid
            if (!ModelState.IsValid)
            {
                // Something is wrong with the incoming data model!
                logger.LogWarning("Invalid user registration attempt.");
                
                // Redisplay the client-readable message to the client.
                ModelState.AddModelError("message", "Invalid user registration attempt.");
                return BadRequest(ModelState);
            }
            
            try
            {
                var creationLoginResult = await accountService.AddUserAsync(userCreationDto);
                if (!creationLoginResult.Success)
                {
                    logger.LogWarning("User registration failed: {Message}", creationLoginResult.Message);
                    
                    // Redisplay the client-readable error message to the client.
                    ModelState.AddModelError("message", creationLoginResult.Message ?? 
                                              "User registration failed. Please try again.");
                    return BadRequest(ModelState);
                }
                // Return a JSON result containing the JWT in the response.
                return Ok(new {Token = creationLoginResult.Token});
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
        #endregion
    }
}
