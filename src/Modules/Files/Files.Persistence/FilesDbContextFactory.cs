using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Files.Persistence;

/// EF Core design-time factory used by `dotnet ef` so migrations can be added
/// without spinning up the full host (NATS, FusionCache, etc.).
public sealed class FilesDbContextFactory : IDesignTimeDbContextFactory<FilesDbContext>
{
    public FilesDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("LIVESTOCK_DESIGN_CONNECTION")
            ?? "Host=localhost;Database=livestock_design;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<FilesDbContext>()
            .UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsAssembly(typeof(FilesDbContext).Assembly.FullName))
            .Options;

        return new FilesDbContext(options);
    }
}
