using LivestockTrading.Catalog.Domain.Aggregates;
using LivestockTrading.Catalog.Infrastructure.Persistence.Conversions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LivestockTrading.Catalog.Infrastructure.Persistence.Configurations;

/// <summary>
/// Breed AR mapping. Kural 5:362 + Adım 2 tasarım. PK int identity (Breed.cs:76).
/// CategoryId FK navigationsız (Kural 1). OriginCountryCode = plain string?, VO conversion
/// YOK — Breed.cs:20 doc-literal asimetri (W1-4 commit'li kod baskın, Revize 4).
/// (CategoryId, Code) composite unique (Açık Karar 5). Name/Description → jsonb.
/// </summary>
public sealed class BreedConfiguration : IEntityTypeConfiguration<Breed>
{
    public void Configure(EntityTypeBuilder<Breed> builder)
    {
        builder.ToTable("breeds");
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Code).IsRequired().HasMaxLength(50);
        builder.Property(b => b.CategoryId).IsRequired();
        builder.Property(b => b.OriginCountryCode).HasMaxLength(2);  // plain string, VO değil
        builder.Property(b => b.IsActive).IsRequired();
        builder.Property(b => b.DisplayOrder).IsRequired();
        builder.Property(b => b.CreatedAt).IsRequired();
        builder.Property(b => b.UpdatedAt).IsRequired();

        builder.Property(b => b.Name)
            .HasConversion(new TranslationsToJsonConverter(), TranslationsToJsonConverter.Comparer)
            .HasColumnType("jsonb")
            .IsRequired();
        builder.Property(b => b.Description)
            .HasConversion(TranslationsToJsonConverter.Nullable, TranslationsToJsonConverter.NullableComparer)
            .HasColumnType("jsonb");

        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(b => b.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(b => new { b.CategoryId, b.Code }).IsUnique();
    }
}
