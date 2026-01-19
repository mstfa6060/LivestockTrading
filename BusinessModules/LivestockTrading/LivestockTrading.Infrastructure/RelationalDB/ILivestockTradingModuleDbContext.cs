using Microsoft.EntityFrameworkCore;
using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Infrastructure.RelationalDB;

public interface ILivestockTradingModuleDbContext
{
    DbSet<Student> Students { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
