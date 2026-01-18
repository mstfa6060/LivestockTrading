namespace LivestockTrading.Infrastructure.RelationalDB;

public interface ILivestockTradingModuleDbContext
{
    // Entity DbSet'leri buraya eklenecek
    // Ornek:
    // DbSet<Animal> Animals { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
