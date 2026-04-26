namespace Files.Features.Files.SetCover;

public sealed record SetCoverRequest(Guid BucketId, Guid FileId);

public sealed record SetCoverResponse(bool Success);
