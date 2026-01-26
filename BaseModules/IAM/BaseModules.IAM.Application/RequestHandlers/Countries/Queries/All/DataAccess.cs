namespace BaseModules.IAM.Application.RequestHandlers.Countries.Queries.All;

public class DataAccess : IDataAccess
{
    private readonly IamDbContext _dbContext;

    public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
    {
        _dbContext = dependencyProvider.GetInstance<IamDbContext>();
    }

    public async Task<List<Country>> GetAll(string keyword, CancellationToken ct)
    {
        var query = _dbContext.Countries
            .AsNoTracking()
            .Where(c => c.IsActive);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var lowerKeyword = keyword.ToLower();
            query = query.Where(c =>
                c.Name.ToLower().Contains(lowerKeyword) ||
                c.NativeName.ToLower().Contains(lowerKeyword) ||
                c.Code.ToLower().Contains(lowerKeyword));
        }

        return await query
            .OrderBy(c => c.Name)
            .ToListAsync(ct);
    }
}
