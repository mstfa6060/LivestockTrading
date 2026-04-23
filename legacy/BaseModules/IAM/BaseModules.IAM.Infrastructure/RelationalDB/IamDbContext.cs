using Microsoft.EntityFrameworkCore;
using Common.Definitions.Domain.Entities;
using Common.Definitions.Infrastructure.RelationalDB;
using Common.Services.Environment;

namespace BaseModules.IAM.Infrastructure.RelationalDB;

public class IamDbContext : DefinitionDbContext, IIamDbContext
{
    public DbSet<AppRefreshToken> AppRefreshTokens { get; set; }
    public IamDbContext(IamDbContextOptions customDbContextOptions) : base(customDbContextOptions.DefinitionDbContextOptions)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}

public class IamDbContextOptions
{
    public readonly DbContextOptions<IamDbContext> DbContextOptions;
    public readonly DbContextOptions<DefinitionDbContext> DefinitionDbContextOptions;
    public IamDbContextOptions(RelationalDbConfiguration configuration, EnvironmentService environmentService, DefinitionDbContextOptions definitionDbContextOptions)
    {
        this.DefinitionDbContextOptions = definitionDbContextOptions.DbContextOptions;

        ///////////////////////////////////////////////////////////////////////////////////

        var dbContextOptionsBuilder = new DbContextOptionsBuilder<IamDbContext>();

        if (environmentService.Environment == CustomEnvironments.Test)
            dbContextOptionsBuilder.UseInMemoryDatabase("inmemory-db");
        else
            dbContextOptionsBuilder.UseSqlServer(configuration.ConnectionString);

        this.DbContextOptions = dbContextOptionsBuilder.Options;
    }
}