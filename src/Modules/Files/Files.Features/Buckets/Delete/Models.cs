namespace Files.Features.Buckets.Delete;

public sealed record DeleteBucketRequest(Guid BucketId);

public sealed record DeleteBucketResponse(bool Success);
