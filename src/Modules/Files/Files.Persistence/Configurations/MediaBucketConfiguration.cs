using Files.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Files.Persistence.Configurations;

internal sealed class MediaBucketConfiguration : IEntityTypeConfiguration<MediaBucket>
{
    public void Configure(EntityTypeBuilder<MediaBucket> builder)
    {
        builder.ToTable("MediaBuckets");
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Module).IsRequired().HasMaxLength(100);
        builder.Property(b => b.BucketType).HasConversion<int>();

        builder.HasIndex(b => new { b.OwnerId, b.Module });
        builder.HasIndex(b => b.EntityId);

        builder.HasMany(b => b.Files)
            .WithOne(f => f.Bucket)
            .HasForeignKey(f => f.BucketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(b => !b.IsDeleted);
    }
}
