using Common.Services.Auth.Authorization;
using Common.Services.Auth.CurrentUser;
using Common.Services.FileOperations.ImageProcessing;
using MongoDB.Bson;

namespace BaseModules.FileProvider.Application.RequestHandlers.Files.Commands.Upload;

/// <summary>
/// Dosya Yükleme
/// Bu endpoint, sisteme dosya yüklemeyi sağlar.
/// </summary>
public class Handler : IRequestHandler
{
	private readonly DataAccess _dataAccessLayer;
	private readonly CurrentUserService _currentUserService;
	private readonly IFileStorageService _fileStorageService;
	private readonly FileChangeTrackingService _fileChangeTrackingService;
	private readonly AuthorizationService _authorizationService;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
	{
		_dataAccessLayer = (DataAccess)dataAccess;
		_currentUserService = dependencyProvider.GetInstance<CurrentUserService>();
		_fileStorageService = dependencyProvider.GetInstance<IFileStorageService>();
		_fileChangeTrackingService = dependencyProvider.GetInstance<FileChangeTrackingService>();
		_authorizationService = dependencyProvider.GetInstance<AuthorizationService>();
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var requestPayload = (RequestModel)payload;
		var mapper = new Mapper();
		var currentClientId = _currentUserService.GetCurrentUserId();

		var isCurrentUserAuthorizedOnModule = await IsCurrentUserAuthorizedOnModule(requestPayload.ModuleName);
		var companyId = Guid.Empty;

		if (isCurrentUserAuthorizedOnModule && requestPayload.TenantId.HasValue && requestPayload.TenantId.Value != Guid.Empty)
			companyId = requestPayload.TenantId.Value;
		else
			companyId = _currentUserService.GetCompanyId();

		var bucket = await _dataAccessLayer.GetBucketById(requestPayload.BucketId);
		if (bucket == null)
		{
			bucket = new FileBucket()
			{
				// Frontend'in `/file-storage/{mediaBucketId}/{coverImageFileId}` URL desenine uyabilmesi
				// icin bucket ve dosya kimlikleri storage'a yazmadan once uretiliyor; MinIO anahtari
				// `<bucket.Id>/<fileEntry.Id>` olarak yazilir, boylece kapak URL'i dogrudan cozulebilir.
				Id = ObjectId.GenerateNewId().ToString(),
				IsWaitingForApproval = true,
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

		var fileEntryId = Guid.NewGuid();
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
				},
				storageBucketId: bucket.Id,
				storageFileId: fileEntryId);

			uploadedFileProperties = imageResult.Variants["original"];
			variantPaths = imageResult.Variants.Select(v => new FileVariantInfo { Key = v.Key, Url = v.Value.Path }).ToList();
			imageWidth = imageResult.Width;
			imageHeight = imageResult.Height;
			fileSizeBytes = imageResult.ProcessedSizeBytes;

			Console.WriteLine($"[Upload] Resim islendi: {imageResult.SavingsPercent}% tasarruf");
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
			Id = fileEntryId,
			IsWaitingForApproval = true,

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

	public async Task<bool> IsCurrentUserAuthorizedOnModule(string module)
	{
		switch (module.ToLower())
		{
			case "livestocktrading":
				return await _authorizationService.IsSystemAdmin(Common.Definitions.Domain.Entities.ModuleTypes.LivestockTrading);
			default:
				System.Console.WriteLine($"Module not handled: {module}");
				return false;
		}
	}
}
