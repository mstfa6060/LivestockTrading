namespace LivestockTrading.Application.RequestHandlers.ProductReviews.Commands.Delete;

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

		var productReview = await _dataAccessLayer.GetProductReviewById(request.Id);

		if (productReview == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CommonErrors.IdNotValid));

		productReview.IsDeleted = true;
		productReview.DeletedAt = DateTime.UtcNow;

		await _dataAccessLayer.SaveChanges();

		return ArfBlocksResults.Success(new ResponseModel { Success = true });
	}
}
