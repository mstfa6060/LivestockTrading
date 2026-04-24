using Livestock.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Livestock.Persistence.Configurations;

public class TransportRequestConfiguration : IEntityTypeConfiguration<TransportRequest>
{
    public void Configure(EntityTypeBuilder<TransportRequest> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PickupCountryCode).IsRequired().HasMaxLength(5);
        builder.Property(x => x.PickupCity).IsRequired().HasMaxLength(100);
        builder.Property(x => x.PickupAddress).HasMaxLength(500);
        builder.Property(x => x.DeliveryCountryCode).IsRequired().HasMaxLength(5);
        builder.Property(x => x.DeliveryCity).IsRequired().HasMaxLength(100);
        builder.Property(x => x.DeliveryAddress).HasMaxLength(500);
        builder.Property(x => x.CargoDescription).HasMaxLength(2000);
        builder.Property(x => x.SpecialRequirements).HasMaxLength(2000);
        builder.Property(x => x.Budget).HasPrecision(18, 4);
        builder.Property(x => x.CurrencyCode).HasMaxLength(10);
        builder.Property(x => x.EstimatedWeightKg).HasPrecision(10, 2);

        // PostGIS: pickup + delivery points for route/distance calculations.
        builder.Property(x => x.PickupGeo).HasColumnType("geography(Point, 4326)");
        builder.HasIndex(x => x.PickupGeo).HasMethod("GIST").HasDatabaseName("ix_transport_requests_pickup_geo");
        builder.Property(x => x.DeliveryGeo).HasColumnType("geography(Point, 4326)");
        builder.HasIndex(x => x.DeliveryGeo).HasMethod("GIST").HasDatabaseName("ix_transport_requests_delivery_geo");

        builder.HasOne(x => x.AssignedTransporter).WithMany(x => x.TransportRequests).HasForeignKey(x => x.AssignedTransporterId).OnDelete(DeleteBehavior.SetNull);
        builder.HasMany(x => x.Offers).WithOne(x => x.TransportRequest).HasForeignKey(x => x.TransportRequestId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.TrackingUpdates).WithOne(x => x.TransportRequest).HasForeignKey(x => x.TransportRequestId).OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
