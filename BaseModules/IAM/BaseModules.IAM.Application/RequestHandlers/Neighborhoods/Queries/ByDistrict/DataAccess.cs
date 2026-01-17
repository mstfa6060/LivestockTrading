namespace BaseModules.IAM.Application.RequestHandlers.Neighborhoods.Queries.ByDistrict;

public class DataAccess : IDataAccess
{
    private readonly IamDbContext _dbContext;

    public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
    {
        _dbContext = dependencyProvider.GetInstance<IamDbContext>();
    }

    public async Task<List<Neighborhood>> GetByDistrict(int districtId, string keyword, CancellationToken ct)
    {
        var query = _dbContext.Neighborhoods
            .AsNoTracking()
            .Where(n => n.DistrictId == districtId);

        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(n => n.Name.Contains(keyword));

        return await query
            .OrderBy(n => n.SortOrder)
            .ThenBy(n => n.Name)
            .ToListAsync(ct);
    }
}
