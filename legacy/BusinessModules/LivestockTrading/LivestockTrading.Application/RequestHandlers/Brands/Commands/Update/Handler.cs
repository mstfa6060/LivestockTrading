namespace LivestockTrading.Application.RequestHandlers.Brands.Commands.Update;

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

		var brand = await _dataAccessLayer.GetBrandById(request.Id);

		if (brand == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CommonErrors.IdNotValid));

		mapper.MapToEntity(request, brand);

		await _dataAccessLayer.SaveChanges();

		var response = mapper.MapToResponse(brand);
		return ArfBlocksResults.Success(response);
	}
}
