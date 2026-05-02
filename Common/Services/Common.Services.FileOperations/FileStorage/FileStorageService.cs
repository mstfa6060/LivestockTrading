using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Common.Services.FileOperations.ImageProcessing;

namespace Common.Services.FileOperations.FileStorage;

public class FileStorageService : IFileStorageService
{
	private readonly IImageProcessingService _imageProcessor;

	public FileStorageService()
	{
		_imageProcessor = new ImageProcessingService();
	}

	public FileStorageService(IImageProcessingService imageProcessor)
	{
		_imageProcessor = imageProcessor ?? new ImageProcessingService();
	}

	public string GetBasePath()
	{
		return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
	}

	// Modül bazlı path döndürür
	private string GetModuleBasePath(string moduleName)
	{
		switch (moduleName.ToLower())
		{
			case "livestocktrading":
				var livestockTradingPath = "/app/wwwroot/livestocktrading-storage";
				if (Directory.Exists(livestockTradingPath))
				{
					Console.WriteLine($"Using livestocktrading storage: {livestockTradingPath}");
					return livestockTradingPath;
				}
				break;
		}

		// Fallback: default wwwroot
		Console.WriteLine($"Module storage not found for '{moduleName}', using default wwwroot");
		return this.GetBasePath();
	}
	public async Task<bool> DeleteFile(string filePath)
	{
		try
		{
			var basePath = this.GetBasePath();
			var fullPath = Path.Combine(basePath, filePath);

			if (!File.Exists(fullPath))
				return await Task.FromResult(false);

			File.Delete(fullPath);
			return await Task.FromResult(true);
		}
		catch (Exception ex)
		{
			System.Console.WriteLine($"File Storage Delete Exception: {ex.Message}");
			return await Task.FromResult(false);
		}
	}

	public async Task<FileProperties> DuplicateFile(Guid tenantId, FileProperties sourceFileProperties, string moduleName, string folderName, Guid? entityId, string versionName)
	{
		// Source
		var absoluteFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", sourceFileProperties.Path);
		var sourceFileStream = new FileStream(absoluteFilePath, FileMode.Open);

		// Target
		var result = BuildFileProperties(tenantId, sourceFileProperties.Name, sourceFileProperties.ContentType, moduleName, folderName, entityId, versionName);

		// Create File
		using (FileStream targetFileStream = System.IO.File.Create(result.PhysicalFilePath))
		{
			await sourceFileStream.CopyToAsync(targetFileStream);
			targetFileStream.Flush();
		}

		return result.Properties;
	}

	public async Task<FileProperties> CreateFileByFormFile(Guid tenantId, IFormFile file, string moduleName, string folderName, Guid? entityId, string versionName)
	{
		var ms = new MemoryStream();
		file.CopyTo(ms);
		ms.Seek(0, SeekOrigin.Begin);
		// ms.Flush();
		return await this.CreateFileByStream(tenantId, ms, file.FileName, file.ContentType, moduleName, folderName, entityId, versionName);

	}

	public async Task<FileProperties> CreateEypOfPdfFileByFormFile(Guid tenantId, IFormFile file, string fileName, string moduleName, string folderName, Guid? entityId, string versionName)
	{
		var ms = new MemoryStream();
		file.CopyTo(ms);
		ms.Seek(0, SeekOrigin.Begin);
		// ms.Flush();
		return await this.CreateFileByStream(tenantId, ms, fileName, file.ContentType, moduleName, folderName, entityId, versionName, true);
	}

	public async Task<FileProperties> CreateFileByStream(Guid tenantId, Stream stream, string fileName, string contentType, string moduleName, string folderName, Guid? entityId, string versionName, bool overrideProhibited = false)
	{
		// 1. Yol bilgisi oluşturuluyor
		var result = BuildFileProperties(tenantId, fileName, contentType, moduleName, folderName, entityId, versionName);

		// 2. 📂 Klasör var mı kontrol et, yoksa oluştur
		var directory = Path.GetDirectoryName(result.PhysicalFilePath);
		if (!Directory.Exists(directory))
			Directory.CreateDirectory(directory);

		// 3. Aynı dosya varsa ve üzerine yazmak yasaksa → hata fırlat
		if (File.Exists(result.PhysicalFilePath) && overrideProhibited)
			throw new Exception("FILE_ALREADY_EXIST");

		// 4. Stream'den byte[] oku
		byte[] bytes;
		using (var memoryStream = new MemoryStream())
		{
			stream.CopyTo(memoryStream);
			bytes = memoryStream.ToArray();
		}

		// 5. Eğer aynı dosya varsa sil
		if (File.Exists(result.PhysicalFilePath))
			File.Delete(result.PhysicalFilePath);

		// 6. Dosyayı diske yaz
		File.WriteAllBytes(result.PhysicalFilePath, bytes);

		// 7. Meta veriyi geri dön
		return await Task.FromResult(result.Properties);
	}


	public (FileProperties Properties, string PhysicalFilePath) BuildFileProperties(Guid tenantId, string _fileName, string contentType, string moduleName, string folderName, Guid? entityId, string versionName)
	{

		IModuleStorage moduleStorage;

		switch (moduleName.ToLower())
		{
			case "livestocktrading":
				moduleStorage = new DefinitionModuleStorage();
				break;
			default:
				moduleStorage = new DefinitionModuleStorage();
				break;
		}


		// var file = files[0];
		var fileName = Path.GetFileName(_fileName);
		var originalName = Path.GetFileNameWithoutExtension(_fileName);
		var extention = Path.GetExtension(_fileName);
		var fileNameForWriting = moduleStorage.GetNormalizedFileName(originalName, extention);

		//  Modül bazlı basePath kullan
		var basePath = this.GetModuleBasePath(moduleName);
		var relativeDirectoryPath = moduleStorage.GetDirectoryRelativePathForUploadFile(basePath, moduleName, tenantId, folderName, entityId, versionName);

		// For Windows
		// var physicalFilePath = basePath + "" + relativeDirectoryPath + "\\" + fileNameForWriting;

		var physicalFilePath = Path.Combine(basePath, relativeDirectoryPath, fileNameForWriting);
		var relativeFilePath = Path.Combine(relativeDirectoryPath, fileNameForWriting);
		System.Console.WriteLine($"PhysicalFilePath: {physicalFilePath}");
		System.Console.WriteLine($"RelativeFilePath: {relativeFilePath}");

		var fileProperties = new FileProperties
		{
			Name = fileName,
			Extention = extention,
			Path = relativeFilePath,
			ContentType = contentType
		};

		return (fileProperties, physicalFilePath);
	}

	public async Task<FileProperties> CreateUserImage(Guid tenantId, string fileName, string extention, byte[] data)
	{
		// NOP:
		await Task.FromResult(false);

		try
		{
			if (data == null)
				throw new Exception("Image data byte[] is null");

			var basePath = this.GetBasePath();
			var directoryPath = GetDirectoryForUserImage(basePath, tenantId);

			var fileNameForWriting = fileName.EndsWith(extention) ? fileName : $"{fileName}.{extention}";
			var filePath = Path.Combine(basePath, directoryPath, fileNameForWriting);

			// Delete File If Exist
			try
			{
				if (File.Exists(filePath))
					File.Delete(filePath);
			}
			catch { }

			// Create File
			using (var ms = new MemoryStream(data))
			{
				using (var fs = new FileStream(filePath, FileMode.Create))
				{
					ms.WriteTo(fs);
				}
			}

			return new FileProperties
			{
				Name = fileName,
				Extention = extention,
				Path = $@"{directoryPath}/{fileNameForWriting}",
				ContentType = "image/jpeg"
			};
		}
		catch (Exception ex)
		{
			System.Console.WriteLine($"File Storage User Image Create Exception: {ex.Message}{System.Environment.NewLine}{ex.InnerException?.Message}");
			return new FileProperties() { };
		}
	}

	public async Task<FileProperties> CreateGroupImage(Guid tenantId, string fileName, string extention, byte[] data)
	{
		// NOP:
		await Task.FromResult(false);

		try
		{
			if (data == null)
				throw new Exception("Image data byte[] is null");

			var basePath = this.GetBasePath();
			var directoryPath = GetDirectoryForGroupImage(basePath, tenantId);

			var fileNameForWriting = fileName.EndsWith(extention) ? fileName : $"{fileName}.{extention}";
			var filePath = Path.Combine(basePath, directoryPath, fileNameForWriting);

			// Delete File If Exist
			try
			{
				if (File.Exists(filePath))
					File.Delete(filePath);
			}
			catch { }

			// Create File
			using (var ms = new MemoryStream(data))
			{
				using (var fs = new FileStream(filePath, FileMode.Create))
				{
					ms.WriteTo(fs);
				}
			}

			return new FileProperties
			{
				Name = fileName,
				Extention = extention,
				Path = $@"{directoryPath}/{fileNameForWriting}",
				ContentType = "image/jpeg"
			};
		}
		catch (Exception ex)
		{
			System.Console.WriteLine($"File Storage Group Image Create Exception: {ex.Message}{System.Environment.NewLine}{ex.InnerException?.Message}");
			return new FileProperties() { };
		}
	}

	public string GetDirectoryForUserImage(string basePath, Guid tenantId)
	{
		var tenantPath = Path.Combine(basePath, tenantId.ToString());
		var newPathName = Path.Combine(tenantPath, "user_images");
		if (!Directory.Exists(newPathName))
			Directory.CreateDirectory(newPathName);

		newPathName = newPathName.Replace(basePath, "");
		newPathName = newPathName.StartsWith("/") ? newPathName.Substring(1, newPathName.Length - 1) : newPathName;

		return newPathName;
	}

	public string GetDirectoryForGroupImage(string basePath, Guid tenantId)
	{
		var tenantPath = Path.Combine(basePath, tenantId.ToString());
		var newPathName = Path.Combine(tenantPath, "group_images");
		if (!Directory.Exists(newPathName))
			Directory.CreateDirectory(newPathName);

		newPathName = newPathName.Replace(basePath, "");
		newPathName = newPathName.StartsWith("/") ? newPathName.Substring(1, newPathName.Length - 1) : newPathName;

		return newPathName;
	}

	public async Task<byte[]> ReadFileByPath(string path)
	{
		var basePath = this.GetBasePath();
		var fullPath = Path.Combine(basePath, path);

		if (!File.Exists(fullPath))
			throw new FileNotFoundException($"Requested File Not Found: {fullPath}");

		return await File.ReadAllBytesAsync(fullPath);
	}

	public async Task<bool> WriteFileWithPath(string path, byte[] data)
	{
		var basePath = this.GetBasePath();
		var fullPath = Path.Combine(basePath, path);

		try
		{
			using (FileStream file = new FileStream(fullPath, FileMode.Create, System.IO.FileAccess.Write))
			{
				await file.WriteAsync(data);
			}

			return true;
		}
		catch (System.Exception)
		{
			throw;
		}
	}
	public async Task<bool> CreateFileWithStream(string path, Stream stream)
	{
		// NOP:
		await Task.CompletedTask;

		var basePath = this.GetBasePath();
		var fullPath = Path.Combine(basePath, path);

		// Create File
		using (FileStream filestream = System.IO.File.Create(fullPath))
		{
			stream.CopyTo(filestream);
			filestream.Flush();
		}

		return true;
	}

	public Stream GetFileStream(string filePath)
	{
		var absoluteFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath);
		var sourceFileStream = File.OpenRead(absoluteFilePath);

		return sourceFileStream;
	}

	public async Task<ImageUploadResult> CreateImageFileAsync(
		Guid tenantId,
		IFormFile file,
		string moduleName,
		string folderName,
		Guid? entityId,
		ImageProcessingOptions options = null,
		string storageBucketId = null,
		Guid? storageFileId = null)
	{
		options ??= new ImageProcessingOptions();

		using var stream = file.OpenReadStream();
		var processed = await _imageProcessor.ProcessImageAsync(stream, file.FileName, options);

		if (!processed.IsSuccess)
			throw new Exception($"Resim isleme hatasi: {processed.ErrorMessage}");

		var result = new ImageUploadResult
		{
			Variants = new Dictionary<string, FileProperties>(),
			OriginalSizeBytes = processed.OriginalSizeBytes,
			ProcessedSizeBytes = processed.ProcessedTotalSizeBytes,
			SavingsPercent = processed.SavingsPercent
		};

		var useCanonicalKey = !string.IsNullOrWhiteSpace(storageBucketId) && storageFileId.HasValue;

		// Orijinali kaydet
		var originalProps = useCanonicalKey
			? await SaveVariantAtCanonicalKey(processed.Original, file.FileName, moduleName, storageBucketId, storageFileId.Value)
			: await SaveVariantAsync(tenantId, processed.Original, file.FileName, moduleName, folderName, entityId);
		result.Variants["original"] = originalProps;
		result.Width = processed.Original.Width;
		result.Height = processed.Original.Height;

		// Thumbnail'lari kaydet
		foreach (var thumb in processed.Thumbnails)
		{
			var thumbProps = useCanonicalKey
				? await SaveVariantAtCanonicalKey(thumb, file.FileName, moduleName, storageBucketId, storageFileId.Value)
				: await SaveVariantAsync(tenantId, thumb, file.FileName, moduleName, folderName, entityId);
			result.Variants[thumb.Name] = thumbProps;
		}

		return result;
	}

	private async Task<FileProperties> SaveVariantAsync(
		Guid tenantId,
		ImageVariant variant,
		string originalFileName,
		string moduleName,
		string folderName,
		Guid? entityId)
	{
		var baseName = Path.GetFileNameWithoutExtension(originalFileName);
		var newFileName = variant.Name == "original"
			? $"{baseName}{variant.Extension}"
			: $"{baseName}_{variant.Name}{variant.Extension}";

		using var ms = new MemoryStream(variant.Data);
		return await CreateFileByStream(tenantId, ms, newFileName, variant.ContentType, moduleName, folderName, entityId, variant.Name);
	}

	// MinIO ile aynı `<bucketId>/<fileId>` deseni; ortamda MinIO yoksa diske de tutarli yazilsin diye.
	private async Task<FileProperties> SaveVariantAtCanonicalKey(
		ImageVariant variant,
		string originalFileName,
		string moduleName,
		string storageBucketId,
		Guid storageFileId)
	{
		var relativeFilePath = variant.Name == "original"
			? $"{storageBucketId}/{storageFileId}"
			: $"{storageBucketId}/{storageFileId}_{variant.Name}";

		var basePath = this.GetModuleBasePath(moduleName);
		var physicalFilePath = Path.Combine(basePath, relativeFilePath.Replace("/", Path.DirectorySeparatorChar.ToString()));

		var directory = Path.GetDirectoryName(physicalFilePath);
		if (!Directory.Exists(directory))
			Directory.CreateDirectory(directory);

		if (File.Exists(physicalFilePath))
			File.Delete(physicalFilePath);

		await File.WriteAllBytesAsync(physicalFilePath, variant.Data);

		return new FileProperties
		{
			Name = Path.GetFileName(originalFileName),
			Extention = variant.Extension,
			Path = relativeFilePath,
			ContentType = variant.ContentType
		};
	}
}