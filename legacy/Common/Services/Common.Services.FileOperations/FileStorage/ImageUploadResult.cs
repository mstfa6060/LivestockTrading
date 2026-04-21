namespace Common.Services.FileOperations.FileStorage;

public class ImageUploadResult
{
	/// <summary>
	/// Varyant path'leri: original, thumb, medium, large
	/// </summary>
	public Dictionary<string, FileProperties> Variants { get; set; } = new();

	/// <summary>
	/// Orijinal resim genisligi
	/// </summary>
	public int Width { get; set; }

	/// <summary>
	/// Orijinal resim yuksekligi
	/// </summary>
	public int Height { get; set; }

	/// <summary>
	/// Orijinal dosya boyutu (bytes)
	/// </summary>
	public long OriginalSizeBytes { get; set; }

	/// <summary>
	/// Islem sonrasi toplam boyut
	/// </summary>
	public long ProcessedSizeBytes { get; set; }

	/// <summary>
	/// Tasarruf orani (%)
	/// </summary>
	public double SavingsPercent { get; set; }
}
