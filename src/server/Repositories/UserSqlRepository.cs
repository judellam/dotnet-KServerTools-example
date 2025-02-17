using System.Diagnostics;
using KServerTools.Common;
using Microsoft.EntityFrameworkCore;
using server.Configs;

namespace server.Repositories;

internal class UserSqlRepository(ISqlServerService<UserDatabaseSqlServerConfiguration> userDatabaseService, IJsonLogger logger): IUserRepository {
    /// <summary>
    /// SQL Server connection to the User Database. Ensure setup instructions are followed for this to work.
    /// </summary>
    private readonly ISqlServerService<UserDatabaseSqlServerConfiguration> userDatabaseService = userDatabaseService;
    private readonly IJsonLogger logger = logger;

    /// <summary>
    /// Get the User.
    /// </summary>
    public async Task<UserEntity> GetAsync(UserEntityLookup lookup, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(lookup, nameof(lookup));
        Stopwatch stopwatch = Stopwatch.StartNew();
        try {
            using UserDbContext context = await this.GetUserDbContext(cancellationToken)
                .ConfigureAwait(false);

            UserEntity? user = await context.Users
                .Where(user => user.Username == lookup.Username)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (user is null || user.IsActive == false) {
                throw new KServerTools.Common.UnauthorizedAccessException("User-name or password is incorrect.");
            }

            return user;
        } finally {
            this.logger.Info("End login request", stopwatch.ElapsedMilliseconds);
        }
    }
    
    /// <summary>
    /// Create or update the User.
    /// </summary>
    public async Task<bool> CreateOrUpdateAsync(UserEntity model, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(model, nameof(model));
        Stopwatch stopwatch = Stopwatch.StartNew();
        try {
            using UserDbContext context = await this.GetUserDbContext(cancellationToken)
                .ConfigureAwait(false);

            UserEntity? user = await context.Users
                .FirstOrDefaultAsync(u => u.Username == model.Username, cancellationToken)
                .ConfigureAwait(false);

            if (user is null) {
                context.Users.Add(model);
            } else {
                context.Entry(user).CurrentValues.SetValues(model);
                context.Entry(user).Property(u => u.Id).IsModified = false; // Ensure Id is untouched
            }

            return await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false) == 1;
        } finally {
            this.logger.Info("End create our update login request", stopwatch.ElapsedMilliseconds);
        }
    }

    /// <summary>
    /// Delete the user.
    /// </summary>
    /// <remarks>
    /// This method sets the user to inactive.
    /// </remarks>
    public async Task<bool> DeleteAsync(UserEntityLookup lookup, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(lookup, nameof(lookup));
        Stopwatch stopwatch = Stopwatch.StartNew();
        try {
            using UserDbContext context = await this.GetUserDbContext(cancellationToken)
                .ConfigureAwait(false);

            UserEntity? user = await context.Users
                .Where(user => user.Username == lookup.Username)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (user is null || user.IsActive == false) {
                return false;
            }

            user.IsActive = false;
            context.Users.Update(user);
            int rows = await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return rows == 1;
        } finally {
            this.logger.Info("End delete login request", stopwatch.ElapsedMilliseconds);
        }
    }

    public async Task<IEnumerable<UserEntity>> GetMultipleAsync(AllUserLookup lookup, CancellationToken cancellationToken) {
        Stopwatch stopwatch = Stopwatch.StartNew();
        try {
            using UserDbContext context = await this.GetUserDbContext(cancellationToken)
                .ConfigureAwait(false);

            return await context.Users
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        } finally {
            this.logger.Info("End get multiple request", stopwatch.ElapsedMilliseconds);
        }
    }

    private async Task<UserDbContext> GetUserDbContext(CancellationToken cancellationToken) => 
        await UserDbContext.CreateDbContext(this.userDatabaseService, cancellationToken).ConfigureAwait(false);
}