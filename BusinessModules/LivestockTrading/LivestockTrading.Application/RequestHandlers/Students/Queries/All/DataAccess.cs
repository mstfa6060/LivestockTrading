namespace LivestockTrading.Application.RequestHandlers.Students.Queries.All;

public class DataAccess : IDataAccess
{
    private readonly LivestockTradingModuleDbContext _dbContext;

    public DataAccess(ArfBlocksDependencyProvider dbContextProvider)
    {
        _dbContext = dbContextProvider.GetInstance<LivestockTradingModuleDbContext>();
    }

    public async Task<(List<Student> Students, XPageResponse Page)> All(
        XSorting sorting,
        List<XFilterItem> filters,
        XPageRequest pageRequest,
        CancellationToken ct)
    {
        var query = _dbContext.Students
            .AsNoTracking()
            .Where(s => !s.IsDeleted)
            .Sort(sorting)
            .Filter(filters);

        // Default sorting
        if (sorting == null)
            query = query.OrderByDescending(s => s.CreatedAt);

        // Pagination
        var page = query.GetPage(pageRequest);
        var students = await query.Paginate(page).ToListAsync(ct);

        return (students, page);
    }
}
