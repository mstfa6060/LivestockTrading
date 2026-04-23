namespace Common.Services.FileOperations.ImageProcessing;

public class ImageProcessingOptions
{
	/// <summary>
	/// Sikistirma kalitesi (1-100), varsayilan: 85
	/// </summary>
	public int Quality { get; set; } = 85;

	/// <summary>
	/// WebP formatina donustur
	/// </summary>
	public bool ConvertToWebP { get; set; } = true;

	/// <summary>
	/// EXIF/GPS metadata temizle
	/// </summary>
	public bool StripMetadata { get; set; } = true;

	/// <summary>
	/// Orijinal resmin maksimum genisligi
	/// </summary>
	public int MaxOriginalWidth { get; set; } = 2560;

	/// <summary>
	/// Orijinal resmin maksimum yuksekligi
	/// </summary>
	public int MaxOriginalHeight { get; set; } = 1440;

	/// <summary>
	/// Olusturulacak thumbnail boyutlari
	/// </summary>
	public List<ThumbnailSize> ThumbnailSizes { get; set; } = new()
	{
		new ThumbnailSize("thumb", 200, 200),
		new ThumbnailSize("medium", 800, 600),
		new ThumbnailSize("large", 1920, 1080)
	};

	/// <summary>
	/// Thumbnail olustur
	/// </summary>
	public bool GenerateThumbnails { get; set; } = true;
}

public class ThumbnailSize
{
	public string Name { get; set; }
	public int Width { get; set; }
	public int Height { get; set; }

	public ThumbnailSize() { }

	public ThumbnailSize(string name, int width, int height)
	{
		Name = name;
		Width = width;
		Height = height;
	}
}
