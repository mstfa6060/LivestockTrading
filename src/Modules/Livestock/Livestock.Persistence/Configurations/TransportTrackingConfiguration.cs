using Livestock.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Livestock.Persistence.Configurations;

public class TransportTrackingConfiguration : IEntityTypeConfiguration<TransportTracking>
{
    public void Configure(EntityTypeBuilder<TransportTracking> builder)
    {
        builder.HasKey(x => x.Id);

        // PostGIS: GIST-indexed geography(Point, 4326) used for live tracking
        // proximity queries (ETA/route calculations).
        builder.Property(x => x.Geo).HasColumnType("geography(Point, 4326)");
        builder.HasIndex(x => x.Geo).HasMethod("GIST").HasDatabaseName("ix_transport_trackings_geo");
    }
}
