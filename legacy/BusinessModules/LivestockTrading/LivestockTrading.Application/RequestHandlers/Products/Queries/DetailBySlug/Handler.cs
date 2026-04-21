namespace LivestockTrading.Application.RequestHandlers.Products.Queries.DetailBySlug;

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

		var product = await _dataAccessLayer.GetBySlug(req.Slug, cancellationToken);
		if (product == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.ProductErrors.ProductNotFound));

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
