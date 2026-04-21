namespace LivestockTrading.Application.RequestHandlers.Banners.Commands.Delete;

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

		var banner = await _dataAccessLayer.GetBannerById(request.Id);

		if (banner == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CommonErrors.IdNotValid));

		banner.IsDeleted = true;
		banner.DeletedAt = DateTime.UtcNow;

		await _dataAccessLayer.SaveChanges();

		return ArfBlocksResults.Success(new ResponseModel { Success = true });
	}
}
