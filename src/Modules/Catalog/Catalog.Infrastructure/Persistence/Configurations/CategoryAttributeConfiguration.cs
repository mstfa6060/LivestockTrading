using LivestockTrading.Catalog.Domain.Entities;
using LivestockTrading.Catalog.Infrastructure.Persistence.Conversions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LivestockTrading.Catalog.Infrastructure.Persistence.Configurations;

/// <summary>
/// CategoryAttribute child entity mapping (Category AR child). Kural 5 + Adım 2 tasarım.
/// PK Guid app-assigned (CategoryAttribute.cs:47 Guid.NewGuid → ValueGeneratedNever).
/// (CategoryId, Key) unique — Domain invariant grounded (Category.cs:153 aynı kategoride
/// Key tekil). FK CategoryId → Category, ilişki CategoryConfiguration principal tarafında
/// tanımlı (Cascade); burada yeniden tanımlanmaz (çift-config çakışma önleme). ValueType
/// enum → int. Label/HelpText → jsonb. OptionsJson jsonb (Açık Karar 2).
/// </summary>
public sealed class CategoryAttributeConfiguration : IEntityTypeConfiguration<CategoryAttribute>
{
    public void Configure(EntityTypeBuilder<CategoryAttribute> builder)
    {
        builder.ToTable("category_attributes");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedNever();

        builder.Property(a => a.CategoryId).IsRequired();
        builder.Property(a => a.Key).IsRequired().HasMaxLength(50);
        builder.Property(a => a.ValueType).HasConversion<int>().IsRequired();
        builder.Property(a => a.Required).IsRequired();
        builder.Property(a => a.Filterable).IsRequired();
        builder.Property(a => a.Unit).HasMaxLength(20);
        builder.Property(a => a.OptionsJson).HasColumnType("jsonb");
        builder.Property(a => a.DisplayOrder).IsRequired();
        builder.Property(a => a.CreatedAt).IsRequired();
        builder.Property(a => a.UpdatedAt).IsRequired();

        builder.Property(a => a.Label)
            .HasConversion(new TranslationsToJsonConverter(), TranslationsToJsonConverter.Comparer)
            .HasColumnType("jsonb")
            .IsRequired();
        builder.Property(a => a.HelpText)
            .HasConversion(TranslationsToJsonConverter.Nullable, TranslationsToJsonConverter.NullableComparer)
            .HasColumnType("jsonb");

        builder.HasIndex(a => new { a.CategoryId, a.Key }).IsUnique();
    }
}
