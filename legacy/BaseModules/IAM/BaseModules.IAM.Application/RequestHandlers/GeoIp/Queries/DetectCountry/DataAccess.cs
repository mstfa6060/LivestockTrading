namespace BaseModules.IAM.Application.RequestHandlers.GeoIp.Queries.DetectCountry;

public class DataAccess : IDataAccess
{
    private readonly IamDbContext _dbContext;

    public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
    {
        _dbContext = dependencyProvider.GetInstance<IamDbContext>();
    }

    public async Task<Country> GetCountryByCode(string code, CancellationToken ct)
    {
        return await _dbContext.Countries
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Code == code && c.IsActive, ct);
    }
}
