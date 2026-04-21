namespace Common.Services.FileOperations.ImageProcessing;

public class ProcessedImageResult
{
	public bool IsSuccess { get; set; }
	public string ErrorMessage { get; set; }

	/// <summary>
	/// Islenmis orijinal resim
	/// </summary>
	public ImageVariant Original { get; set; }

	/// <summary>
	/// Thumbnail varyantlari (thumb, medium, large)
	/// </summary>
	public List<ImageVariant> Thumbnails { get; set; } = new();

	/// <summary>
	/// Orijinal dosya boyutu (bytes)
	/// </summary>
	public long OriginalSizeBytes { get; set; }

	/// <summary>
	/// Islem sonrasi toplam boyut
	/// </summary>
	public long ProcessedTotalSizeBytes { get; set; }

	/// <summary>
	/// Tasarruf orani (%)
	/// </summary>
	public double SavingsPercent =>
		OriginalSizeBytes > 0
			? Math.Round((1 - (double)ProcessedTotalSizeBytes / OriginalSizeBytes) * 100, 1)
			: 0;
}

public class ImageVariant
{
	/// <summary>
	/// Varyant adi: original, thumb, medium, large
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// Islenmis resim verisi
	/// </summary>
	public byte[] Data { get; set; }

	/// <summary>
	/// Genislik (px)
	/// </summary>
	public int Width { get; set; }

	/// <summary>
	/// Yukseklik (px)
	/// </summary>
	public int Height { get; set; }

	/// <summary>
	/// Dosya boyutu (bytes)
	/// </summary>
	public long SizeBytes { get; set; }

	/// <summary>
	/// MIME type: image/webp, image/jpeg
	/// </summary>
	public string ContentType { get; set; }

	/// <summary>
	/// Dosya uzantisi: .webp, .jpg
	/// </summary>
	public string Extension { get; set; }
}
