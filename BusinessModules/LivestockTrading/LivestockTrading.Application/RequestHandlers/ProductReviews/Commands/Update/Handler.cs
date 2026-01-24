namespace LivestockTrading.Application.RequestHandlers.ProductReviews.Commands.Update;

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

		var productReview = await _dataAccessLayer.GetProductReviewById(request.Id);

		if (productReview == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CommonErrors.IdNotValid));

		mapper.MapToEntity(request, productReview);

		await _dataAccessLayer.SaveChanges();

		var response = mapper.MapToResponse(productReview);
		return ArfBlocksResults.Success(response);
	}
}
