using LivestockTrading.Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Shared.Infrastructure;

namespace LivestockTrading.Catalog.Infrastructure.DesignTime;

/// <summary>
/// Design-time DbContext factory for EF migrations tooling (offline, no DI container).
/// Doc grounding: 04-migration.md §4d:643-666 (Identity-emsal adapted to catalog schema),
/// §4d:650 (LT__ env prefix), §4b:122 (UseNetTopologySuite — Location.Centroid NTS),
/// §4d:661 (UseSnakeCaseNamingConvention), §4d:658-659 (MigrationsHistoryTable + Assembly).
/// </summary>
public sealed class CatalogDbContextDesignTimeFactory : IDesignTimeDbContextFactory<CatalogDbContext>
{
    public CatalogDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables(prefix: "LT__")
            .Build();

        var connectionString = DatabaseConnectionStringBuilder.BuildMigrator(configuration, "catalog");

        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "catalog");
                npgsql.MigrationsAssembly(typeof(CatalogDbContext).Assembly.FullName);
                npgsql.UseNetTopologySuite();
            })
            .UseSnakeCaseNamingConvention()
            .Options;

        return new CatalogDbContext(options);
    }
}
