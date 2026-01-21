// namespace LivestockTrading.Application.RequestHandlers.Students.Queries.Detail;

// public class DataAccess : IDataAccess
// {
//     private readonly LivestockTradingModuleDbContext _dbContext;

//     public DataAccess(ArfBlocksDependencyProvider dbContextProvider)
//     {
//         _dbContext = dbContextProvider.GetInstance<LivestockTradingModuleDbContext>();
//     }

//     public async Task<Student> GetById(Guid id, CancellationToken ct)
//     {
//         return await _dbContext.Students
//             .AsNoTracking()
//             .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted, ct);
//     }
// }
