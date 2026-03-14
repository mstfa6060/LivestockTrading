using LivestockTrading.Application.Authorization;

namespace LivestockTrading.Application.RequestHandlers.Products.Queries.All;

public class Handler : IRequestHandler
{
	private readonly DataAccess _dataAccessLayer;
	private readonly PermissionService _permissionService;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
	{
		_dataAccessLayer = (DataAccess)dataAccess;
		_permissionService = dependencyProvider.GetInstance<PermissionService>();
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var mapper = new Mapper();
		var req = (RequestModel)payload;
		var includeDeleted = _permissionService.IsModerator();

		var (products, page) = await _dataAccessLayer.All(
			req.CountryCode,
			req.CategoryId,
			req.Sorting,
			req.Filters,
			req.PageRequest,
			includeDeleted,
			cancellationToken);

		// TargetCurrencyCode verilmişse dönüşüm için kur bilgilerini al
		Dictionary<string, LivestockTrading.Domain.Entities.Currency> currencyRates = null;
		if (!string.IsNullOrWhiteSpace(req.TargetCurrencyCode))
			currencyRates = await _dataAccessLayer.GetCurrencyRates(cancellationToken);

		var response = mapper.MapToResponse(products, req.TargetCurrencyCode, currencyRates);

		return ArfBlocksResults.Success(response, page);
	}
}
