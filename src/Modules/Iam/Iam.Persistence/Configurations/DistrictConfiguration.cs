using Iam.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Iam.Persistence.Configurations;

internal sealed class DistrictConfiguration : IEntityTypeConfiguration<District>
{
    public void Configure(EntityTypeBuilder<District> builder)
    {
        builder.ToTable("Districts");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).ValueGeneratedNever();

        builder.Property(d => d.Name).IsRequired().HasMaxLength(200);
        builder.Property(d => d.NameTranslations).HasColumnType("text");
        builder.Property(d => d.Timezone).HasMaxLength(64);

        builder.HasIndex(d => d.ProvinceId);
    }
}
