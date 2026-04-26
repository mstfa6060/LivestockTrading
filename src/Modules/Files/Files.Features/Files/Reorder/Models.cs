namespace Files.Features.Files.Reorder;

public sealed record ReorderRequest(Guid BucketId, List<Guid> FileIds);

public sealed record ReorderResponse(bool Success);
