namespace LivestockTrading.Application.RequestHandlers.ProductBoosts.Commands.Create;

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

		var boostPackage = await _dataAccessLayer.GetBoostPackage(request.BoostPackageId, cancellationToken);
		if (boostPackage == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.BoostPackageErrors.BoostPackageNotFound));

		var entity = mapper.MapToEntity(request, boostPackage);
		await _dataAccessLayer.Add(entity);

		// Ürünün boost alanlarını güncelle
		await _dataAccessLayer.UpdateProductBoostFields(request.ProductId, boostPackage.BoostScore, entity.ExpiresAt);

		var response = mapper.MapToResponse(entity);
		return ArfBlocksResults.Success(response);
	}
}
