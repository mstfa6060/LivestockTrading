namespace LivestockTrading.Application.RequestHandlers.Categories.Queries.All;

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

		var (categories, productCounts) = await _dataAccessLayer.All(req.Filters, cancellationToken);

		var mapped = mapper.MapToResponse(categories, productCounts, req.LanguageCode);

		// Alphabetical sort by translated name in the selected language
		var sorted = mapped
			.OrderBy(c => c.Name, StringComparer.CurrentCultureIgnoreCase)
			.ToList();

		// In-memory pagination
		var sortedQuery = sorted.AsQueryable();
		var page = sortedQuery.GetPage(req.PageRequest);
		var paged = sortedQuery.Paginate(page).ToList();

		return ArfBlocksResults.Success(paged, page);
	}
}
