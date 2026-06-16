namespace Files.Features.Files.GetPresignedUrl;

public sealed record GetPresignedUrlRequest(Guid FileId, int ExpirySeconds = 3600);

public sealed record GetPresignedUrlResponse(Guid FileId, string Url, DateTime ExpiresAt);
