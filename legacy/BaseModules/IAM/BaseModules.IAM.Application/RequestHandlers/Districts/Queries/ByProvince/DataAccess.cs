namespace BaseModules.IAM.Application.RequestHandlers.Districts.Queries.ByProvince;

public class DataAccess : IDataAccess
{
    private readonly IamDbContext _dbContext;

    public DataAccess(ArfBlocksDependencyProvider dependencyProvider)
    {
        _dbContext = dependencyProvider.GetInstance<IamDbContext>();
    }

    public async Task<List<District>> GetByProvince(int provinceId, string keyword, CancellationToken ct)
    {
        var query = _dbContext.Districts
            .AsNoTracking()
            .Where(d => d.ProvinceId == provinceId);

        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(d => d.Name.Contains(keyword));

        return await query
            .OrderBy(d => d.SortOrder)
            .ThenBy(d => d.Name)
            .ToListAsync(ct);
    }
}
