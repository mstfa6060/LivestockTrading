namespace LivestockTrading.Application.RequestHandlers.Offers.Commands.Update;

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

		var offer = await _dataAccessLayer.GetOfferById(request.Id);

		if (offer == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CommonErrors.IdNotValid));

		mapper.MapToEntity(request, offer);

		await _dataAccessLayer.SaveChanges();

		var response = mapper.MapToResponse(offer);
		return ArfBlocksResults.Success(response);
	}
}
