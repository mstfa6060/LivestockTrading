using Common.Services.Messaging;
using LivestockTrading.Domain.Events;

namespace LivestockTrading.Application.RequestHandlers.TransportOffers.Commands.Update;

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

		var entity = (await _dataAccessLayer.GetById(request.Id)).EnsureExists();

		var oldStatus = entity.Status;
		mapper.MapToEntity(request, entity);

		await _dataAccessLayer.SaveChanges();

		// Notify transporter about accept/reject
		if ((int)oldStatus != request.Status)
		{
			try
			{
				await _publisher.PublishFanout("livestocktrading.notification.push", new TransportOfferStatusChangedEvent
				{
					OfferId = entity.Id,
					TransporterUserId = entity.TransporterId,
					RequestOwnerUserId = request.RequestOwnerUserId,
					NewStatus = request.Status
				});
			}
			catch { }
		}

		var response = mapper.MapToResponse(entity);
		return ArfBlocksResults.Success(response);
	}
}
