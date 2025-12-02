using Expenses.API.Data.Services;
using Expenses.API.Dtos;
using Expenses.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Expenses.API.Controllers
{
    /// <summary>
    /// Controller responsible for handling user account-related operations such as login and registration.
    /// </summary>
    /// <param name="accountService">Service for handling account-related operations</param>
    /// <param name="logger">An ILogger property for capturing valuable information during runtime.</param>
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(
        IAccountService accountService,
        ILogger<AccountController> logger) : ControllerBase
    {
        #region Endpoints for Accounts
        
        /// <summary>
        /// Checks if an email is already registered in the system.
        /// </summary>
        /// <param name="email">Email to check use of</param>
        /// <returns>True if email already taken, false otherwise.</returns>
        /// <response code="200">Returns true if email is already taken, false otherwise.</response>
        /// <response code="404">If email is not found.</response>
        /// <response code="500">If an internal server error occurs.</response>
        /// <exception cref="Exception">Throws exception if an error occured during processing email check</exception>
        [Tags("Account")]
        [EndpointSummary("Checks if an email is already registered.")]
        [EndpointDescription("Checks if an email is already registered in the system.")]
        [EndpointName("IsEmailAlreadyTaken")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        [HttpGet(("IsEmailAlreadyTaken"))]
        public async Task<ActionResult<bool>> IsEmailAlreadyTaken(string email)
        {
            logger.LogInformation("Checking if email {Email} is already taken...", email);
            try
            {
                var result = await accountService.IsEmailAvailableAsync(email);
                if (result)
                    // Email is already taken in the database
                    return Ok(true);
                // Email is available to be used in the database
                return NotFound(false);
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
        /// <returns>A DTO object of type LoginResultDto containing the success status, message, and a Bear token (in JWT format).</returns>
        /// <response code="200">User has been logged in.</response>
        /// <response code="400">Login failed (bad request) If the request is invalid, e.g., missing email or password.</response>
        /// <response code="401">Login failed (unauthorized) If the credentials are invalid.</response>
        /// <response code="500">If an internal server error occurs.</response>
        /// <exception cref="Exception">Throws exception if an error occured during processing login</exception>
        [Tags("Account")]
        [EndpointSummary("Performs a user login.")]
        [EndpointDescription("Authenticates a user according to email and password.")]
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
        /// <param name="userCreateDto">A DTO object containing the user data for new user account.</param>
        /// <returns>A 201 - Created Status Code in case of success, with an object containing the token created</returns>
        /// <response code="201">User has been registered successfully with a JWT token sent to client.</response>
        /// <response code="400">Invalid data/request, e.g., missing email or password, or if the user already exists.</response>
        /// <response code="500">An internal server error occurs.</response>
        /// <exception cref="Exception">Throws exception if an error occured during processing registration</exception>
        [HttpPost("Register")]
        [Tags("Account")]
        [EndpointSummary("Registers a new user.")]
        [EndpointDescription("Registers a new user with email and password in the database.")]
        [EndpointName("Register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> Register([FromBody] UserCreateDto userCreateDto)
        {
            logger.LogInformation("Registering a new user with email: {Email}...", userCreateDto.Email);
            
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
                var creationLoginResult = await accountService.AddUserAsync(userCreateDto);
                if (!creationLoginResult.Success)
                {
                    logger.LogWarning("User registration failed: {Message}", creationLoginResult.Message);
                    
                    // Redisplay the client-readable error message to the client.
                    ModelState.AddModelError("message", creationLoginResult.Message ?? 
                                              "User registration failed. Please try again.");
                    return BadRequest(ModelState);
                }
                // Return a JSON result containing the JWT in the response.
                //return Ok(new {Token = creationLoginResult.Token});// Ok was used before
                return Created(string.Empty, creationLoginResult);
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
        /// Obtains the user by a given email
        /// </summary>
        /// <param name="email">The specified email</param>
        /// <returns>The user specified by the given email if found, or null otherwise.</returns>
        /// <response code="200">Specified user found and returned successfully.</response>
        /// <response code="404">Specified user with given email not found.</response>
        /// <response code="500">An Internal Server Error occurred while processing the request.</response>
        /// <exception cref="Exception">Throws exception if an error occurs while retrieving the transaction.</exception>
        [Tags("Account")]
        [EndpointSummary("Obtain user by given email from database")]
        [EndpointDescription("Retrieve a specified user by his email")]
        [EndpointName("UserProfile")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        [HttpGet("UserProfile")]
        public async Task<IActionResult> GetUserProfileByEmail(string email)
        {
            logger.LogInformation("Getting the profile of user:{0}", email);
            try
            {
                var existingUser = await accountService.GetUserProfile(email);

                if (existingUser != null)
                {
                    logger.LogInformation("User with Email:{0} is found.", email);
                    return Ok(existingUser);
                }
                logger.LogWarning("User with Email:{0} does not exist.", email);
                return NotFound($"User with Email:{email} not found!");
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "An error occurred while getting the user with email:{0}.", email);
                return BadRequest($"Email:{email} not found in the database!");
            }
        }
        
        /// <summary>
        /// Updates profile for user with given ID 
        /// </summary>
        /// <param name="userId">The unique ID of user</param>
        /// <param name="payload">User data to modify</param>
        /// <returns>Updated version of user profile</returns>
        /// <response code="200">User updated successfully</response>
        /// <response code="404">User with specified ID not found.</response>
        /// <response code="400">The provided payload is null or invalid.</response>
        /// <response code="500">An Internal Server Error occurred while processing the request.</response>
        [Tags("Account")]
        [EndpointSummary("Updates user profile by user ID")]
        [EndpointDescription("Updates the profile of a user identified by the given Id.")]
        [EndpointName("UpdateUserprofile")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        [HttpPut("UpdateUserProfile/{userId:int}")]
        public async Task<IActionResult> UpdateUserProfile(int userId, [FromBody] UserUpdateDto payload)
        {
            logger.LogInformation("Updating user profile for user with ID:{0}...", userId);
            
            // Check if the model is valid or not
            if (!ModelState.IsValid)
            {
                // Something failed with the incoming data model!
                logger.LogWarning("Invalid updating attempt.");
                // Redisplay the validation errors to the client.
                ModelState.AddModelError("message", " Valid firstName and lastName are required.");
                return BadRequest(ModelState);
            }
            
            // Proceed to update the user profile in the database
            try
            {
                var updateUser = await accountService.UpdateUserProfile(userId, payload);

                if (updateUser != null)
                {
                    logger.LogInformation("User profile for user with ID{0} updated successfully.", userId);
                    return Ok(updateUser);
                }
                logger.LogWarning("User does not exist in the database!");
                return NotFound("User not found in database!");
            }
            catch (Exception exception)
            {
                logger.LogError(exception,"An error occurred while updating user profile with ID:{0}.", userId);
                return BadRequest($"Error occurred while updating user profile with ID:{userId}");
            }
        }
        #endregion
    }
}
