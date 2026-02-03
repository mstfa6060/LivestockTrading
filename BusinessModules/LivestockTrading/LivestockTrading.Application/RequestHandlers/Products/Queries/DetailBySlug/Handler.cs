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

		var response = mapper.MapToResponse(product);

		return ArfBlocksResults.Success(response);
	}
}
