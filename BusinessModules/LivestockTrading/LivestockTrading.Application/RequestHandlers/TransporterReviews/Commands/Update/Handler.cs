namespace LivestockTrading.Application.RequestHandlers.TransporterReviews.Commands.Update;

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

		var transporterReview = await _dataAccessLayer.GetTransporterReviewById(request.Id);

		if (transporterReview == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CommonErrors.IdNotValid));

		mapper.MapToEntity(request, transporterReview);

		await _dataAccessLayer.SaveChanges();

		var response = mapper.MapToResponse(transporterReview);
		return ArfBlocksResults.Success(response);
	}
}
