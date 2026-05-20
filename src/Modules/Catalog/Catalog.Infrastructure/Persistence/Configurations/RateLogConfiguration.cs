using LivestockTrading.Catalog.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LivestockTrading.Catalog.Infrastructure.Persistence.Configurations;

/// <summary>
/// RateLog Infra-local entity mapping. 05-catalog.md §6:702-713 grounded + Adım 2 tasarım.
/// Tablo catalog.rate_logs. PK Guid v7 app-assigned (RateLog.cs ctor → ValueGeneratedNever).
/// Source RateProvider enum → int (Revize 7). RatesJson jsonb (Açık Karar 2). FetchedAt
/// timestamptz. (RateDate, Source) NON-unique index — append-only (günde çok fetch denemesi
/// loglanır), uniqueness OLMAZ.
/// </summary>
public sealed class RateLogConfiguration : IEntityTypeConfiguration<RateLog>
{
    public void Configure(EntityTypeBuilder<RateLog> builder)
    {
        builder.ToTable("rate_logs");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever();

        builder.Property(r => r.RateDate).IsRequired();
        builder.Property(r => r.Source).HasConversion<int>().IsRequired();
        builder.Property(r => r.RatesJson).IsRequired().HasColumnType("jsonb");
        builder.Property(r => r.Success).IsRequired();
        builder.Property(r => r.Error).HasMaxLength(2000);
        builder.Property(r => r.FetchedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.HasIndex(r => new { r.RateDate, r.Source });
    }
}
