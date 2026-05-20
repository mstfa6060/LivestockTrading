using LivestockTrading.Catalog.Domain.Aggregates;
using LivestockTrading.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LivestockTrading.Catalog.Infrastructure.Persistence.Configurations;

/// <summary>
/// BrandCategory M2M junction mapping (Brand AR child). Kural 5 + Adım 2 tasarım.
/// Composite PK (BrandId, CategoryId) — Id YOK (BrandCategory.cs:31, Revize 5).
/// FK BrandId → Brand: ilişki BrandConfiguration principal tarafında tanımlı (Cascade);
/// burada yeniden tanımlanmaz. FK CategoryId → Category navigationsız Restrict (Kural 1).
/// CategoryId ayrı index (composite PK BrandId-leading → ters lookup).
/// </summary>
public sealed class BrandCategoryConfiguration : IEntityTypeConfiguration<BrandCategory>
{
    public void Configure(EntityTypeBuilder<BrandCategory> builder)
    {
        builder.ToTable("brand_categories");
        builder.HasKey(bc => new { bc.BrandId, bc.CategoryId });

        builder.Property(bc => bc.BrandId).IsRequired();
        builder.Property(bc => bc.CategoryId).IsRequired();

        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(bc => bc.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(bc => bc.CategoryId);
    }
}
