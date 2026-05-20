using LivestockTrading.Catalog.Domain.Entities;
using LivestockTrading.Catalog.Infrastructure.Persistence.Conversions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LivestockTrading.Catalog.Infrastructure.Persistence.Configurations;

/// <summary>
/// Location reference mapping (v2, 5-level). 05-catalog.md §4 + Adım 2 tasarım. PK int
/// identity (Location.cs:15 — Karar 3e istisnası "slug çakışma" → Slug UNIQUE DEĞİL).
/// ParentId self-ref navigationsız (Kural 1). Level enum → int. CountryCode non-null
/// converter (Revize 3). Centroid geometry(Point,4326) WGS84 (Açık Karar 4 + Location.cs:26
/// NTS Faz 1, DesignTimeFactory:32 UseNetTopologySuite). Name/NativeName → jsonb.
/// </summary>
public sealed class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("locations");
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Level).HasConversion<int>().IsRequired();
        builder.Property(l => l.Code).IsRequired().HasMaxLength(50);
        builder.Property(l => l.Slug).IsRequired().HasMaxLength(80);
        builder.Property(l => l.Path).IsRequired().HasMaxLength(500);
        builder.Property(l => l.PolygonGeoJsonUrl).HasMaxLength(500);
        builder.Property(l => l.IsActive).IsRequired();
        builder.Property(l => l.Population).IsRequired();
        builder.Property(l => l.DisplayOrder).IsRequired();
        builder.Property(l => l.CreatedAt).IsRequired();
        builder.Property(l => l.UpdatedAt).IsRequired();

        builder.Property(l => l.CountryCode)
            .HasConversion(CountryCodeConverter.NonNull)
            .HasMaxLength(2)
            .IsRequired();

        builder.Property(l => l.Centroid)
            .HasColumnType("geometry(Point,4326)");

        builder.Property(l => l.Name)
            .HasConversion(new TranslationsToJsonConverter(), TranslationsToJsonConverter.Comparer)
            .HasColumnType("jsonb")
            .IsRequired();
        builder.Property(l => l.NativeName)
            .HasConversion(TranslationsToJsonConverter.Nullable, TranslationsToJsonConverter.NullableComparer)
            .HasColumnType("jsonb");

        builder.HasOne<Location>()
            .WithMany()
            .HasForeignKey(l => l.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(l => l.ParentId);
        builder.HasIndex(l => l.CountryCode);
        builder.HasIndex(l => l.Level);
    }
}
