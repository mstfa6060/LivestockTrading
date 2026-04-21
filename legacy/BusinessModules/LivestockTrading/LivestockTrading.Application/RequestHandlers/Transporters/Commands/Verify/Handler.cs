using LivestockTrading.Domain.Entities;
using LivestockTrading.Domain.Errors;
using Common.Services.ErrorCodeGenerator;

namespace LivestockTrading.Application.RequestHandlers.Transporters.Commands.Verify;

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

		var transporter = await _dataAccessLayer.GetTransporterById(request.Id, cancellationToken);

		if (transporter == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.TransporterErrors.TransporterNotFound));

		// Update transporter status to Active and mark as verified
		transporter.Status = TransporterStatus.Active;
		transporter.IsVerified = true;
		transporter.VerifiedAt = DateTime.UtcNow;
		transporter.IsActive = true;
		transporter.UpdatedAt = DateTime.UtcNow;

		await _dataAccessLayer.SaveChanges(cancellationToken);

		var response = mapper.MapToResponse(transporter);
		return ArfBlocksResults.Success(response);
	}
}
