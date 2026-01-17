namespace BaseModules.IAM.Application.RequestHandlers.Provinces.Queries.All;

public class DataAccess : IDataAccess
{
    private readonly IamDbContext _dbContext;

    public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
    {
        _dbContext = dependencyProvider.GetInstance<IamDbContext>();
    }

    public async Task<List<Province>> GetAll(string keyword, CancellationToken ct)
    {
        var query = _dbContext.Provinces.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(p => p.Name.Contains(keyword));

        return await query
            .OrderBy(p => p.SortOrder)
            .ThenBy(p => p.Name)
            .ToListAsync(ct);
    }
}
