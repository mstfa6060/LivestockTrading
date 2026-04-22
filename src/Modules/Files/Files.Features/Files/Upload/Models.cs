namespace Files.Features.Files.Upload;

public sealed record UploadRequest(Guid BucketId);

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
