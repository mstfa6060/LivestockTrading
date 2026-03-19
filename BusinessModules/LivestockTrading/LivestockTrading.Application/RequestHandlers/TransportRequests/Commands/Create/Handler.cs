using Common.Services.Messaging;
using LivestockTrading.Domain.Events;

namespace LivestockTrading.Application.RequestHandlers.TransportRequests.Commands.Create;

public class Handler : IRequestHandler
{
	private readonly DataAccess _dataAccessLayer;
	private readonly IRabbitMqPublisher _publisher;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
	{
		_dataAccessLayer = (DataAccess)dataAccess;
		_publisher = dependencyProvider.GetInstance<IRabbitMqPublisher>();
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var request = (RequestModel)payload;
		var mapper = new Mapper();

		var entity = mapper.MapToEntity(request);

		await _dataAccessLayer.AddTransportRequest(entity);

		// Notify transporters about new request
		try
		{
			await _publisher.PublishFanout("livestocktrading.notification.push", new TransportRequestCreatedEvent
			{
				RequestId = entity.Id,
				RequestUserId = entity.BuyerId,
				ProductTitle = entity.Notes ?? entity.SpecialInstructions ?? ""
			});
		}
		catch { }

		var response = mapper.MapToResponse(entity);
		return ArfBlocksResults.Success(response);
	}
}
