using Microsoft.EntityFrameworkCore;
using Common.Definitions.Infrastructure.RelationalDB;
using Common.Services.Environment;

namespace BaseModules.FileProvider.Infrastructure.RelationalDB;

public class FileProviderRelationalDbContext : DefinitionDbContext
{
    public static FileProviderRelationalDbContext CreateInstance(RelationalDbConfiguration configuration, EnvironmentService environmentService)
    {
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<DefinitionDbContext>();
        // if (environmentService.Environment == CustomEnvironments.Test)
        //     dbContextOptionsBuilder.UseInMemoryDatabase("example-task-db");
        // else
        dbContextOptionsBuilder.UseSqlServer(configuration.ConnectionString);
        return new FileProviderRelationalDbContext(dbContextOptionsBuilder.Options);
    }

    public FileProviderRelationalDbContext(DbContextOptions<DefinitionDbContext> options) : base(options)
    { }
}