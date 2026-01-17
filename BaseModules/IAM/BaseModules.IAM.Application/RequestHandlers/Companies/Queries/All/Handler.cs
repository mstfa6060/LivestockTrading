namespace BaseModules.IAM.Application.RequestHandlers.Companies.Queries.All;

/// <summary>
/// Tüm Şirketleri Listele
/// Bu endpoint, sistemdeki tüm şirketlerin listesini döner.
/// Filtreleme ve sayfalama desteği sunar.
/// </summary>
public class Handler : IRequestHandler
{
	private readonly DataAccess _dataAccessLayer;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, DataAccess dataAccess)
	{
		_dataAccessLayer = dataAccess;
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var request = (RequestModel)payload;
		var mapper = new Mapper();

		var (companies, pageInfo) = await _dataAccessLayer.All(request.Sorting, request.Filters, request.PageRequest);
		var response = mapper.MapToResponse(companies);

		return ArfBlocksResults.Success(response, pageInfo);
	}
}
