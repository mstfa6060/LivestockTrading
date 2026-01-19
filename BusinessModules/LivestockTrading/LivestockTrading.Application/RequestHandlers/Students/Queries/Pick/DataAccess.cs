namespace LivestockTrading.Application.RequestHandlers.Students.Queries.Pick;

public class DataAccess : IDataAccess
{
    private readonly LivestockTradingModuleDbContext _dbContext;

    public DataAccess(ArfBlocksDependencyProvider dbContextProvider)
    {
        _dbContext = dbContextProvider.GetInstance<LivestockTradingModuleDbContext>();
    }

    public async Task<List<Student>> Pick(
        List<Guid> selectedIds,
        string keyword,
        int limit,
        CancellationToken ct)
    {
        var query = _dbContext.Students
            .AsNoTracking()
            .Where(s => !s.IsDeleted && s.IsActive);

        // Seçili ID'ler varsa öncelikli olarak getir
        if (selectedIds != null && selectedIds.Any())
        {
            return await query
                .Where(s => selectedIds.Contains(s.Id))
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync(ct);
        }

        // Keyword araması
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var lowerKeyword = keyword.ToLower();
            query = query.Where(s =>
                s.FirstName.ToLower().Contains(lowerKeyword) ||
                s.LastName.ToLower().Contains(lowerKeyword) ||
                s.StudentNumber.ToLower().Contains(lowerKeyword) ||
                s.Email.ToLower().Contains(lowerKeyword) ||
                s.Department.ToLower().Contains(lowerKeyword));
        }

        return await query
            .OrderByDescending(s => s.CreatedAt)
            .Take(limit > 0 ? limit : 10)
            .ToListAsync(ct);
    }
}
