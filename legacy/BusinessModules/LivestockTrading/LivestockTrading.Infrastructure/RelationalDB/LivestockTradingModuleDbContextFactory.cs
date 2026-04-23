using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Common.Definitions.Infrastructure.RelationalDB;

namespace LivestockTrading.Infrastructure.RelationalDB;

/// <summary>
/// Design-time factory for EF Core migrations
/// </summary>
public class LivestockTradingModuleDbContextFactory : IDesignTimeDbContextFactory<LivestockTradingModuleDbContext>
{
    public LivestockTradingModuleDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DefinitionDbContext>();


        return new LivestockTradingModuleDbContext(optionsBuilder.Options);
    }
}
