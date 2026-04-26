namespace Files.Features.Files.Delete;

public sealed record DeleteFileRequest(Guid BucketId, Guid FileId);

public sealed record DeleteFileResponse(bool Success);
