using Files.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Files.Persistence.Configurations;

internal sealed class FileRecordConfiguration : IEntityTypeConfiguration<FileRecord>
{
    public void Configure(EntityTypeBuilder<FileRecord> builder)
    {
        builder.ToTable("FileRecords");
        builder.HasKey(f => f.Id);

        builder.Property(f => f.ObjectKey).IsRequired().HasMaxLength(512);
        builder.Property(f => f.ThumbnailObjectKey).HasMaxLength(512);
        builder.Property(f => f.OriginalName).IsRequired().HasMaxLength(512);
        builder.Property(f => f.ContentType).IsRequired().HasMaxLength(128);
        builder.Property(f => f.Extension).IsRequired().HasMaxLength(20);

        builder.HasIndex(f => f.BucketId);
        builder.HasIndex(f => f.ObjectKey).IsUnique();

        builder.HasQueryFilter(f => !f.IsDeleted);
    }
}
