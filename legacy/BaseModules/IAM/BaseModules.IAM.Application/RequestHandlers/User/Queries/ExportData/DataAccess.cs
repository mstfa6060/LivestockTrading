namespace BaseModules.IAM.Application.RequestHandlers.Users.Queries.ExportData;

public class DataAccess : IDataAccess
{
    private readonly IamDbContext _dbContext;

    public DataAccess(ArfBlocksDependencyProvider provider)
    {
        _dbContext = provider.GetInstance<IamDbContext>();
    }

    public async Task<Common.Definitions.Domain.Entities.User> GetUserById(Guid userId)
    {
        return await _dbContext.AppUsers.FirstOrDefaultAsync(x => x.Id == userId && !x.IsDeleted);
    }
}
