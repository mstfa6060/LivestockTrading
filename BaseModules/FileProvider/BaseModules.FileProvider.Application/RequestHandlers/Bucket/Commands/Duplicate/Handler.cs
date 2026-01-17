namespace BaseModules.FileProvider.Application.RequestHandlers.Buckets.Commands.Duplicate;

/// <summary>
/// Bucket Çoğaltma
/// Bu endpoint, bir bucket'ı çoğaltır.
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
		var currentUserId = _currentUserService.GetCurrentUserId();
		var companyId = _currentUserService.GetCompanyId();
		var currentUserDisplayName = _currentUserService.GetCurrentUserDisplayName();

		var sourceBucket = await _dataAccessLayer.GetBucketById(requestPayload.BucketId);

		var newBucket = new FileBucket()
		{
			IsWaitingForApproval = true,
			BucketType = sourceBucket.BucketType,
			ModuleName = sourceBucket.ModuleName,
			ChangeSets = new List<ChangeSet>(),
			Files = new List<FileEntry>(),
		};

		foreach (var sourceFileEntry in sourceBucket.Files)
		{
			// Duplicate file as physical
			var sourceFileProperties = new FileProperties()
			{
				ContentType = sourceFileEntry.ContentType,
				Extention = sourceFileEntry.Extention,
				Name = sourceFileEntry.Name,
				Path = sourceFileEntry.Path,
			};
			var uploadedFileProperties = await _fileStorageService.DuplicateFile(companyId, sourceFileProperties, sourceBucket.ModuleName, requestPayload.FolderName, requestPayload.EntityId, requestPayload.VersionName);

			// Create FileEntry object
			var newFileEntry = new FileEntry()
			{
				Id = Guid.NewGuid(),
				IsWaitingForApproval = true,

				ContentType = uploadedFileProperties.ContentType,
				Extention = uploadedFileProperties.Extention,
				Name = uploadedFileProperties.Name,
				Path = uploadedFileProperties.Path,

				Index = newBucket.Files.Count,
				IsDefault = false,
				UploadedAt = DateTime.Now,

				UserDisplayName = currentUserDisplayName,
				UserId = currentUserId,
			};

			// Create Change Tracking Item
			_fileChangeTrackingService.AddFileCreating(newBucket, newFileEntry, requestPayload.ChangeId, currentUserId);

			// Add to Bucket
			newBucket.Files.Add(newFileEntry);
		}

		// Update on Document Database
		await _dataAccessLayer.CreateOrUpdateBucket(newBucket);

		var stabilizedChangeId = requestPayload.ChangeId != Guid.Empty ? requestPayload.ChangeId : Guid.NewGuid();
		var response = new Mapper().MapToResponse(newBucket, stabilizedChangeId);

		return ArfBlocksResults.Success(response);
	}
}