using System.Security.Cryptography;
using System.Text;
using KServerTools.Common;
using Microsoft.Data.SqlClient;
using server.Models;
using server.Repositories;

namespace server.Components;

public class UserComponent(IUserRepository userRepository, IJsonLogger logger) : IUserComponent {
    private readonly IUserRepository userRepository = userRepository;
    private readonly IJsonLogger logger = logger;

    public async Task<UserLoginResponse> Login(User user, CancellationToken cancellationToken) {
        UserEntity? userEntity = await userRepository
            .GetAsync(new UserEntityLookup(user.Username), cancellationToken)
            .ConfigureAwait(false);

        // Verif the user exists and is active.
        if (userEntity is null || !userEntity.IsActive || !VerifyPasswordHash(user.Password, userEntity.Hash, userEntity.Salt)) { 
            throw new KServerTools.Common.UnauthorizedAccessException("User-name or password is incorrect.");
        }

        // Generate a JWT token for the user.
        string token = JwtGenerator.Generate(
            userEntity.Id,
            userEntity.Username,
            userEntity.Email);

        return new UserLoginResponse(token);
    }

    public async Task<RegisterUser> Register(RegisterUser user, CancellationToken cancellationToken) {
        (byte[] hash, byte[] salt) = GetPasswordHash(user.Password);
        UserEntity userEntity = new(user.Username, user.Email, hash, salt);

        try {
            bool userAdded = await userRepository.CreateOrUpdateAsync(userEntity, cancellationToken)
                .ConfigureAwait(false);

            // Verify the user has been commited to the database.
            userEntity = await userRepository
                .GetAsync(new UserEntityLookup(user.Username), cancellationToken)
                .ConfigureAwait(false);
            
            // Create a new model to return to the caller.
            // Normally should be done in a helper.
            user = new RegisterUser(userEntity.Username, string.Empty, userEntity.Email);
        } catch (SqlException sqlEx) {
            if (sqlEx.Number == 2627) {
                throw new BadRequestException("User already exists.");
            }
            throw new InternalServerErrorException("Failed to create user.", sqlEx);
        }

        return user;
    }

    public async Task<User[]> GetAllUsers(CancellationToken cancellationToken) {
        try {
            IEnumerable<UserEntity> users = await userRepository.GetMultipleAsync(new AllUserLookup(), cancellationToken)
                .ConfigureAwait(false);

            return [..users.Select(user => new User(user.Username, string.Empty) {
                Email = user.Email
            })];
        } catch (Exception ex){
            throw new InternalServerErrorException("Unable to query for all users.", ex);
        }
    }

    private static (byte[] hash, byte[] salt) GetPasswordHash(string password) {
        int saltSize = 256;
        byte[] initialSalt = new byte[saltSize];
        for (int i = 0; i < saltSize; i++) {
            initialSalt[i] = (byte)new Random().Next(0, 256);
        }
        using var hmac = new HMACSHA512(initialSalt);
        return (hmac.ComputeHash(Encoding.UTF8.GetBytes(password)), hmac.Key);
    }

    private static bool VerifyPasswordHash(string password, byte[] hash, byte[] salt) {
        using var hmac = new HMACSHA512(salt);
        byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        for (int i = 0; i < computedHash.Length; i++) {
            if (computedHash[i] != hash[i]) {
                return false;
            }
        }
        return true;
    }
}