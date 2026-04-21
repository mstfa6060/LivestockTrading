// namespace LivestockTrading.Application.EventHandlers.Students.Commands.LogStudentActivity;

// public class DataAccess : IDataAccess
// {
//     private readonly LivestockTradingModuleDbContext _dbContext;

//     public DataAccess(ArfBlocksDependencyProvider dbContextProvider)
//     {
//         _dbContext = dbContextProvider.GetInstance<LivestockTradingModuleDbContext>();
//     }

//     /// <summary>
//     /// Son X gün içinde oluşturulan veya güncellenen öğrencileri getirir
//     /// </summary>
//     public async Task<List<Student>> GetRecentStudents(int daysToKeep, CancellationToken ct)
//     {
//         var cutoffDate = DateTime.UtcNow.AddDays(-daysToKeep);

//         return await _dbContext.Students
//             .AsNoTracking()
//             .Where(s => !s.IsDeleted &&
//                        (s.CreatedAt >= cutoffDate ||
//                         (s.UpdatedAt.HasValue && s.UpdatedAt >= cutoffDate)))
//             .OrderByDescending(s => s.UpdatedAt ?? s.CreatedAt)
//             .ToListAsync(ct);
//     }
// }
