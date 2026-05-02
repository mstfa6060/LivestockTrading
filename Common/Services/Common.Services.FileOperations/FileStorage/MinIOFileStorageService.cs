using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Minio;
using Minio.DataModel.Args;
using Common.Services.FileOperations.ImageProcessing;

namespace Common.Services.FileOperations.FileStorage;

public class MinIOFileStorageService : IFileStorageService
{
	private readonly IMinioClient _minioClient;
	private readonly string _bucketName;
	private readonly IImageProcessingService _imageProcessor;

	public MinIOFileStorageService(string endpoint, string accessKey, string secretKey, bool useSSL, string bucketName)
	{
		_bucketName = bucketName;
		_imageProcessor = new ImageProcessingService();

		_minioClient = new MinioClient()
			.WithEndpoint(endpoint)
			.WithCredentials(accessKey, secretKey)
			.WithSSL(useSSL)
			.Build();

		EnsureBucketExistsAsync().GetAwaiter().GetResult();
	}

	private async Task EnsureBucketExistsAsync()
	{
		var exists = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(_bucketName));
		if (!exists)
		{
			await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(_bucketName));
			Console.WriteLine($"[MinIO] Bucket '{_bucketName}' created.");
		}
	}

	public string GetBasePath() => string.Empty;

	public async Task<FileProperties> CreateFileByFormFile(Guid tenantId, IFormFile file, string moduleName, string folderName, Guid? entityId, string versionName)
	{
		using var ms = new MemoryStream();
		await file.CopyToAsync(ms);
		ms.Seek(0, SeekOrigin.Begin);
		return await CreateFileByStream(tenantId, ms, file.FileName, file.ContentType, moduleName, folderName, entityId, versionName);
	}

	public async Task<FileProperties> CreateEypOfPdfFileByFormFile(Guid tenantId, IFormFile file, string fileName, string moduleName, string folderName, Guid? entityId, string versionName)
	{
		using var ms = new MemoryStream();
		await file.CopyToAsync(ms);
		ms.Seek(0, SeekOrigin.Begin);
		return await CreateFileByStream(tenantId, ms, fileName, file.ContentType, moduleName, folderName, entityId, versionName, true);
	}

	public async Task<FileProperties> CreateFileByStream(Guid tenantId, Stream stream, string fileName, string contentType, string moduleName, string folderName, Guid? entityId, string versionName, bool overrideProhibited = false)
	{
		var props = BuildFileProperties(tenantId, fileName, contentType, moduleName, folderName, entityId, versionName);
		var objectName = props.Properties.Path.Replace("\\", "/");

		// Ensure stream is at beginning
		if (stream.CanSeek)
			stream.Seek(0, SeekOrigin.Begin);

		await _minioClient.PutObjectAsync(new PutObjectArgs()
			.WithBucket(_bucketName)
			.WithObject(objectName)
			.WithStreamData(stream)
			.WithObjectSize(stream.Length)
			.WithContentType(contentType ?? "application/octet-stream"));

		Console.WriteLine($"[MinIO] Uploaded: {_bucketName}/{objectName}");

		return props.Properties;
	}

	public (FileProperties Properties, string PhysicalFilePath) BuildFileProperties(Guid tenantId, string _fileName, string contentType, string moduleName, string folderName, Guid? entityId, string versionName)
	{
		var moduleStorage = new DefinitionModuleStorage();
		var fileName = Path.GetFileName(_fileName);
		var originalName = Path.GetFileNameWithoutExtension(_fileName);
		var extention = Path.GetExtension(_fileName);
		var fileNameForWriting = moduleStorage.GetNormalizedFileName(originalName, extention);

		// Build path without touching filesystem (MinIO doesn't need directories)
		var dirParts = new List<string> { "modules", moduleName, folderName };
		if (entityId.HasValue)
			dirParts.Add(entityId.Value.ToString());
		var relativeDirectoryPath = string.Join("/", dirParts);

		var relativeFilePath = $"{relativeDirectoryPath}/{fileNameForWriting}";

		var fileProperties = new FileProperties
		{
			Name = fileName,
			Extention = extention,
			Path = relativeFilePath,
			ContentType = contentType
		};

		return (fileProperties, relativeFilePath);
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

		var originalProps = useCanonicalKey
			? await PutVariantAtCanonicalKey(processed.Original, file.FileName, storageBucketId, storageFileId.Value)
			: await SaveVariantToMinIO(tenantId, processed.Original, file.FileName, moduleName, folderName, entityId);
		result.Variants["original"] = originalProps;
		result.Width = processed.Original.Width;
		result.Height = processed.Original.Height;

		foreach (var thumb in processed.Thumbnails)
		{
			var thumbProps = useCanonicalKey
				? await PutVariantAtCanonicalKey(thumb, file.FileName, storageBucketId, storageFileId.Value)
				: await SaveVariantToMinIO(tenantId, thumb, file.FileName, moduleName, folderName, entityId);
			result.Variants[thumb.Name] = thumbProps;
		}

		return result;
	}

	private async Task<FileProperties> SaveVariantToMinIO(
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

	// Frontend, kapak gorseli URL'ini `/file-storage/{mediaBucketId}/{coverImageFileId}` deseniyle
	// kuruyor. Kullanici yuklemelerinin de bu desende cozulmesi icin orjinal ve varyantlar
	// `<bucketId>/<fileId>` (orjinal) ve `<bucketId>/<fileId>_<variant>` anahtarlariyla yazilir.
	// Tarayici Content-Type header'ini kullandigi icin uzantisiz key gorseli dogru render eder.
	private async Task<FileProperties> PutVariantAtCanonicalKey(
		ImageVariant variant,
		string originalFileName,
		string storageBucketId,
		Guid storageFileId)
	{
		var objectName = variant.Name == "original"
			? $"{storageBucketId}/{storageFileId}"
			: $"{storageBucketId}/{storageFileId}_{variant.Name}";

		using var ms = new MemoryStream(variant.Data);
		await _minioClient.PutObjectAsync(new PutObjectArgs()
			.WithBucket(_bucketName)
			.WithObject(objectName)
			.WithStreamData(ms)
			.WithObjectSize(ms.Length)
			.WithContentType(variant.ContentType ?? "application/octet-stream"));

		Console.WriteLine($"[MinIO] Uploaded canonical: {_bucketName}/{objectName}");

		return new FileProperties
		{
			Name = Path.GetFileName(originalFileName),
			Extention = variant.Extension,
			Path = objectName,
			ContentType = variant.ContentType
		};
	}

	public async Task<bool> DeleteFile(string filePath)
	{
		try
		{
			var objectName = filePath.Replace("\\", "/");
			await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
				.WithBucket(_bucketName)
				.WithObject(objectName));
			return true;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"[MinIO] Delete error: {ex.Message}");
			return false;
		}
	}

	public async Task<byte[]> ReadFileByPath(string path)
	{
		var objectName = path.Replace("\\", "/");
		using var ms = new MemoryStream();
		await _minioClient.GetObjectAsync(new GetObjectArgs()
			.WithBucket(_bucketName)
			.WithObject(objectName)
			.WithCallbackStream(stream => stream.CopyTo(ms)));
		return ms.ToArray();
	}

	public async Task<bool> WriteFileWithPath(string path, byte[] data)
	{
		var objectName = path.Replace("\\", "/");
		using var ms = new MemoryStream(data);
		await _minioClient.PutObjectAsync(new PutObjectArgs()
			.WithBucket(_bucketName)
			.WithObject(objectName)
			.WithStreamData(ms)
			.WithObjectSize(ms.Length)
			.WithContentType("application/octet-stream"));
		return true;
	}

	public async Task<FileProperties> DuplicateFile(Guid tenantId, FileProperties sourceFile, string moduleName, string folderName, Guid? entityId, string versionName)
	{
		var data = await ReadFileByPath(sourceFile.Path);
		using var ms = new MemoryStream(data);
		return await CreateFileByStream(tenantId, ms, sourceFile.Name, sourceFile.ContentType, moduleName, folderName, entityId, versionName);
	}

	public async Task<FileProperties> CreateUserImage(Guid tenantId, string fileName, string extention, byte[] data)
	{
		if (data == null) throw new Exception("Image data byte[] is null");
		var fileNameForWriting = fileName.EndsWith(extention) ? fileName : $"{fileName}.{extention}";
		var objectName = $"{tenantId}/user_images/{fileNameForWriting}";

		using var ms = new MemoryStream(data);
		await _minioClient.PutObjectAsync(new PutObjectArgs()
			.WithBucket(_bucketName)
			.WithObject(objectName)
			.WithStreamData(ms)
			.WithObjectSize(ms.Length)
			.WithContentType("image/jpeg"));

		return new FileProperties { Name = fileName, Extention = extention, Path = objectName, ContentType = "image/jpeg" };
	}

	public async Task<FileProperties> CreateGroupImage(Guid tenantId, string fileName, string extention, byte[] data)
	{
		if (data == null) throw new Exception("Image data byte[] is null");
		var fileNameForWriting = fileName.EndsWith(extention) ? fileName : $"{fileName}.{extention}";
		var objectName = $"{tenantId}/group_images/{fileNameForWriting}";

		using var ms = new MemoryStream(data);
		await _minioClient.PutObjectAsync(new PutObjectArgs()
			.WithBucket(_bucketName)
			.WithObject(objectName)
			.WithStreamData(ms)
			.WithObjectSize(ms.Length)
			.WithContentType("image/jpeg"));

		return new FileProperties { Name = fileName, Extention = extention, Path = objectName, ContentType = "image/jpeg" };
	}

	public Stream GetFileStream(string filePath)
	{
		var ms = new MemoryStream();
		_minioClient.GetObjectAsync(new GetObjectArgs()
			.WithBucket(_bucketName)
			.WithObject(filePath.Replace("\\", "/"))
			.WithCallbackStream(stream => stream.CopyTo(ms))).GetAwaiter().GetResult();
		ms.Seek(0, SeekOrigin.Begin);
		return ms;
	}
}
