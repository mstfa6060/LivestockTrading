namespace LivestockTrading.Application.RequestHandlers.Students.Commands.Create;

public class DataAccess : IDataAccess
{
    private readonly LivestockTradingModuleDbContext _dbContext;

    public DataAccess(ArfBlocksDependencyProvider dbContextProvider)
    {
        _dbContext = dbContextProvider.GetInstance<LivestockTradingModuleDbContext>();
    }

    public async Task AddStudent(Student student, CancellationToken ct)
    {
        await _dbContext.Students.AddAsync(student, ct);
        await _dbContext.SaveChangesAsync(ct);
    }
}
