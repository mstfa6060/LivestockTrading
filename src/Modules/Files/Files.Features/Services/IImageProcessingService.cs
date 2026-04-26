namespace Files.Features.Services;

public interface IImageProcessingService
{
    bool IsImageContentType(string contentType);

    /// Encodes the source bytes to WebP at the configured quality.
    /// Returns the encoded bytes plus dimensions. Strips EXIF/metadata.
    Task<ProcessedImage> EncodeWebpAsync(byte[] source, CancellationToken ct = default);

    /// Produces a downscaled WebP thumbnail no larger than maxSize on the longer edge.
    Task<ProcessedImage> CreateThumbnailAsync(byte[] source, int maxSize = 300, CancellationToken ct = default);
}

public sealed record ProcessedImage(byte[] Bytes, int Width, int Height, string ContentType, string Extension);
