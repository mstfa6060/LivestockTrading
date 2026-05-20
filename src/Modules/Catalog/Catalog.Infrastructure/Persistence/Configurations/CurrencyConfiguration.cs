using LivestockTrading.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LivestockTrading.Catalog.Infrastructure.Persistence.Configurations;

/// <summary>
/// Currency reference mapping. 05-catalog.md §4 + Adım 2 tasarım. PK int identity
/// (Currency.cs:66). Code ISO 4217 unique (doğal anahtar). RateToUsd numeric(18,6)
/// forex hassasiyeti (Açık Karar 3). CreatedAt/UpdatedAt YOK (Currency.cs fresh read,
/// Revize 6 — map edilmez). char alanlar Npgsql character(1).
/// </summary>
public sealed class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder.ToTable("currencies");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Code).IsRequired().HasMaxLength(3);
        builder.Property(c => c.NameEn).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Symbol).IsRequired().HasMaxLength(10);
        builder.Property(c => c.SymbolPosition).IsRequired();
        builder.Property(c => c.DecimalPlaces).IsRequired();
        builder.Property(c => c.ThousandSep).IsRequired();
        builder.Property(c => c.DecimalSep).IsRequired();
        builder.Property(c => c.IsActive).IsRequired();
        builder.Property(c => c.RateToUsd).HasPrecision(18, 6);
        builder.Property(c => c.RateUpdatedAt);

        builder.HasIndex(c => c.Code).IsUnique();
    }
}
