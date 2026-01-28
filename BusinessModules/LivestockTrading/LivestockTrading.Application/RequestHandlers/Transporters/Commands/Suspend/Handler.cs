using LivestockTrading.Domain.Entities;
using LivestockTrading.Domain.Errors;
using Common.Services.ErrorCodeGenerator;

namespace LivestockTrading.Application.RequestHandlers.Transporters.Commands.Suspend;

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

		var transporter = await _dataAccessLayer.GetTransporterById(request.TransporterId, cancellationToken);

		if (transporter == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.TransporterErrors.TransporterNotFound));

		var suspendedAt = DateTime.UtcNow;

		// Update transporter status to Suspended
		transporter.Status = TransporterStatus.Suspended;
		transporter.IsActive = false;
		transporter.UpdatedAt = suspendedAt;
		// Note: SuspensionReason could be stored in a separate field or audit log
		// For now we're just updating the status

		await _dataAccessLayer.SaveChanges(cancellationToken);

		var response = mapper.MapToResponse(transporter, suspendedAt);
		return ArfBlocksResults.Success(response);
	}
}
