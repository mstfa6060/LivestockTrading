namespace LivestockTrading.Application.RequestHandlers.SubscriptionPlans.Queries.Detail;

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

		var plan = await _dataAccessLayer.GetById(req.Id, cancellationToken);
		if (plan == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.SubscriptionPlanErrors.SubscriptionPlanNotFound));

		var response = mapper.MapToResponse(plan, req.LanguageCode);

		return ArfBlocksResults.Success(response);
	}
}
