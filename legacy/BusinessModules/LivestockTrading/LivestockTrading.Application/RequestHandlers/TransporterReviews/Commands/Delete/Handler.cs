namespace LivestockTrading.Application.RequestHandlers.TransporterReviews.Commands.Delete;

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

		var transporterReview = await _dataAccessLayer.GetTransporterReviewById(request.Id);

		if (transporterReview == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CommonErrors.IdNotValid));

		transporterReview.IsDeleted = true;
		transporterReview.DeletedAt = DateTime.UtcNow;

		await _dataAccessLayer.SaveChanges();

		return ArfBlocksResults.Success(new ResponseModel { Success = true });
	}
}
