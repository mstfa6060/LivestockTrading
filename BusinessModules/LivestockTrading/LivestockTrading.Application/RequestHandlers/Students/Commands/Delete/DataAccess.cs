// namespace LivestockTrading.Application.RequestHandlers.Students.Commands.Delete;

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
//             .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted, ct);
//     }

//     public async Task UpdateStudent(Student student, CancellationToken ct)
//     {
//         _dbContext.Students.Update(student);
//         await _dbContext.SaveChangesAsync(ct);
//     }
// }
