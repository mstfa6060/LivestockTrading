using LivestockTrading.Catalog.Domain.Aggregates;
using LivestockTrading.Catalog.Domain.Entities;
using LivestockTrading.Catalog.Infrastructure.Persistence.Conversions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LivestockTrading.Catalog.Infrastructure.Persistence.Configurations;

/// <summary>
/// Category AR mapping. Kural 5:362 thin per-AR + Adım 2 tasarım. PK int identity
/// (Category.cs:74 "Id 0 — EF persist sonrası set"). Code global unique (Açık Karar 5).
/// _attributes backing-field 1:N CategoryAttribute (Category.cs:30-31). ParentId self-ref
/// navigationsız (Kural 1, AggregateRoot.cs:6). Name/Description Translations → jsonb.
/// </summary>
public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Code).IsRequired().HasMaxLength(50);
        builder.Property(c => c.Level).IsRequired();
        builder.Property(c => c.DisplayOrder).IsRequired();
        builder.Property(c => c.IsActive).IsRequired();
        builder.Property(c => c.IconKey).HasMaxLength(100);
        builder.Property(c => c.CreatedAt).IsRequired();
        builder.Property(c => c.UpdatedAt).IsRequired();

        builder.Property(c => c.Name)
            .HasConversion(new TranslationsToJsonConverter(), TranslationsToJsonConverter.Comparer)
            .HasColumnType("jsonb")
            .IsRequired();
        builder.Property(c => c.Description)
            .HasConversion(TranslationsToJsonConverter.Nullable, TranslationsToJsonConverter.NullableComparer)
            .HasColumnType("jsonb");

        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(c => c.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Attributes)
            .WithOne()
            .HasForeignKey(a => a.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(c => c.Attributes)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(c => c.Code).IsUnique();
        builder.HasIndex(c => c.ParentId);
    }
}
