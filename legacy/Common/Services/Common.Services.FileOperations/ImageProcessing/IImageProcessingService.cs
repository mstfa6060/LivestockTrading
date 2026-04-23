namespace Common.Services.FileOperations.ImageProcessing;

public interface IImageProcessingService
{
	/// <summary>
	/// Resmi isle ve varyantlar olustur
	/// </summary>
	Task<ProcessedImageResult> ProcessImageAsync(
		Stream imageStream,
		string fileName,
		ImageProcessingOptions options = null);

	/// <summary>
	/// Tek resmi sikistir
	/// </summary>
	Task<byte[]> CompressAsync(Stream stream, int quality = 80);

	/// <summary>
	/// Belirtilen boyuta kucult
	/// </summary>
	Task<byte[]> ResizeAsync(Stream stream, int maxWidth, int maxHeight);

	/// <summary>
	/// EXIF metadata temizle
	/// </summary>
	Task<byte[]> StripMetadataAsync(Stream stream);

	/// <summary>
	/// WebP formatina donustur
	/// </summary>
	Task<byte[]> ConvertToWebPAsync(Stream stream, int quality = 80);

	/// <summary>
	/// Dosya tipini magic byte ile dogrula
	/// </summary>
	bool ValidateImageType(Stream stream, out string detectedMimeType);

	/// <summary>
	/// Dosyanin resim olup olmadigini kontrol et
	/// </summary>
	bool IsImageFile(string contentType);
}
