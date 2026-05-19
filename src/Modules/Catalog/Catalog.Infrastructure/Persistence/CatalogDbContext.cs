using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Catalog.Infrastructure.Persistence;

/// <summary>
/// EF Core DbContext for the Catalog module.
/// W3.0: empty model — <c>HasDefaultSchema("catalog")</c> +
/// <c>ApplyConfigurationsFromAssembly</c> (IEntityTypeConfiguration impls land W3.1;
/// reflection scan is a no-op until then, compiles clean).
/// No DbSet (W3.1), no SaveChanges override (W3.4 domain-event interceptor).
/// Doc grounding: 04-migration.md §4b:112 (HasDefaultSchema + ApplyConfigurationsFromAssembly),
/// 03-domain-patterns.md Kural 5:362 (DbContext per modül).
/// </summary>
public sealed class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("catalog");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);
    }
}
