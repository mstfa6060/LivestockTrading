using Common.Services.FileOperations.ImageProcessing;

namespace BaseModules.FileProvider.Application.RequestHandlers.Files.Commands.UploadToApprovedBucket;

/// <summary>
/// Onaylı Bucket'a Dosya Yükleme
/// Bu endpoint, onaylı bucket'a dosya yükler.
/// </summary>
public class Handler : IRequestHandler
{
	private readonly DataAccess _dataAccessLayer;
	private readonly CurrentUserService _currentUserService;
	private readonly IFileStorageService _fileStorageService;
	private readonly FileChangeTrackingService _fileChangeTrackingService;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
	{
		_dataAccessLayer = (DataAccess)dataAccess;
		_currentUserService = dependencyProvider.GetInstance<CurrentUserService>();
		_fileStorageService = dependencyProvider.GetInstance<IFileStorageService>();
		_fileChangeTrackingService = dependencyProvider.GetInstance<FileChangeTrackingService>();
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var requestPayload = (RequestModel)payload;
		var mapper = new Mapper();
		var currentClientId = _currentUserService.GetCurrentUserId();
		var companyId = _currentUserService.GetCompanyId();

		var bucket = await _dataAccessLayer.GetBucketById(requestPayload.BucketId);
		if (bucket == null)
		{
			bucket = new FileBucket()
			{
				IsWaitingForApproval = false,
				BucketType = requestPayload.BucketType,
				ModuleName = requestPayload.ModuleName,
				ChangeSets = new List<ChangeSet>(),
				Files = new List<FileEntry>(),
			};
		}

		// Create File as Physical
		FileProperties uploadedFileProperties;
		List<FileVariantInfo> variantPaths = null;
		int? imageWidth = null;
		int? imageHeight = null;
		long? fileSizeBytes = null;

		var isImage = requestPayload.FormFile.ContentType?.StartsWith("image/") == true;

		if (isImage)
		{
			// Resim isleme servisi ile yukle
			var imageResult = await _fileStorageService.CreateImageFileAsync(
				companyId,
				requestPayload.FormFile,
				requestPayload.ModuleName,
				requestPayload.FolderName,
				requestPayload.EntityId,
				new ImageProcessingOptions
				{
					Quality = 85,
					ConvertToWebP = true,
					StripMetadata = true,
					GenerateThumbnails = true
				});

			uploadedFileProperties = imageResult.Variants["original"];
			variantPaths = imageResult.Variants.Select(v => new FileVariantInfo { Key = v.Key, Url = v.Value.Path }).ToList();
			imageWidth = imageResult.Width;
			imageHeight = imageResult.Height;
			fileSizeBytes = imageResult.ProcessedSizeBytes;

			Console.WriteLine($"[UploadToApprovedBucket] Resim islendi: {imageResult.SavingsPercent}% tasarruf");
		}
		else
		{
			// Normal dosya yukleme
			uploadedFileProperties = await _fileStorageService.CreateFileByFormFile(companyId, requestPayload.FormFile, requestPayload.ModuleName, requestPayload.FolderName, requestPayload.EntityId, requestPayload.VersionName);
			fileSizeBytes = requestPayload.FormFile.Length;
		}

		// Create FileEntry Entity
		var fileEntry = new FileEntry()
		{
			Id = Guid.NewGuid(),
			IsWaitingForApproval = false,

			ContentType = uploadedFileProperties.ContentType,
			Extention = uploadedFileProperties.Extention,
			Name = uploadedFileProperties.Name,
			Path = uploadedFileProperties.Path,

			Index = bucket.Files.Count,
			IsDefault = false,
			UploadedAt = DateTime.Now,

			UserDisplayName = _currentUserService.GetCurrentUserDisplayName(),
			UserId = currentClientId,

			// Resim varyantlari
			Variants = variantPaths,
			Width = imageWidth,
			Height = imageHeight,
			SizeBytes = fileSizeBytes
		};

		// Add to Bucket
		bucket.Files.Add(fileEntry);

		// Create Change Tracking Item
		var changeSet = _fileChangeTrackingService.AddFileCreating(bucket, fileEntry, requestPayload.ChangeId, currentClientId);

		// Update on Document Database
		await _dataAccessLayer.CreateOrUpdateBucket(bucket);

		// Build response
		var visibleFileEntries = _fileChangeTrackingService.GetVisibleFileEntries(bucket, changeSet.ChangeId);
		var response = mapper.MapToResponse(bucket, visibleFileEntries, changeSet.ChangeId);

		return ArfBlocksResults.Success(response);
	}
}