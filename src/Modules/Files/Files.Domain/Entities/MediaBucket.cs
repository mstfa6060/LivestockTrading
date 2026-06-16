using Files.Domain.Enums;

namespace Files.Domain.Entities;

public class MediaBucket : BaseEntity
{
    public Guid OwnerId { get; set; }
    public string Module { get; set; } = default!;
    public Guid? EntityId { get; set; }
    public BucketType BucketType { get; set; } = BucketType.Multiple;

    public ICollection<FileRecord> Files { get; set; } = [];
}
