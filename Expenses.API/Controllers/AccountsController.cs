using Expenses.API.Brokers.Loggings;
using Expenses.API.DTOs.UserAccounts;
using Expenses.API.Models;
using Expenses.API.Models.Exceptions.UserAccounts;
using Expenses.API.Services.Foundations.Accounts;
using Expenses.API.Services.Foundations.JWToken;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace Expenses.API.Controllers
{
    /// <summary>
    /// Controller responsible for handling user account-related operations such as login and registration.
    /// </summary>
    /// <param name="userAccountService">Service for handling account-related operations</param>
    /// <param name="loggingBroker">An ILoggingBroker property for logging information, warnings, and errors.</param>
    /// <param name="jwtHandler">An IJwtHandler property for generating JWT tokens.</param>   
    [Route("api/v1/users")]
    [ApiController]
    public class AccountsController(
        IUserAccountService userAccountService,
        ILoggingBroker loggingBroker,
        IJwtHandler jwtHandler) : RESTFulController
    {
        #region Endpoints for Accounts CRUD operations

        // GET: api/uses/v1/is-already-taken?email={email}
        /// <summary>
        /// Checks if an email is already registered in the system.
        /// </summary>
        /// <param name="email">Email to check the use of</param>
        /// <returns>True if email already taken, false otherwise.</returns>
        /// <response code="200">Returns true if email is already taken, false otherwise.</response>
        /// <response code="404">If email is not found.</response>
        /// <response code="500">If an internal server error occurs.</response>
        /// <exception cref="Exception">Throws exception if an error occurred during processing email check</exception>
        [Tags("UserAccounts")]
        [EndpointSummary("Checks if an email is already registered.")]
        [EndpointDescription("Checks if an email is already registered in the system.")]
        [EndpointName("IsEmailAlreadyTaken")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        [HttpGet(("is-email-already-taken"))]
        public async ValueTask<ActionResult<bool>> IsEmailAlreadyTaken(string email) // to be reviewed!!!
        {
            loggingBroker.LogInformation($"Attempting to check if email {email} is already taken...");

            try
            {
                var maybeUserAccount = await userAccountService.IsEmailAvailableAsync(email);

                return maybeUserAccount != null ? Ok(true) : NotFound(false);

                // Possible exceptions:
                // return InvalidUserAccountException.Create(email);
                // return NotFoundUserAccountException(false);
                // return UserAccountServiceException.Create("An error occurred while checking email availability.");
            }
            catch (InvalidUserAccountException invalidUserAccountException)
            {
                loggingBroker.LogWarning($"Invalid email format: {invalidUserAccountException.Message}");

                return BadRequest(invalidUserAccountException.Message);
            }
            catch (NotFoundUserAccountException notFoundUserAccountException)
            {
                loggingBroker.LogWarning($"Email '{email}' is available for registration!");

                return NotFound(notFoundUserAccountException.Message);
            }
            catch (UserAccountDependencyException userAccountDependencyException)
            {
                loggingBroker.LogError(userAccountDependencyException);

                return Problem(
                    detail: "An error occurred while processing your request.",
                    instance: HttpContext.TraceIdentifier,
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Internal Server Error"
                );
            }
            catch (UserAccountServiceException userAccountServiceException)
            {
                loggingBroker.LogError(userAccountServiceException);

                return Problem(
                    detail: "An error occurred while processing your request.",
                    instance: HttpContext.TraceIdentifier,
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Internal Server Error"
                );
            }
        }

        // POST: api/v1/users/login
        /// <summary>
        /// Performs a user login.
        /// </summary>
        /// <param name="userLogin">A DTO object containing the user's credentials.</param>
        /// <returns>A DTO object of type LoginResultDto containing the success status, message, and a Bear token (in JWT format).</returns>
        /// <response code="200">User has been logged in.</response>
        /// <response code="400">Login failed (bad request) If the request is invalid, e.g., missing email or password.</response>
        /// <response code="401">Login failed (unauthorized) If the credentials are invalid.</response>
        /// <response code="500">If an internal server error occurs.</response>
        /// <exception cref="UserAccountValidationException">Throws exception if the user data is invalid.</exception>
        /// <exception cref="UserAccountDependencyValidationException">Throws exception if the user data is invalid.</exception>
        /// <exception cref="UserAccountDependencyException">Throws exception if an error occurs while logging in the user.</exception>
        /// <exception cref="UserAccountServiceException">Throws exception if an error occurs while logging in the user.</exception>       
        [Tags("UserAccounts")]
        [EndpointSummary("Performs a user login.")]
        [EndpointDescription("Authenticates a user according to email and password.")]
        [EndpointName("Login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserLoginResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(UserLoginResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UserLoginResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        [HttpPost("login")]
        public async ValueTask<ActionResult<UserLoginResponse>> Login([FromBody] UserLoginRequest userLogin)
        {
            loggingBroker.LogInformation($"Attempting to log in user with email: {userLogin.Email}...");

            try
            {
                // validate the user credentials and retrieve the user account if found, or NULL otherwise.
                var identifiedUserAccount = await userAccountService.IdentifyUserAccountAsync(userLogin);
                /*if (!loginResult.Success)
                    // Return a JSON result containing the success status and message only.
                    return Unauthorized(loginResult);*/                

                // Create a JWT token for the identified user account
                var jwtToken = jwtHandler.GenerateJwtToken(identifiedUserAccount);

                // Return the login result with the generated JWT token as the login is successful.
                return Ok(new UserLoginResponse()
                {
                    Success = true,
                    Message = "Login successful.",
                    Token = jwtToken
                });

                // List of possible exceptions to be handled in the service layer and caught here:
                // - InvalidUserAccountException --> UserAccountValidationException(InvalidUserAccountException)
                // - NullUserAccountException --> UserAccountValidationException(NullUserAccountException)
                // - NotFoundUserAccountException --> UserAccountValidationException(NotFoundUserAccountException)                
            }
            catch (UserAccountValidationException userAccountValidationException)
                when (userAccountValidationException.InnerException is NotFoundUserAccountException)
            {
                loggingBroker.LogWarning($"User login failed: {userAccountValidationException.Message}");

                // Redisplay the validation errors to the client.
                return Unauthorized(new UserLoginResponse()
                {
                    Success = false,
                    Message = userAccountValidationException.Message,
                    Token = null
                });
            }
            catch (UserAccountValidationException userAccountValidationException)
            {
                loggingBroker.LogWarning($"User login failed: {userAccountValidationException.Message}");
                
                return BadRequest(new UserLoginResponse()
                {
                    Success = false,
                    Message = userAccountValidationException.Message,
                    Token = null
                });
            }
            catch (UserAccountDependencyValidationException userAccountValidationException)
                when (userAccountValidationException.InnerException is InvalidUserAccountReferenceException)
            {
                return FailedDependency(userAccountValidationException.InnerException);
            }
            catch (UserAccountDependencyException userAccountDependencyException)
            {
                return InternalServerError(userAccountDependencyException);
            }
            catch (UserAccountServiceException userAccountServiceException)
            {
                return InternalServerError(userAccountServiceException);
            }
        }

        // POST: api/v1/users/register
        /// <summary>
        /// Registers a new user with email and password.
        /// </summary>
        /// <param name="userRegisterRequest">A DTO object containing the user data for a new user account.</param>
        /// <returns>A 201-Created Status Code in case of success, with an object containing the token created</returns>
        /// <response code="201">User has been registered successfully with a JWT token sent to the client.</response>
        /// <response code="400">Invalid data/request, e.g., missing email or password, or if the user already exists.</response>
        /// <response code="500">An internal server error occurs.</response>
        /// <exception cref="UserAccountValidationException">Throws exception if the user data is invalid.</exception>
        /// <exception cref="UserAccountDependencyValidationException">Throws exception if the user data is invalid.</exception>       
        /// <exception cref="UserAccountDependencyException">Throws exception if an error occurs while registering the user.</exception>
        /// <exception cref="UserAccountServiceException">Throws exception if an error occurs while registering the user.</exception>
        [HttpPost("register")]
        [Tags("UserAccounts")]
        [EndpointSummary("Registers a new user.")]
        [EndpointDescription("Registers a new user with email and password in the database.")]
        [EndpointName("Register")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserLoginResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(UserLoginResponse))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status424FailedDependency, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        public async ValueTask<IActionResult> Register([FromBody] UserRegisterRequest userRegisterRequest)
        {
            loggingBroker.LogInformation($"Registering a new user with email: {userRegisterRequest.Email}...");

            try
            {
                var createdUserAccount = await userAccountService.AddUserAccountAsync(userRegisterRequest);

                var jwtToken = jwtHandler.GenerateJwtToken(createdUserAccount);

                var successResponse = new UserLoginResponse()
                {
                    Success = true,
                    Message = "User registered successfully.",
                    Token = jwtToken
                };

                return Created(string.Empty, successResponse);

                // List of possible exceptions to be handled in the service layer and caught here:
                // - NullUserAccountException --> UserAccountValidationException(NullUserAccountException)
                // - InvalidUserAccountException --> UserAccountValidationException(InvalidUserAccountException)
                // - SQlException --> UserAccountDependencyException(SqlException)
                // - NotFoundUserAccountException --> UserAccountValidationException(NotFoundUserAccountException)
                // - DuplicateKeyException --> UserAccountDependencyValidationException(AlreadyExistsUserAccountException)
                // - ForeignKeyConstraintConflictException --> UserAccountDependencyValidationException(InvalidUserAccountReferenceException)
                // - DbUpdateConcurrencyException --> UserAccountDependencyValidationException(LockedUserAccountException)
                // - DbUpdateException --> UserAccountDependencyException(FailedUserAccountStorageException)
                // - Exception --> UserAccountServiceException(UserAccountServiceException)

                // From Service Layer:
                // - UserAccountValidationException --> BadRequest(UserAccountValidationException.InnerException)
                // - UserAccountDependencyException --> InternalServerError(UserAccountDependencyException.InnerException)
                // - UserAccountDependencyValidationException --> UserAccountServiceException(UserAccountServiceException)
                // - UserAccountServiceException --> UserAccountServiceException(UserAccountServiceException)                
            }
            catch (UserAccountValidationException userAccountValidationException)
            {
                var failureResponse = new UserLoginResponse()
                {
                    Success = false,
                    Message = userAccountValidationException.Message,
                    Token = null
                };

                loggingBroker.LogWarning($"User registration failed: {failureResponse.Message}");

                // Redisplay the validation errors to the client.
                return BadRequest(failureResponse);
            }
            catch (UserAccountDependencyValidationException userAccountValidationException)
                when (userAccountValidationException.InnerException is InvalidUserAccountReferenceException)
            {
                return FailedDependency(userAccountValidationException.InnerException);
            }
            catch (UserAccountDependencyValidationException userAccountDependencyValidationException)
                when (userAccountDependencyValidationException.InnerException is AlreadyExistsUserAccountException)
            {
                loggingBroker
                    .LogWarning(
                        $"User registration failed: {userAccountDependencyValidationException.InnerException.Message}");

                return Conflict(userAccountDependencyValidationException.InnerException);
            }
            catch (UserAccountDependencyException userAccountDependencyException)
            {
                return InternalServerError(userAccountDependencyException);
            }
            catch (UserAccountServiceException userAccountServiceException)
            {
                return InternalServerError(userAccountServiceException);
            }
        }

        // GET: api/v1/users/profile?email={email}
        /// <summary>
        /// Gets the user by a given email
        /// </summary>
        /// <param name="email">The specified email</param>
        /// <returns>The user specified by the given email if found, or null otherwise.</returns>
        /// <response code="200">Specified user found and returned successfully.</response>
        /// <response code="404">Specified user with given email not found.</response>
        /// <response code="400">Invalid email format.</response>
        /// <response code="500">An Internal Server Error occurred while processing the request.</response>
        /// <exception cref="UserAccountValidationException">Throws exception if the email is invalid.</exception>
        /// <exception cref="NotFoundUserAccountException">Throws exception if the user is not found.</exception>
        /// <exception cref="UserAccountDependencyException">
        /// Throws exception if an error occurs while retrieving the user.
        /// </exception>
        /// <exception cref="UserAccountServiceException">
        /// Throws exception if an error occurs while retrieving the user.
        /// </exception>
        [Tags("UserAccounts")]
        [EndpointSummary("Obtain user by given email from database")]
        [EndpointDescription("Retrieve a specified user by his email")]
        [EndpointName("UserProfile")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserAccount))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        [HttpGet("profile")]
        public async ValueTask<IActionResult> GetUserProfileByEmail(string email)
        {
            loggingBroker.LogInformation($"Getting the profile of user:{email}...");

            try
            {
                var retrievedProfile = await userAccountService.RetrieveUserProfileAsync(email);

                loggingBroker.LogInformation($"User with email:{email} retrieved successfully.");

                return Ok(retrievedProfile);
            }
            catch (UserAccountValidationException userAccountValidationException)
                when (userAccountValidationException.InnerException is NotFoundUserAccountException)
            {
                loggingBroker.LogWarning($"User with email:{email} not found!");

                return NotFound(userAccountValidationException.InnerException);
            }
            catch (UserAccountValidationException userAccountValidationException)
            {
                loggingBroker.LogWarning($"Invalid email format: {userAccountValidationException.Message}");

                return BadRequest(userAccountValidationException.InnerException);
            }
            catch (UserAccountDependencyException userAccountDependencyException)
            {
                return InternalServerError(userAccountDependencyException);
            }
            catch (UserAccountServiceException userAccountServiceException)
            {
                return InternalServerError(userAccountServiceException);
            }
        }

        // PUT: api/v1/users/{userId}/update-profile
        /// <summary>
        /// Updates the profile of a user with a given ID 
        /// </summary>
        /// <param name="userId">The unique ID of user.</param>
        /// <param name="payload">User data to modify</param>
        /// <returns>Updated version of user profile</returns>
        /// <response code="200">User updated successfully.</response>
        /// <response code="404">User with specified ID not found.</response>
        /// <response code="400">The provided payload is null or invalid.</response>
        /// <response code="500">An Internal Server Error occurred while processing the request.</response>
        /// <exception cref="UserAccountValidationException">
        /// Throws exception if the provided user data is invalid.
        /// </exception>
        /// <exception cref="UserAccountDependencyException">
        /// Throws exception if an error occurs while updating the user profile.
        /// </exception>
        /// <exception cref="UserAccountServiceException">
        /// Throws exception if an error occurs while updating the user profile.
        /// </exception>
        [Tags("UserAccounts")]
        [EndpointSummary("Updates user profile by user ID")]
        [EndpointDescription("Updates the profile of a user identified by the given Id.")]
        [EndpointName("UpdateUserProfile")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserAccount))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
        [HttpPut("{userId:guid}/update-profile")]
        public async ValueTask<IActionResult> UpdateUserProfile(Guid userId, [FromBody] UserUpdateRequest payload)
        {
            loggingBroker.LogInformation($"Updating user profile for user with ID:{userId}...");

            try
            {
                var modifiedUserAccount = await userAccountService.ModifyUserProfileAsync(userId, payload);

                return Ok(modifiedUserAccount);
            }
            catch (UserAccountValidationException userAccountValidationException)
                when (userAccountValidationException.InnerException is NotFoundUserAccountException)
            {
                loggingBroker.LogWarning($"User with ID:{userId} not found in the database!");

                return NotFound(userAccountValidationException.InnerException);
            }
            catch (UserAccountValidationException userAccountValidationException)
            {
                loggingBroker.LogWarning($"Invalid email format: {userAccountValidationException.Message}");

                return BadRequest(userAccountValidationException.InnerException);
            }
            catch (UserAccountDependencyValidationException userAccountValidationException)
                when (userAccountValidationException.InnerException is InvalidUserAccountReferenceException)
            {
                return FailedDependency(userAccountValidationException.InnerException);
            }
            catch (UserAccountDependencyValidationException userAccountValidationException)
                when (userAccountValidationException.InnerException is AlreadyExistsUserAccountException)
            {
                return Conflict(userAccountValidationException.InnerException);
            }
            catch (UserAccountDependencyException userAccountDependencyException)
            {
                return InternalServerError(userAccountDependencyException);
            }
            catch (UserAccountServiceException userAccountServiceException)
            {
                return InternalServerError(userAccountServiceException);
            }
        }

        #endregion
    }
}