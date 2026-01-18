using Common.Services.Auth.Authorization;
using Common.Services.Auth.CurrentUser;

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
				IsWaitingForApproval = true,
				BucketType = requestPayload.BucketType,
				ModuleName = requestPayload.ModuleName,
				ChangeSets = new List<ChangeSet>(),
				Files = new List<FileEntry>(),
			};
		}

		// Create File as Physical
		var uploadedFileProperties = await _fileStorageService.CreateFileByFormFile(companyId, requestPayload.FormFile, requestPayload.ModuleName, requestPayload.FolderName, requestPayload.EntityId, requestPayload.VersionName);
		// Create FileEntry Entity
		var fileEntry = new FileEntry()
		{
			Id = Guid.NewGuid(),
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
