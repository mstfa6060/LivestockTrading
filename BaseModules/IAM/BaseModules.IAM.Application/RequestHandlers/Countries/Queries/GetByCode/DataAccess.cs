namespace BaseModules.IAM.Application.RequestHandlers.Countries.Queries.GetByCode;

public class DataAccess : IDataAccess
{
    private readonly IamDbContext _dbContext;

    public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
    {
        _dbContext = dependencyProvider.GetInstance<IamDbContext>();
    }

    public async Task<Country> GetByCode(string code, CancellationToken ct)
    {
        return await _dbContext.Countries
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Code == code && c.IsActive, ct);
    }
}
