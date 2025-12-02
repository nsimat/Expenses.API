using Expenses.API.Dtos;
using Expenses.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Expenses.API.Data.Services;

/// <summary>
/// This service handles user account related operations such as registration, login, and
/// email availability checks.
/// </summary>
public class AccountService : IAccountService
{
    /// <summary>
    /// Context for accessing the database.
    /// </summary>
    private readonly ExpensesDbContext _context;

    /// <summary>
    /// Password hasher for securely hashing and verifying passwords.
    /// </summary>
    private readonly PasswordHasher<User> _passwordHasher;

    /// <summary>
    /// Application configuration property
    /// </summary>
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Logger for logging information and errors.
    /// </summary>
    private readonly ILogger<AccountService> _logger;

    /// <summary>
    /// JWT handler for generating and validating JWT tokens.
    /// </summary>
    private readonly JwtHandler _jwtHandler;

    /// <summary>
    /// Constructor to initialize the AccountService with necessary dependencies.
    /// </summary>
    /// <param name="context">The context for database accessing</param>
    /// <param name="passwordHasher">Password hasher to initialize the service constructor</param>
    /// <param name="configuration">Configuration to initialize the service constructor</param>
    /// <param name="logger">Logger to initialize the service constructor</param>
    /// <param name="jwtHandler">JWT handler to initialize the service constructor</param>
    public AccountService(
        ExpensesDbContext context,
        PasswordHasher<User> passwordHasher,
        IConfiguration configuration,
        ILogger<AccountService> logger,
        JwtHandler jwtHandler)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _jwtHandler = jwtHandler ?? throw new ArgumentNullException(nameof(jwtHandler));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="apiLoginRequest">A DTO containing user's credentials</param>
    /// <returns>User if found in the database, null otherwise</returns>
    public async Task<User?> FindUserAsync(ApiLoginRequestDto apiLoginRequest)
    {
        _logger.LogInformation("Attempting to find user with email: {Email}", apiLoginRequest.Email);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == apiLoginRequest.Email);
        return user;
    }

    /// <summary>
    /// checks if an email is already registered in the system
    /// </summary>
    /// <param name="email">Email to check availability in the database</param>
    /// <returns>True if email found already in the system, or false otherwise</returns>
    public async Task<bool> IsEmailAvailableAsync(string email)
    {
        _logger.LogInformation("Checking availability for email: {Email}", email);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        return user != null;
    }

    /// <summary>
    /// Add a new user to the system
    /// </summary>
    /// <param name="userCreateDto">A DTO representing user's data</param>
    /// <returns>The user newly added to the system</returns>
    public async Task<ApiLoginResultDto> AddUserAsync(UserCreateDto userCreateDto)
    {
        _logger.LogInformation("Adding new user with email: {Email}", userCreateDto.Email);

        // Check if the user already exists. Otherwise, create a new user record
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == userCreateDto.Email);

        // If user already exists, return no token
        if (existingUser != null)
            return new ApiLoginResultDto()
            {
                Success = false,
                Message = "Invalid Email or Password. Please try again."
            };

        // Or Create a new user object after hashing password
        var hashedPassword = _passwordHasher.HashPassword(null!, userCreateDto.Password);
        var newUser = new User
        {
            Email = userCreateDto.Email,
            Password = hashedPassword,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _context.Users.AddAsync(newUser);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User with email {Email} registered successfully.", userCreateDto.Email);

        // Create a JWT as the given user credentials are valid.
        var jwtToken = _jwtHandler.GenerateJwtToken(newUser);

        return new ApiLoginResultDto()
        {
            Success = true,
            Message = "Registration successful.",
            Token = jwtToken
        };
    }

    /// <summary>
    /// Identify user by email and password, 
    /// </summary>
    /// <param name="apiLoginRequest">A DTO representing user's credentials to identify</param>
    /// <returns>the user entity if found, or NULL otherwise.</returns>
    public async Task<ApiLoginResultDto> IdentifyUserAsync(ApiLoginRequestDto apiLoginRequest)
    {
        _logger.LogInformation("Identifying user with email: {Email}", apiLoginRequest.Email);

        // Check if the user exists
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == apiLoginRequest.Email);

        if (user == null)
            return new ApiLoginResultDto()
            {
                Success = false,
                Message = "Invalid Email or Password. Please try again."
            };

        // Verify the password matches
        var passwordVerificationResult = _passwordHasher
            .VerifyHashedPassword(user, user.Password, apiLoginRequest.Password);

        if (passwordVerificationResult == PasswordVerificationResult.Failed)
        {
            _logger.LogWarning("Invalid credentials for email: {Email}.", apiLoginRequest.Email);
            return new ApiLoginResultDto()
            {
                Success = false,
                Message = "Invalid Email or Password. Please try again."
            };
        }

        _logger.LogInformation("User with email {Email} logged in successfully.", apiLoginRequest.Email);

        // Create a JWT as the given user credentials are valid.
        var jwtToken = _jwtHandler.GenerateJwtToken(user);

        return new ApiLoginResultDto()
        {
            Success = true,
            Message = "Login successful.",
            Token = jwtToken
        };
    }

    /// <summary>
    /// Obtain the profile of a user from his email
    /// </summary>
    /// <param name="email">The unique email of a user</param>
    /// <returns>The specified user identified by the given email</returns>
    public async Task<User?> GetUserProfile(string email)
    {
        _logger.LogInformation("Retrieving the user from his email:{0}", email);

        var existingUer = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        return existingUer;
    }

    /// <summary>
    /// Updates the user data in the database
    /// </summary>
    /// <param name="id">The unique ID of a user in the database</param>
    /// <param name="userUpdateDto">A DTO containing modified user data</param>
    /// <returns>The newly modified data in the database</returns>
    public async Task<User?> UpdateUserProfile(int id, UserUpdateDto userUpdateDto)
    {
        _logger.LogInformation("Updating user profile with ID:{0} & Info:{1}", id, userUpdateDto);

        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.Email == userUpdateDto.email);

        if (existingUser != null)
        {
            _logger.LogInformation("User with ID:{0} exists in the database.", id);
            existingUser.FirstName = userUpdateDto.FirstName;
            existingUser.LastName = userUpdateDto.LastName;
            existingUser.UpdatedAt = DateTime.UtcNow;

            _context.Users.Update(existingUser);
            _context.Users.Entry(existingUser).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        return existingUser;
    }
}