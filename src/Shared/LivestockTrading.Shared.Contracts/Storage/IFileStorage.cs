namespace Shared.Contracts.Storage;

/// <summary>
/// Cross-module file storage port (plan-doc 02-modules-list §466 placement:
/// Shared/Contracts/Storage). Concrete implementation lives in Shared.Infrastructure
/// (Wave 4 W4.3) backed by MinIO; the same port carries the signed-download URL
/// surface used by GDPR data-export (plan-doc 05-identity §11). Identity is the
/// first consumer (avatar upload, W4.2.D); Listings, Marketplace and Accounts
/// will reuse it for listing images, dispute evidence and seller documents.
/// </summary>
public interface IFileStorage
{
    Task<string> UploadAsync(
        string bucket,
        string key,
        Stream content,
        string contentType,
        CancellationToken ct);

    Task DeleteAsync(string bucket, string key, CancellationToken ct);

    Task<string> GenerateSignedDownloadUrlAsync(
        string bucket,
        string key,
        TimeSpan ttl,
        CancellationToken ct);
}
