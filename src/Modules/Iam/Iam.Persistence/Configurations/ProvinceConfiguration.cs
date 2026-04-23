using Iam.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Iam.Persistence.Configurations;

internal sealed class ProvinceConfiguration : IEntityTypeConfiguration<Province>
{
    public void Configure(EntityTypeBuilder<Province> builder)
    {
        builder.ToTable("Provinces");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();

        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Code).HasMaxLength(10);
        builder.Property(p => p.NameTranslations).HasColumnType("text");
        builder.Property(p => p.Timezone).HasMaxLength(64);

        builder.HasIndex(p => p.CountryId);
        builder.HasIndex(p => new { p.CountryId, p.Code });
    }
}
