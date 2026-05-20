using LivestockTrading.Catalog.Domain.Aggregates;
using LivestockTrading.Catalog.Infrastructure.Persistence.Conversions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LivestockTrading.Catalog.Infrastructure.Persistence.Configurations;

/// <summary>
/// Brand AR mapping. Kural 5:362 + Adım 2 tasarım. PK Guid v7 app-assigned
/// (Brand.cs:68 Guid.CreateVersion7 → ValueGeneratedNever). Slug unique kebab-case
/// maxLen 80 (Brand.cs:20 + SlugHelper.cs:9). Status enum → int (Revize 7).
/// OriginCountry CountryCode? nullable converter (Revize 3). _categories backing-field
/// 1:N BrandCategory (Brand.cs:39-40). Name/Description → jsonb.
/// </summary>
public sealed class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.ToTable("brands");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).ValueGeneratedNever();

        builder.Property(b => b.Slug).IsRequired().HasMaxLength(80);
        builder.Property(b => b.LogoUrl).HasMaxLength(500);
        builder.Property(b => b.Website).HasMaxLength(500);
        builder.Property(b => b.RejectionReason).HasMaxLength(1000);
        builder.Property(b => b.Status).HasConversion<int>().IsRequired();
        builder.Property(b => b.IsActive).IsRequired();
        builder.Property(b => b.DisplayOrder).IsRequired();
        builder.Property(b => b.CreatedAt).IsRequired();
        builder.Property(b => b.UpdatedAt).IsRequired();

        builder.Property(b => b.OriginCountry)
            .HasConversion(CountryCodeConverter.Nullable)
            .HasMaxLength(2);

        builder.Property(b => b.Name)
            .HasConversion(new TranslationsToJsonConverter(), TranslationsToJsonConverter.Comparer)
            .HasColumnType("jsonb")
            .IsRequired();
        builder.Property(b => b.Description)
            .HasConversion(TranslationsToJsonConverter.Nullable, TranslationsToJsonConverter.NullableComparer)
            .HasColumnType("jsonb");

        builder.HasMany(b => b.Categories)
            .WithOne()
            .HasForeignKey(bc => bc.BrandId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(b => b.Categories)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(b => b.Slug).IsUnique();
        builder.HasIndex(b => b.Status);
    }
}
