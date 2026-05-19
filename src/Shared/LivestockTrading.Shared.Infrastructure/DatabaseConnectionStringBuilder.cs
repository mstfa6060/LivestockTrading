using Microsoft.Extensions.Configuration;

namespace Shared.Infrastructure;

/// <summary>
/// Builds PostgreSQL connection strings per role from configuration.
/// W3.0: <see cref="BuildMigrator"/> only (DesignTimeFactory / EF migrations,
/// <c>livestock_migrator</c> DDL role). BuildApp (<c>livestock_app</c> DML, runtime)
/// deferred to W3.7 host-wiring — use-case driven (KAYDET-17 emsali, no premature surface).
/// Doc grounding: 04-migration.md §4d:653 (BuildMigrator signature),
/// §4b:137 (Search Path hibrit), §4b:148 (livestock_migrator DDL role),
/// §4d:709 (livestock_trading db), §4d:650 (LT__ env prefix → Database:* nested).
/// Host/Port/Name optional with localhost dev defaults: W3.0 is offline
/// (migrations add never connects, C.5#4); full config is a W3.7 host-wiring concern.
/// </summary>
public static class DatabaseConnectionStringBuilder
{
    public static string BuildMigrator(IConfiguration configuration, string schemaName)
    {
        var host = configuration["Database:Host"] ?? "localhost";
        var port = configuration["Database:Port"] ?? "5432";
        var database = configuration["Database:Name"] ?? "livestock_trading";
        var password = configuration["Database:MigratorPassword"] ?? string.Empty;

        return $"Host={host};Port={port};Database={database};" +
               $"Username=livestock_migrator;Password={password};" +
               $"Search Path={schemaName},public";
    }
}
