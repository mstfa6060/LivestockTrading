namespace Files.Domain.Entities;

public class FileRecord : BaseEntity
{
    public Guid BucketId { get; set; }
    public string ObjectKey { get; set; } = default!;
    public string OriginalName { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public string Extension { get; set; } = default!;
    public long SizeBytes { get; set; }
    public bool IsCover { get; set; }
    public int SortOrder { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }

    public MediaBucket Bucket { get; set; } = default!;
}
