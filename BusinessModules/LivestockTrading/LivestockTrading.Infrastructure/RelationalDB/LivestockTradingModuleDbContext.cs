using Microsoft.EntityFrameworkCore;
using Common.Definitions.Infrastructure.RelationalDB;
using Common.Services.Environment;

namespace LivestockTrading.Infrastructure.RelationalDB;

public class LivestockTradingModuleDbContext : DefinitionDbContext, ILivestockTradingModuleDbContext
{
    public LivestockTradingModuleDbContext(LivestockTradingDbContextOptions customDbContextOptions)
        : base(customDbContextOptions.DefinitionDbContextOptions)
    { }

    // Entity Sets - Buraya entity'ler eklenecek
    // Ornek:
    // public DbSet<Animal> Animals { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        LivestockTradingModelBuilder.Build(modelBuilder);

        // Prefix all LivestockTrading tables to avoid collisions with other modules
        var prefix = "LivestockTrading_";
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;
            if (clrType == null)
                continue;

            var ns = clrType.Namespace ?? string.Empty;
            if (!ns.StartsWith("LivestockTrading.Domain"))
                continue;

            var currentName = entityType.GetTableName();
            if (string.IsNullOrWhiteSpace(currentName))
                continue;

            if (!currentName.StartsWith(prefix, StringComparison.Ordinal))
            {
                entityType.SetTableName(prefix + currentName);
            }
        }
    }
}

public class LivestockTradingDbContextOptions
{
    public readonly DbContextOptions<LivestockTradingModuleDbContext> DbContextOptions;
    public readonly DbContextOptions<DefinitionDbContext> DefinitionDbContextOptions;

    public LivestockTradingDbContextOptions(
        RelationalDbConfiguration configuration,
        EnvironmentService environmentService,
        DefinitionDbContextOptions definitionDbContextOptions)
    {
        this.DefinitionDbContextOptions = definitionDbContextOptions.DbContextOptions;

        var dbContextOptionsBuilder = new DbContextOptionsBuilder<LivestockTradingModuleDbContext>();

        if (environmentService.Environment == CustomEnvironments.Test)
            dbContextOptionsBuilder.UseInMemoryDatabase("globallivestock-inmemory-db");
        else
            dbContextOptionsBuilder.UseSqlServer(configuration.ConnectionString, sql =>
            {
                sql.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(3),
                    errorNumbersToAdd: null
                );
            });

        this.DbContextOptions = dbContextOptionsBuilder.Options;
    }
}
