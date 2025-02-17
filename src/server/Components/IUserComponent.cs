using server.Models;

namespace server.Components;

/// <summary>
/// User component interface
/// </summary>
public interface IUserComponent {
    /// <summary>
    /// Logs the user on and returns a token to the caller if successful.
    /// </summary>
    public Task<UserLoginResponse> Login(User user, CancellationToken cancellationToken);

    /// <summary>
    /// Registers a new user.
    /// </summary>
    public Task<RegisterUser> Register(RegisterUser user, CancellationToken cancellationToken);

    /// <summary>
    /// Get all the users in the database.
    /// </summary>
    public Task<User[]> GetAllUsers(CancellationToken cancellationToken);
}