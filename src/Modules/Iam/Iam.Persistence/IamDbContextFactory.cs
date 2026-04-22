using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Iam.Persistence;

/// EF Core design-time factory used by `dotnet ef` so migrations can be added
/// without spinning up the full host (NATS, FusionCache, etc.).
public sealed class IamDbContextFactory : IDesignTimeDbContextFactory<IamDbContext>
{
    public IamDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("LIVESTOCK_DESIGN_CONNECTION")
            ?? "Host=localhost;Database=livestock_design;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<IamDbContext>()
            .UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsAssembly(typeof(IamDbContext).Assembly.FullName))
            .Options;

        return new IamDbContext(options);
    }
}
