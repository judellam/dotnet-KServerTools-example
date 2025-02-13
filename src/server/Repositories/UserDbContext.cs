using KServerTools.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using server.Configs;

namespace server.Repositories;

internal class UserDbContext(DbContextOptions<UserDbContext> options, SqlConnection connection) : DbContext(options) {
    private readonly SqlConnection connection = connection;
    public DbSet<UserEntity> Users { get; set; }
    public static async Task<UserDbContext> CreateDbContext(ISqlServerService<UserDatabaseSqlServerConfiguration> userDatabase, CancellationToken cancellationToken) {
        SqlConnection connection = await userDatabase.GetOrCreateConnection(cancellationToken)
            .ConfigureAwait(false);

        var options = new DbContextOptionsBuilder<UserDbContext>()
            .UseSqlServer(connection) // Configure EF to use the existing connection
            .Options;

        return new UserDbContext(options, connection);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<UserEntity>(entity => {
            entity.ToTable("Users");
            entity.Property(e => e.Username).IsRequired().HasMaxLength(255);
            entity.HasKey(e => e.Email);
            entity.HasIndex(e => e.Username);
        });
    }

    public override void Dispose() {
        base.Dispose();
        try{
            this.connection?.Dispose();
        } catch {
        }
    }
}