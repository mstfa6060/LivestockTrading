using LivestockTrading.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LivestockTrading.Catalog.Infrastructure.Persistence.Configurations;

/// <summary>
/// Country reference mapping. 05-catalog.md §4 + Adım 2 tasarım. PK int identity
/// (Country.cs:86 Id==0 transient). Code ISO 3166-1 alpha-2 unique (doğal anahtar).
/// VO/FK yok (Country.cs:7 "cross-module event yaymaz", kod-bazlı denormalize).
/// CreatedAt/UpdatedAt mevcut (Country.cs:22-23).
/// </summary>
public sealed class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.ToTable("countries");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Code).IsRequired().HasMaxLength(2);
        builder.Property(c => c.NameEn).IsRequired().HasMaxLength(100);
        builder.Property(c => c.NativeName).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Region).IsRequired().HasMaxLength(50);
        builder.Property(c => c.DefaultCurrencyCode).IsRequired().HasMaxLength(3);
        builder.Property(c => c.DefaultLanguageCode).IsRequired().HasMaxLength(2);
        builder.Property(c => c.PhonePrefix).HasMaxLength(10);
        builder.Property(c => c.IsActive).IsRequired();
        builder.Property(c => c.DisplayOrder).IsRequired();
        builder.Property(c => c.CreatedAt).IsRequired();
        builder.Property(c => c.UpdatedAt).IsRequired();

        builder.HasIndex(c => c.Code).IsUnique();
    }
}
