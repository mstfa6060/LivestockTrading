using Files.Domain.Enums;

namespace Files.Features.Buckets.Create;

public sealed record CreateBucketRequest(
    string Module,
    Guid? EntityId,
    BucketType BucketType
);

public sealed record CreateBucketResponse(
    Guid BucketId,
    string Module,
    Guid? EntityId,
    BucketType BucketType,
    DateTime CreatedAt
);
