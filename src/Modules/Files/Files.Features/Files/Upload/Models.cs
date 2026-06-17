using Files.Domain.Enums;

namespace Files.Features.Files.Upload;

// BucketId may be Guid.Empty on the very first upload; in that case the
// endpoint auto-creates a bucket using ModuleName + BucketType + EntityId
// before storing the file. Existing uploads pass the resolved BucketId
// back to reuse the bucket.
public sealed record UploadRequest(
    Guid BucketId,
    string? ModuleName,
    BucketType? BucketType,
    Guid? EntityId
);

public sealed record UploadResponse(
    Guid FileId,
    Guid BucketId,
    string ObjectKey,
    string? ThumbnailObjectKey,
    string OriginalName,
    string ContentType,
    string Extension,
    long SizeBytes,
    bool IsCover,
    int SortOrder,
    int? Width,
    int? Height,
    bool IsImage,
    DateTime CreatedAt
);
