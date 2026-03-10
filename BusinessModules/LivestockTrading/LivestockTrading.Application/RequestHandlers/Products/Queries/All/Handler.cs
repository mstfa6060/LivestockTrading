namespace LivestockTrading.Application.RequestHandlers.Products.Queries.All;

public class Handler : IRequestHandler
{
	private readonly DataAccess _dataAccessLayer;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
	{
		_dataAccessLayer = (DataAccess)dataAccess;
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var mapper = new Mapper();
		var req = (RequestModel)payload;

		var (products, page) = await _dataAccessLayer.All(
			req.CountryCode,
			req.Sorting,
			req.Filters,
			req.PageRequest,
			cancellationToken);

		// Batch resolve cover image paths
		var coverImageFileIds = products
			.Where(p => !string.IsNullOrWhiteSpace(p.CoverImageFileId))
			.Select(p => p.CoverImageFileId)
			.ToList();

		var imagePaths = coverImageFileIds.Count > 0
			? await _dataAccessLayer.GetCoverImagePaths(coverImageFileIds, cancellationToken)
			: new Dictionary<string, string>();

		var response = mapper.MapToResponse(products, imagePaths);

		return ArfBlocksResults.Success(response, page);
	}
}
