using Common.Services.Messaging;
using LivestockTrading.Domain.Events;

namespace LivestockTrading.Application.RequestHandlers.TransportOffers.Commands.Create;

public class Handler : IRequestHandler
{
	private readonly DataAccess _dataAccessLayer;
	private readonly IRabbitMqPublisher _publisher;
	private readonly CurrentUserService _currentUserService;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
	{
		_dataAccessLayer = (DataAccess)dataAccess;
		_publisher = dependencyProvider.GetInstance<IRabbitMqPublisher>();
		_currentUserService = dependencyProvider.GetInstance<CurrentUserService>();
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var request = (RequestModel)payload;
		var mapper = new Mapper();

		var entity = mapper.MapToEntity(request);

		await _dataAccessLayer.Add(entity);

		// Notify request owner about new offer
		try
		{
			await _publisher.PublishFanout("livestocktrading.notification.push", new TransportOfferCreatedEvent
			{
				OfferId = entity.Id,
				RequestId = entity.TransportRequestId,
				TransporterUserId = entity.TransporterId,
				RequestOwnerUserId = request.RequestOwnerUserId,
				TransporterName = _currentUserService.GetCurrentUserDisplayName(),
				OfferedPrice = entity.OfferedPrice
			});
		}
		catch { }

		var response = mapper.MapToResponse(entity);
		return ArfBlocksResults.Success(response);
	}
}
