using Microsoft.EntityFrameworkCore;
using Common.Definitions.Domain.Entities;
using Common.Definitions.Infrastructure.RelationalDB;

namespace BaseModules.Notification.Application.RequestHandlers.MobilApplicationVersiyon.Queries.GetVersion;

public class DataAccess : IDataAccess
{
    private readonly DefinitionDbContext _dbContext;

    public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
    {
        _dbContext = dependencyProvider.GetInstance<DefinitionDbContext>();
    }

    public async Task<Common.Definitions.Domain.Entities.MobilApplicationVersiyon> GetByPlatform(string platform)
    {
        return await _dbContext.MobilApplicationVersiyons
            .Where(x => x.Platform.ToLower() == platform.ToLower()
                     && x.IsActive
                     && !x.IsDeleted)
            .FirstOrDefaultAsync();
    }
}
