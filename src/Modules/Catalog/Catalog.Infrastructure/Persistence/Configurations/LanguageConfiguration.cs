using LivestockTrading.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LivestockTrading.Catalog.Infrastructure.Persistence.Configurations;

/// <summary>
/// Language reference mapping. 05-catalog.md §4 + Adım 2 tasarım. PK int identity
/// (Language.cs:51). Code ISO 639-1 unique (doğal anahtar). CreatedAt/UpdatedAt YOK
/// (Language.cs fresh read, Revize 6 — map edilmez). VO/FK yok.
/// </summary>
public sealed class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.ToTable("languages");
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Code).IsRequired().HasMaxLength(2);
        builder.Property(l => l.NameEn).IsRequired().HasMaxLength(100);
        builder.Property(l => l.NativeName).IsRequired().HasMaxLength(100);
        builder.Property(l => l.IsRtl).IsRequired();
        builder.Property(l => l.IsActive).IsRequired();
        builder.Property(l => l.DisplayOrder).IsRequired();

        builder.HasIndex(l => l.Code).IsUnique();
    }
}
