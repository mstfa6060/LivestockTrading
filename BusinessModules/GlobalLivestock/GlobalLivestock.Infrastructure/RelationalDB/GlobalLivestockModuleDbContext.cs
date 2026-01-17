using Microsoft.EntityFrameworkCore;
using Common.Definitions.Infrastructure.RelationalDB;
using Common.Services.Environment;

namespace GlobalLivestock.Infrastructure.RelationalDB;

public class GlobalLivestockModuleDbContext : DefinitionDbContext, IGlobalLivestockModuleDbContext
{
    public GlobalLivestockModuleDbContext(GlobalLivestockDbContextOptions customDbContextOptions)
        : base(customDbContextOptions.DefinitionDbContextOptions)
    { }

    // Entity Sets - Buraya entity'ler eklenecek
    // Ornek:
    // public DbSet<Animal> Animals { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        GlobalLivestockModelBuilder.Build(modelBuilder);

        // Prefix all GlobalLivestock tables to avoid collisions with other modules
        var prefix = "GlobalLivestock_";
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;
            if (clrType == null)
                continue;

            var ns = clrType.Namespace ?? string.Empty;
            if (!ns.StartsWith("GlobalLivestock.Domain"))
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

public class GlobalLivestockDbContextOptions
{
    public readonly DbContextOptions<GlobalLivestockModuleDbContext> DbContextOptions;
    public readonly DbContextOptions<DefinitionDbContext> DefinitionDbContextOptions;

    public GlobalLivestockDbContextOptions(
        RelationalDbConfiguration configuration,
        EnvironmentService environmentService,
        DefinitionDbContextOptions definitionDbContextOptions)
    {
        this.DefinitionDbContextOptions = definitionDbContextOptions.DbContextOptions;

        var dbContextOptionsBuilder = new DbContextOptionsBuilder<GlobalLivestockModuleDbContext>();

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
