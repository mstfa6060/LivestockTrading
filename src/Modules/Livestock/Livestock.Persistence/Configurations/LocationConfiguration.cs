using Livestock.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Livestock.Persistence.Configurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.HasKey(x => x.Id);

        // PostGIS: geography(Point, 4326) so distance queries use ST_DWithin
        // on a GIST index instead of scanning Latitude/Longitude doubles.
        builder.Property(x => x.Geo).HasColumnType("geography(Point, 4326)");
        builder.HasIndex(x => x.Geo).HasMethod("GIST").HasDatabaseName("ix_locations_geo");
    }
}
