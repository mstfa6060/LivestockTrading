using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace Common.Services.FileOperations.ImageProcessing;

public class ImageProcessingService : IImageProcessingService
{
	private static readonly Dictionary<string, byte[]> MagicBytes = new()
	{
		{ "image/jpeg", new byte[] { 0xFF, 0xD8, 0xFF } },
		{ "image/png", new byte[] { 0x89, 0x50, 0x4E, 0x47 } },
		{ "image/gif", new byte[] { 0x47, 0x49, 0x46 } },
		{ "image/webp", new byte[] { 0x52, 0x49, 0x46, 0x46 } },
		{ "image/bmp", new byte[] { 0x42, 0x4D } }
	};

	private static readonly HashSet<string> ImageMimeTypes = new()
	{
		"image/jpeg", "image/jpg", "image/png", "image/gif",
		"image/webp", "image/bmp", "image/tiff"
	};

	public bool IsImageFile(string contentType)
	{
		return !string.IsNullOrEmpty(contentType) &&
			   ImageMimeTypes.Contains(contentType.ToLower());
	}

	public bool ValidateImageType(Stream stream, out string detectedMimeType)
	{
		detectedMimeType = null;
		if (stream == null || stream.Length < 8) return false;

		var header = new byte[8];
		var originalPosition = stream.Position;
		stream.Position = 0;
		stream.Read(header, 0, 8);
		stream.Position = originalPosition;

		foreach (var (mimeType, magic) in MagicBytes)
		{
			if (header.Take(magic.Length).SequenceEqual(magic))
			{
				detectedMimeType = mimeType;
				return true;
			}
		}
		return false;
	}

	public async Task<ProcessedImageResult> ProcessImageAsync(
		Stream imageStream,
		string fileName,
		ImageProcessingOptions options = null)
	{
		options ??= new ImageProcessingOptions();
		var result = new ProcessedImageResult();

		try
		{
			// 1. Magic byte dogrulama
			if (!ValidateImageType(imageStream, out var detectedType))
			{
				result.IsSuccess = false;
				result.ErrorMessage = "Gecersiz resim formati";
				return result;
			}

			imageStream.Position = 0;
			result.OriginalSizeBytes = imageStream.Length;

			// 2. Resmi yukle
			using var image = await Image.LoadAsync(imageStream);

			// 3. EXIF temizle
			if (options.StripMetadata)
			{
				image.Metadata.ExifProfile = null;
				image.Metadata.IptcProfile = null;
				image.Metadata.XmpProfile = null;
			}

			// 4. Orijinali isle
			result.Original = await ProcessOriginalAsync(image, options);
			result.ProcessedTotalSizeBytes = result.Original.SizeBytes;

			// 5. Thumbnail'lar olustur
			if (options.GenerateThumbnails)
			{
				foreach (var size in options.ThumbnailSizes)
				{
					var thumb = await CreateThumbnailAsync(image, size, options);
					result.Thumbnails.Add(thumb);
					result.ProcessedTotalSizeBytes += thumb.SizeBytes;
				}
			}

			result.IsSuccess = true;

			Console.WriteLine($"[ImageProcessing] {fileName}: " +
				$"{result.OriginalSizeBytes / 1024}KB -> {result.ProcessedTotalSizeBytes / 1024}KB " +
				$"({result.SavingsPercent}% tasarruf)");
		}
		catch (Exception ex)
		{
			result.IsSuccess = false;
			result.ErrorMessage = ex.Message;
			Console.WriteLine($"[ImageProcessing] HATA: {ex.Message}");
		}

		return result;
	}

	private async Task<ImageVariant> ProcessOriginalAsync(Image image, ImageProcessingOptions options)
	{
		// Max boyuta sigdir
		if (image.Width > options.MaxOriginalWidth || image.Height > options.MaxOriginalHeight)
		{
			image.Mutate(x => x.Resize(new ResizeOptions
			{
				Size = new Size(options.MaxOriginalWidth, options.MaxOriginalHeight),
				Mode = ResizeMode.Max
			}));
		}

		using var ms = new MemoryStream();

		if (options.ConvertToWebP)
		{
			await image.SaveAsWebpAsync(ms, new WebpEncoder { Quality = options.Quality });
			return new ImageVariant
			{
				Name = "original",
				Data = ms.ToArray(),
				Width = image.Width,
				Height = image.Height,
				SizeBytes = ms.Length,
				ContentType = "image/webp",
				Extension = ".webp"
			};
		}
		else
		{
			await image.SaveAsJpegAsync(ms, new JpegEncoder { Quality = options.Quality });
			return new ImageVariant
			{
				Name = "original",
				Data = ms.ToArray(),
				Width = image.Width,
				Height = image.Height,
				SizeBytes = ms.Length,
				ContentType = "image/jpeg",
				Extension = ".jpg"
			};
		}
	}

	private async Task<ImageVariant> CreateThumbnailAsync(
		Image image,
		ThumbnailSize size,
		ImageProcessingOptions options)
	{
		using var clone = image.Clone(x => x.Resize(new ResizeOptions
		{
			Size = new Size(size.Width, size.Height),
			Mode = ResizeMode.Max
		}));

		using var ms = new MemoryStream();

		if (options.ConvertToWebP)
		{
			await clone.SaveAsWebpAsync(ms, new WebpEncoder { Quality = options.Quality });
			return new ImageVariant
			{
				Name = size.Name,
				Data = ms.ToArray(),
				Width = clone.Width,
				Height = clone.Height,
				SizeBytes = ms.Length,
				ContentType = "image/webp",
				Extension = ".webp"
			};
		}
		else
		{
			await clone.SaveAsJpegAsync(ms, new JpegEncoder { Quality = options.Quality });
			return new ImageVariant
			{
				Name = size.Name,
				Data = ms.ToArray(),
				Width = clone.Width,
				Height = clone.Height,
				SizeBytes = ms.Length,
				ContentType = "image/jpeg",
				Extension = ".jpg"
			};
		}
	}

	public async Task<byte[]> CompressAsync(Stream stream, int quality = 80)
	{
		using var image = await Image.LoadAsync(stream);
		using var ms = new MemoryStream();
		await image.SaveAsJpegAsync(ms, new JpegEncoder { Quality = quality });
		return ms.ToArray();
	}

	public async Task<byte[]> ResizeAsync(Stream stream, int maxWidth, int maxHeight)
	{
		using var image = await Image.LoadAsync(stream);
		image.Mutate(x => x.Resize(new ResizeOptions
		{
			Size = new Size(maxWidth, maxHeight),
			Mode = ResizeMode.Max
		}));
		using var ms = new MemoryStream();
		await image.SaveAsJpegAsync(ms);
		return ms.ToArray();
	}

	public async Task<byte[]> StripMetadataAsync(Stream stream)
	{
		using var image = await Image.LoadAsync(stream);
		image.Metadata.ExifProfile = null;
		image.Metadata.IptcProfile = null;
		image.Metadata.XmpProfile = null;
		using var ms = new MemoryStream();
		await image.SaveAsJpegAsync(ms);
		return ms.ToArray();
	}

	public async Task<byte[]> ConvertToWebPAsync(Stream stream, int quality = 80)
	{
		using var image = await Image.LoadAsync(stream);
		using var ms = new MemoryStream();
		await image.SaveAsWebpAsync(ms, new WebpEncoder { Quality = quality });
		return ms.ToArray();
	}
}
