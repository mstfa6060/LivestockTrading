using LivestockTrading.Catalog.Domain.Entities;
using LivestockTrading.Catalog.Infrastructure.Persistence.Conversions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LivestockTrading.Catalog.Infrastructure.Persistence.Configurations;

/// <summary>
/// CertificationType reference mapping. 05-catalog.md §4 + Adım 2 tasarım. PK int
/// identity (CertificationType.cs:64). Code unique kebab (doğal anahtar). NameTranslations
/// + DescriptionTranslations her ikisi non-null Translations → jsonb (CertificationType.cs:17-18
/// fresh read; Sapma 36 kod-içi belgeli — direct VO, JSON-string yok). CreatedAt/UpdatedAt
/// YOK (Revize 6).
/// </summary>
public sealed class CertificationTypeConfiguration : IEntityTypeConfiguration<CertificationType>
{
    public void Configure(EntityTypeBuilder<CertificationType> builder)
    {
        builder.ToTable("certification_types");
        builder.HasKey(ct => ct.Id);

        builder.Property(ct => ct.Code).IsRequired().HasMaxLength(50);
        builder.Property(ct => ct.IsActive).IsRequired();
        builder.Property(ct => ct.DisplayOrder).IsRequired();

        builder.Property(ct => ct.NameTranslations)
            .HasConversion(new TranslationsToJsonConverter(), TranslationsToJsonConverter.Comparer)
            .HasColumnType("jsonb")
            .IsRequired();
        builder.Property(ct => ct.DescriptionTranslations)
            .HasConversion(new TranslationsToJsonConverter(), TranslationsToJsonConverter.Comparer)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.HasIndex(ct => ct.Code).IsUnique();
    }
}
