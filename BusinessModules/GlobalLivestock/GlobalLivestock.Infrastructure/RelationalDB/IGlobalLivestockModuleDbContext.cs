namespace GlobalLivestock.Infrastructure.RelationalDB;

public interface IGlobalLivestockModuleDbContext
{
    // Entity DbSet'leri buraya eklenecek
    // Ornek:
    // DbSet<Animal> Animals { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
