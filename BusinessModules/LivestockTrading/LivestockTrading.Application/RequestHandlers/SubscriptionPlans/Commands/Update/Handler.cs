namespace LivestockTrading.Application.RequestHandlers.SubscriptionPlans.Commands.Update;

public class Handler : IRequestHandler
{
	private readonly DataAccess _dataAccessLayer;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
	{
		_dataAccessLayer = (DataAccess)dataAccess;
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var request = (RequestModel)payload;
		var mapper = new Mapper();

		var plan = await _dataAccessLayer.GetById(request.Id);

		if (plan == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CommonErrors.IdNotValid));

		mapper.MapToEntity(request, plan);

		await _dataAccessLayer.SaveChanges();

		var response = mapper.MapToResponse(plan);
		return ArfBlocksResults.Success(response);
	}
}
