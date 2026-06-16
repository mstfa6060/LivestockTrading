using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Livestock.Persistence;

/// EF Core design-time factory used by `dotnet ef` so migrations can be added
/// without spinning up the full host (NATS, FusionCache, etc.).
public sealed class LivestockDbContextFactory : IDesignTimeDbContextFactory<LivestockDbContext>
{
    public LivestockDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("LIVESTOCK_DESIGN_CONNECTION")
            ?? "Host=localhost;Database=livestock_design;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<LivestockDbContext>()
            .UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsAssembly(typeof(LivestockDbContext).Assembly.FullName))
            .Options;

        return new LivestockDbContext(options);
    }
}
