namespace LivestockTrading.Application.RequestHandlers.Deals.Commands.Update;

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

		var deal = await _dataAccessLayer.GetDealById(request.Id);

		if (deal == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CommonErrors.IdNotValid));

		mapper.MapToEntity(request, deal);

		await _dataAccessLayer.SaveChanges();

		var response = mapper.MapToResponse(deal);
		return ArfBlocksResults.Success(response);
	}
}
