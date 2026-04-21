namespace BaseModules.FileProvider.Application.RequestHandlers.Buckets.Queries.Detail;

/// <summary>
/// Bucket Detayı
/// Bu endpoint, bir bucket'ın detaylarını getirir.
/// </summary>
public class Handler : IRequestHandler
{
	private readonly DataAccess dataAccessLayer;
	private readonly FileChangeTrackingService _fileChangeTrackingService;
	public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
	{
		dataAccessLayer = (DataAccess)dataAccess;
		_fileChangeTrackingService = dependencyProvider.GetInstance<FileChangeTrackingService>();
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var requestPayload = (RequestModel)payload;

		// Get Bucket From Document DB
		var bucket = await dataAccessLayer.GetBucketById(requestPayload.BucketId);
		var matchedChange = bucket.ChangeSets?
									.FirstOrDefault(c => c.ChangeId == requestPayload.ChangeId);

		// Build Response
		var visibleFileEntries = _fileChangeTrackingService.GetVisibleFileEntries(bucket, requestPayload.ChangeId);
		var response = new Mapper().MapToResponse(bucket, visibleFileEntries, requestPayload.ChangeId);

		return ArfBlocksResults.Success(response);
	}
}