using LivestockTrading.Application.Services;

namespace LivestockTrading.Application.RequestHandlers.SellerSubscriptions.Commands.Create;

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

		var entity = mapper.MapToEntity(request);

		await _dataAccessLayer.Add(entity);

		// Satıcının aktif abonelik referansını güncelle
		await _dataAccessLayer.UpdateSellerActiveSubscription(request.SellerId, entity.Id);

		var response = mapper.MapToResponse(entity);
		return ArfBlocksResults.Success(response);
	}
}
