namespace Files.Features.Services;

public interface IStorageService
{
    Task UploadAsync(Stream stream, string objectKey, string contentType, long sizeBytes, CancellationToken ct = default);
    Task DeleteAsync(string objectKey, CancellationToken ct = default);
    Task<string> GetPresignedUrlAsync(string objectKey, int expirySeconds = 3600, CancellationToken ct = default);
    Task CopyAsync(string sourceKey, string destKey, CancellationToken ct = default);
    Task EnsureBucketExistsAsync(CancellationToken ct = default);
}
