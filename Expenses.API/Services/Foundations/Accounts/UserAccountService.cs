using AutoInject.Attributes.ScopedAttributes;
using Expenses.API.Brokers.DateTimes;
using Expenses.API.Brokers.Loggings;
using Expenses.API.Brokers.Storages;
using Expenses.API.DTOs.Mapping;
using Expenses.API.DTOs.UserAccounts;
using Expenses.API.Models;
using Microsoft.AspNetCore.Identity;

namespace Expenses.API.Services.Foundations.Accounts;

/// <summary>
/// This service handles user account-related operations such as registration, login, and
/// email availability checks.
/// </summary>
[Scoped(typeof(IUserAccountService))]
public partial class UserAccountService : IUserAccountService
{
    #region Fields
    
    /// <summary>
    /// Broker interface for handling storage-specific operations related to accounts,
    /// transactions, and other entities within the Expenses API.
    /// </summary>
    private readonly IStorageBroker _storageBroker;

    /// <summary>
    /// Password hasher for securely hashing and verifying passwords.
    /// </summary>
    private readonly PasswordHasher<UserAccount> _passwordHasher;

    /// <summary>
    /// Broker responsible for logging operations such as recording informational, trace, debug, warning, error, and
    /// critical messages.
    /// </summary>
    private readonly ILoggingBroker _loggingBroker;

    /// <summary>
    /// Broker interface for handling date and time operations, such as retrieving
    /// the current date and time, used across various services within the application.
    /// </summary>
    private readonly IDateTimeBroker _dateTimeBroker;
    
    #endregion
    
    #region Constructors: Called when using new to instantiate a UserAccountService type

    /// <summary>
    /// Constructor to initialize the AccountService with necessary dependencies.
    /// </summary>
    /// <param name="storageBroker"></param>
    /// <param name="passwordHasher">Password hasher to initialize the service constructor</param>
    /// <param name="loggingBroker"></param>
    /// <param name="dateTimeBroker"></param>
    public UserAccountService(
        IStorageBroker storageBroker,
        PasswordHasher<UserAccount> passwordHasher,
        ILoggingBroker loggingBroker,
        IDateTimeBroker dateTimeBroker)
    {
        _storageBroker = storageBroker ?? throw new ArgumentNullException(nameof(storageBroker));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _loggingBroker = loggingBroker ?? throw new ArgumentNullException(nameof(loggingBroker));
        _dateTimeBroker = dateTimeBroker ?? throw new ArgumentNullException(nameof(dateTimeBroker));
    }
    
    #endregion
    
    //#region Helper Methods: These methods are used to encapsulate repetitive logic within the service methods.

    #region CRUD Operations: Called by the controller to perform user account operations

    /// <inheritdoc/>
    public async ValueTask<UserAccountResponse> IsEmailAvailableAsync(string email)
    {
        _loggingBroker.LogInformation($"Checking availability for email: {email}...");

        // Step 1. Validates the email format and checks if it's null or empty.
        // If the email is invalid, it throws an InvalidUserAccountException.  
        ValidateUserAccountIdOrEmail(email);

        // Step 2. Retrieves the user account associated with the provided email from the database.
        var maybeUserAccount = await _storageBroker.SelectUserAccountByEmailAsync(email);

        // Step 3. Validates the retrieved user account.
        // If the user account is null, it throws a NotFoundUserAccountException.
        // If the user account is found but has invalid data, it throws an InvalidUserAccountException.  
        ValidateStorageUserAccount(maybeUserAccount, email);

        // Return true if the email is already registered (user found), or false if the email is available (user not found)
        return maybeUserAccount.ToUserAccountDto();
    }

    /// <inheritdoc/>
    public ValueTask<UserAccountResponse> AddUserAccountAsync(UserRegisterRequest userRegisterRequest) =>
        TryCatch(async () =>
        {
            _loggingBroker.LogInformation($"Adding new user with email: {userRegisterRequest.Email}...");

            // Step 0. Checks if the email provided for registration is in a valid format and not null or empty.
            // If the email is invalid, it throws an InvalidUserAccountException.
            ValidateUserAccountIdOrEmail(userRegisterRequest.Email);

            // Step 1. Checks if a user account with the provided email already exists in the database.
            var existedUserAccount = await _storageBroker.SelectUserAccountByEmailAsync(userRegisterRequest.Email);

            // Checks the validation of existedUserAccount i.e., if the user account object returned in not null.
            // If the user account object is not null, it indicates that a user account with the same email already exists,
            // and an AlreadyExistsUserAccountException is thrown to signal this condition.    
            ValidateUserAccountNotAlreadyExists(existedUserAccount);

            // Step 2.2. Or Creates a new user account object after hashing password
            var hashedPassword = _passwordHasher.HashPassword(null!, userRegisterRequest.Password);
            var newUser = new UserAccount
            {
                Id = Guid.CreateVersion7(),
                Email = userRegisterRequest.Email,
                NormalizedEmail = userRegisterRequest.Email.ToUpper(),
                PasswordHash = hashedPassword, // How to validate a password hashed?
                IsAdmin = false,
                CreatedAt = _dateTimeBroker.GetCurrentDateTimeOffset(),
                UpdatedAt = _dateTimeBroker.GetCurrentDateTimeOffset()
            };

            // Step 3. Validates the new user account object to ensure it meets the required criteria for creation.
            // If the user account object is null, it throws a NullUserAccountException.
            // If the user account object has invalid data, it throws an InvalidUserAccountException.
            // If the user account object has an invalid ID, it throws an InvalidUserAccountException.
            // If the user account object has an invalid email, it throws an InvalidUserAccountException.
            // If the user account object has an invalid password hash, it throws an InvalidUserAccountException.
            // If the user account object has an invalid creation date, it throws an InvalidUserAccountException.
            // If the user account object has an invalid update date, it throws an InvalidUserAccountException.  
            ValidateUserAccountOnAdd(newUser);

            // Step 4. Inserts the new user account into the database.
            // If the insertion fails due to a database error, it throws a FailedUserAccountStorageException.
            // If the insertion fails because the user account already exists, it throws a NotFoundUserAccountException.
            // If the insertion fails due to a concurrency issue, it throws a ConcurrencyUserAccountException.
            // If the insertion fails due to a general error, it throws a UserAccountServiceException.
            // If the insertion is successful, it logs the successful registration of the user account.  
            var maybeUserAccount = await _storageBroker.InsertUserAccountAsync(newUser);

            // Step 5. Validates the inserted user account to ensure it was created successfully and matches the provided email.
            // If the inserted user account is null, it throws a NotFoundUserAccountException.
            // If the inserted user account has invalid data, it throws an InvalidUserAccountException.
            // If the inserted user account's email does not match the provided email, it throws a NotFoundUserAccountException.   
            ValidateStorageUserAccount(maybeUserAccount, userRegisterRequest.Email);

            _loggingBroker.LogInformation($"User with email:{maybeUserAccount.Email} registered successfully.");

            // Step 6. Creates a JWT as the given user credentials are valid and returns it to the client for
            // authentication in later requests.
            //var jwtToken = _jwtHandler.GenerateJwtToken(newUser); // send it to the controller layer!!!

            // Finally, returns the user account details as a UserAccountDto if the creation and validation are successful.
            return maybeUserAccount.ToUserAccountDto();
        }); //2


    /// <inheritdoc/>
    public ValueTask<UserAccountResponse> IdentifyUserAccountAsync(UserLoginRequest userLogin) =>
        TryCatch(async () =>
            {
                _loggingBroker.LogInformation($"Identifying user with email: {userLogin.Email}...");

                // Step 0. Validates the email provided for login is in a valid format and not null or empty.
                // If the email is invalid, it throws an InvalidUserAccountException.   
                ValidateUserAccountIdOrEmail(userLogin.Email);

                // Step 1. Checks if a user account with the provided email exists in the database.
                var maybeUserAccount = await _storageBroker.SelectUserAccountByEmailAsync(userLogin.Email);

                // Step 2. Validates the user account found in the database.
                // If the user account is null, it throws a NotFoundUserAccountException.
                // If the user account is found but has invalid data, it throws an InvalidUserAccountException.
                // If the user account's email does not match the provided email, it throws a NotFoundUserAccountException.   
                ValidateStorageUserAccount(maybeUserAccount, userLogin.Email);
                
                // Step 2.2. Checks if the password matches the stored password hash for the retrieved user account.
                // If the password does not match, it throws a NotFoundUserAccountException.
                // If the password is valid, it proceeds to the next step.
                ValidateStorageUserAccountPassword(maybeUserAccount, maybeUserAccount.PasswordHash,
                    userLogin.Password);                

                // The user account is successfully identified, log the successful login attempt
                _loggingBroker.LogInformation($"User with email:{maybeUserAccount.Email} logged in successfully.");

                // Return the user account details as a UserAccountDto if the identification and validation are successful.
                return maybeUserAccount.ToUserAccountDto();
            }
        );

    /// <inheritdoc/>
    public ValueTask<UserAccountResponse> RetrieveUserProfileAsync(string email) => //5
        TryCatch(async () =>
        {
            _loggingBroker.LogInformation($"Retrieving the user from his email:{email}");

            // Step 0. Validates the email provided for retrieving the user profile is in a valid format and not null or empty.
            // If the email is invalid, it throws an InvalidUserAccountException.   
            ValidateUserAccountIdOrEmail(email);

            // Step 1. Get the user profile from the database using the provided email.
            // If no user is found, it throws a NotFoundUserAccountException.
            // If a user is found, it returns the user account details as a UserAccountDto. 
            var existedUserAccount = await _storageBroker.SelectUserAccountByEmailAsync(email);

            // Step 2. Validate the retrieved user account.
            // If the user account is null, it throws a NotFoundUserAccountException.
            // If the user account is found but has invalid data, it throws an InvalidUserAccountException.
            // If the user account's email does not match the provided email, it throws a NotFoundUserAccountException.  
            ValidateStorageUserAccount(existedUserAccount, email);

            // Return the user account details as a UserAccountDto if the retrieval and validation are successful.
            return existedUserAccount.ToUserAccountDto();
        });

    /// <inheritdoc/>
    public ValueTask<UserAccountResponse> ModifyUserProfileAsync(Guid userId, UserUpdateRequest userUpdateRequest) => 
        TryCatch(async () =>
        {
            _loggingBroker.LogInformation($"Updating user profile with ID:{userId} & Info:{userUpdateRequest}");

            // step 1. Validates the user ID provided for updating the user profile is in a valid format and not null or empty.
            // If the user ID is invalid, it throws an InvalidUserAccountException. 
            ValidateUserAccountIdOrEmail(userId);

            // Step 2. Validates the user data provided for updating the user profile to ensure it meets the required
            // criteria for modification.
            // If the user data is null, it throws a NullUserAccountException.
            // If the user data has invalid data, it throws an InvalidUserAccountException. 
            ValidateUserAccountOnModifyProfile(userUpdateRequest);

            // Step 3. Checks if a user account with the provided user ID exists in the database.
            var fetchedUserAccount = await _storageBroker.SelectUserAccountByIdAsync(userId);

            // Step 4. Validates the user account found in the database.
            // if the user account is null, it throws a NotFoundUserAccountException.
            // If the user account is found but has invalid data, it throws an InvalidUserAccountException. 
            ValidateStorageUserAccount(fetchedUserAccount, userId); //ici

            _loggingBroker.LogInformation($"User with ID:{userId} exists in the database.");
            fetchedUserAccount.FirstName = userUpdateRequest.FirstName;
            fetchedUserAccount.LastName = userUpdateRequest.LastName;
            fetchedUserAccount.NormalizedFullName = $"{userUpdateRequest.FirstName.ToUpper()} {userUpdateRequest.LastName.ToUpper()}";
            fetchedUserAccount.DateOfBirth = userUpdateRequest.DateOfBirth;
            fetchedUserAccount.UpdatedAt = _dateTimeBroker.GetCurrentDateTimeOffset();

            // Step 4. updates the user account in the database with the modified data.
            // If the update fails due to a database error, it throws a FailedUserAccountStorageException.
            // If the update fails because the user account does not exist, it throws a NotFoundUserAccountException.
            // If the update fails due to a concurrency issue, it throws a ConcurrencyUserAccountException.
            // If the update fails due to a general error, it throws a UserAccountServiceException. 
            var modifiedUserAccount = await _storageBroker.UpdateUserAccountAsync(fetchedUserAccount);

            // Finally, returns the updated user account details as a UserAccountDto if the modification and update are
            // successful.
            return modifiedUserAccount.ToUserAccountDto();
        });
    
    #endregion
}