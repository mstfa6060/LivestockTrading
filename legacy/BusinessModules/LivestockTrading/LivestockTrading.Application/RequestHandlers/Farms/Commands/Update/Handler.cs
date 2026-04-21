namespace LivestockTrading.Application.RequestHandlers.Farms.Commands.Update;

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

		var farm = await _dataAccessLayer.GetFarmById(request.Id);

		if (farm == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CommonErrors.IdNotValid));

		mapper.MapToEntity(request, farm);

		await _dataAccessLayer.SaveChanges();

		var response = mapper.MapToResponse(farm);
		return ArfBlocksResults.Success(response);
	}
}
