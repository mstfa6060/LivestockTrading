using SkiaSharp;

namespace Files.Features.Services;

public sealed class SkiaImageProcessingService : IImageProcessingService
{
    private const int WebpQuality = 85;
    private const string WebpContentType = "image/webp";
    private const string WebpExtension = "webp";

    private static readonly HashSet<string> ImageContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/jpg", "image/png", "image/webp", "image/gif", "image/bmp", "image/tiff",
    };

    public bool IsImageContentType(string contentType)
        => !string.IsNullOrWhiteSpace(contentType) && ImageContentTypes.Contains(contentType);

    public Task<ProcessedImage> EncodeWebpAsync(byte[] source, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        using var bitmap = DecodeOrThrow(source);
        return Task.FromResult(EncodeWebp(bitmap));
    }

    public Task<ProcessedImage> CreateThumbnailAsync(byte[] source, int maxSize = 300, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        using var bitmap = DecodeOrThrow(source);

        var (targetWidth, targetHeight) = ScaleToFit(bitmap.Width, bitmap.Height, maxSize);
        if (targetWidth == bitmap.Width && targetHeight == bitmap.Height)
        {
            return Task.FromResult(EncodeWebp(bitmap));
        }

        using var resized = bitmap.Resize(new SKImageInfo(targetWidth, targetHeight), SKSamplingOptions.Default);
        return Task.FromResult(EncodeWebp(resized));
    }

    private static SKBitmap DecodeOrThrow(byte[] source)
    {
        var bitmap = SKBitmap.Decode(source)
            ?? throw new InvalidOperationException("Image could not be decoded — source bytes are not a recognised raster format.");
        return bitmap;
    }

    private static ProcessedImage EncodeWebp(SKBitmap bitmap)
    {
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Webp, WebpQuality)
            ?? throw new InvalidOperationException("WebP encoding failed.");
        return new ProcessedImage(data.ToArray(), bitmap.Width, bitmap.Height, WebpContentType, WebpExtension);
    }

    private static (int Width, int Height) ScaleToFit(int width, int height, int maxSize)
    {
        if (width <= maxSize && height <= maxSize) { return (width, height); }
        var ratio = (double)width / height;
        return ratio >= 1
            ? (maxSize, Math.Max(1, (int)Math.Round(maxSize / ratio)))
            : (Math.Max(1, (int)Math.Round(maxSize * ratio)), maxSize);
    }
}
