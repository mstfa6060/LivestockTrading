namespace LivestockTrading.Application.RequestHandlers.Banners.Commands.Update;

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

		var banner = await _dataAccessLayer.GetBannerById(request.Id);

		if (banner == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CommonErrors.IdNotValid));

		mapper.MapToEntity(request, banner);

		await _dataAccessLayer.SaveChanges();

		var response = mapper.MapToResponse(banner);
		return ArfBlocksResults.Success(response);
	}
}
