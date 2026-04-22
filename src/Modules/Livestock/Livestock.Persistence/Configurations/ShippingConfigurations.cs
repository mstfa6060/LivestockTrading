using Livestock.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Livestock.Persistence.Configurations;

internal sealed class ShippingCarrierConfiguration : IEntityTypeConfiguration<ShippingCarrier>
{
    public void Configure(EntityTypeBuilder<ShippingCarrier> builder)
    {
        builder.ToTable("ShippingCarriers");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Code).IsRequired().HasMaxLength(50);
        builder.Property(c => c.Website).HasMaxLength(500);
        builder.Property(c => c.TrackingUrlTemplate).HasMaxLength(1000);
        builder.Property(c => c.SupportedCountries).HasMaxLength(2000);

        builder.HasIndex(c => c.Code).IsUnique();
        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}

internal sealed class ShippingZoneConfiguration : IEntityTypeConfiguration<ShippingZone>
{
    public void Configure(EntityTypeBuilder<ShippingZone> builder)
    {
        builder.ToTable("ShippingZones");
        builder.HasKey(z => z.Id);

        builder.Property(z => z.Name).IsRequired().HasMaxLength(200);
        builder.Property(z => z.CountryCodes).IsRequired().HasMaxLength(2000);

        builder.HasIndex(z => z.SellerId);

        builder.HasOne(z => z.Seller)
            .WithMany()
            .HasForeignKey(z => z.SellerId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(z => !z.IsDeleted);
    }
}

internal sealed class ShippingRateConfiguration : IEntityTypeConfiguration<ShippingRate>
{
    public void Configure(EntityTypeBuilder<ShippingRate> builder)
    {
        builder.ToTable("ShippingRates");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.ShippingCost).HasColumnType("numeric(18,4)");
        builder.Property(r => r.MinOrderAmount).HasColumnType("numeric(18,4)");
        builder.Property(r => r.CurrencyCode).IsRequired().HasMaxLength(10);

        builder.HasIndex(r => r.ShippingZoneId);
        builder.HasIndex(r => r.ShippingCarrierId);

        builder.HasOne(r => r.Zone)
            .WithMany(z => z.Rates)
            .HasForeignKey(r => r.ShippingZoneId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Carrier)
            .WithMany(c => c.Rates)
            .HasForeignKey(r => r.ShippingCarrierId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(r => !r.IsDeleted);
    }
}
