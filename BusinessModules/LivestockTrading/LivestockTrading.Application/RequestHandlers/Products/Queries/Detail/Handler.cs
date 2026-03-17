using LivestockTrading.Application.Authorization;

namespace LivestockTrading.Application.RequestHandlers.Products.Queries.Detail;

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

		var product = await _dataAccessLayer.GetById(req.Id, includeDeleted, cancellationToken);
		if (product == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CommonErrors.IdNotValid));

		// ViewerCurrencyCode verilmişse pre-computed ProductPrice kaydını al
		LivestockTrading.Domain.Entities.ProductPrice viewerProductPrice = null;
		string viewerCurrencySymbol = null;
		if (!string.IsNullOrWhiteSpace(req.ViewerCurrencyCode))
		{
			viewerProductPrice = await _dataAccessLayer.GetProductPriceForCurrency(product.Id, req.ViewerCurrencyCode, cancellationToken);
			if (viewerProductPrice != null)
				viewerCurrencySymbol = await _dataAccessLayer.GetCurrencySymbol(req.ViewerCurrencyCode, cancellationToken);
		}

		var response = mapper.MapToResponse(product, viewerProductPrice, req.ViewerCurrencyCode, viewerCurrencySymbol);

		return ArfBlocksResults.Success(response);
	}
}
