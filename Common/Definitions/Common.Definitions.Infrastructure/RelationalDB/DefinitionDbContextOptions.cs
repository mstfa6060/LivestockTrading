namespace Common.Definitions.Infrastructure.RelationalDB;
public class DefinitionDbContextOptions
{
    public readonly DbContextOptions<DefinitionDbContext> DbContextOptions;

    public DefinitionDbContextOptions(RelationalDbConfiguration configuration, EnvironmentService environmentService)
    {
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<DefinitionDbContext>();

        if (environmentService.Environment == CustomEnvironments.Test)
            dbContextOptionsBuilder.UseInMemoryDatabase("inmemory-db", InMemoryDb.InMemoryDatabaseRoot);
        else
            dbContextOptionsBuilder.UseSqlServer(configuration.ConnectionString);

        this.DbContextOptions = dbContextOptionsBuilder.Options;
    }
}