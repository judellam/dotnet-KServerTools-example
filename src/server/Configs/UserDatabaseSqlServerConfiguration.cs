using KServerTools.Common;

namespace server.Configs;

/// <summary>
/// The User Database Configuration. Implementing ISqlServerDatabaseConfiguration from KServerTools.Common.
/// </summary>
public class UserDatabaseSqlServerConfiguration : ISqlServerDatabaseConfiguration {
    public required string Server { get; set; }
    public required string Database { get; set; }
    public string[] Scopes { get; set; } = ["https://database.windows.net/.default"]; // unused, unless you're using an Azure service principal
    public string? ConnectionStringData { get; set; }
    public ISecretResolver? SecretResolver { get; set; }
    public string? ConnectionString { get; set; }

    public async Task<string?> GetConnectionString(CancellationToken cancellationToken) {
        if (!string.IsNullOrEmpty(this.ConnectionString)) {
            return this.ConnectionString;
        }
        
        if (string.IsNullOrEmpty(this.ConnectionStringData)) {
            return null;
        }

        if (this.SecretResolver is not null) {
            this.ConnectionString = await this.SecretResolver.Resolve(this.ConnectionStringData, cancellationToken)
                .ConfigureAwait(false);
        } else {
            this.ConnectionString = this.ConnectionStringData;
        }

        return this.ConnectionString;
    }
}