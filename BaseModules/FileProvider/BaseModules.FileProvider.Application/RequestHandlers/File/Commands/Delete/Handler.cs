using Common.Services.Auth.CurrentUser;

namespace BaseModules.FileProvider.Application.RequestHandlers.Files.Commands.Delete;

/// <summary>
/// Dosya Silme
/// Bu endpoint, yüklü bir dosyayı siler.
/// </summary>
public class Handler : IRequestHandler
{
	private readonly DataAccess _dataAccessLayer;
	// private readonly IFileStorageService _fileStorageService;
	private readonly CurrentUserService _currentUserService;
	private readonly FileChangeTrackingService _fileChangeTrackingService;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
	{
		_dataAccessLayer = (DataAccess)dataAccess;
		// _fileStorageService = dependencyProvider.GetInstance<IFileStorageService>();
		_currentUserService = dependencyProvider.GetInstance<CurrentUserService>();
		_fileChangeTrackingService = dependencyProvider.GetInstance<FileChangeTrackingService>();
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var requestPayload = (RequestModel)payload;
		var mapper = new Mapper();
		var currentClientId = _currentUserService.GetCurrentUserId();

		//  Get Data
		var bucket = await _dataAccessLayer.GetBucketById(requestPayload.BucketId);
		var fileEntry = bucket.Files.FirstOrDefault(e => e.Id == requestPayload.FileId);

		// Create Change Tracking Item
		var changeSet = _fileChangeTrackingService.AddFileDeleting(bucket, fileEntry, requestPayload.ChangeId, currentClientId);

		// bucket.Files.Remove(fileEntry);
		await _dataAccessLayer.UpdateBucket(bucket);

		// Return Response
		var response = mapper.MapToResponse(bucket, fileEntry, changeSet);

		return ArfBlocksResults.Success(response);
	}
}