namespace LivestockTrading.Application.RequestHandlers.SellerReviews.Commands.Update;

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

		var sellerReview = await _dataAccessLayer.GetSellerReviewById(request.Id);

		if (sellerReview == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CommonErrors.IdNotValid));

		mapper.MapToEntity(request, sellerReview);

		await _dataAccessLayer.SaveChanges();

		var response = mapper.MapToResponse(sellerReview);
		return ArfBlocksResults.Success(response);
	}
}
