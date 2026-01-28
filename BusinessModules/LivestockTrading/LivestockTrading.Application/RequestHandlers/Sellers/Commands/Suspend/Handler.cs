using LivestockTrading.Domain.Entities;
using LivestockTrading.Domain.Errors;
using Common.Services.ErrorCodeGenerator;

namespace LivestockTrading.Application.RequestHandlers.Sellers.Commands.Suspend;

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

		var seller = await _dataAccessLayer.GetSellerById(request.Id, cancellationToken);

		if (seller == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.SellerErrors.SellerNotFound));

		var suspendedAt = DateTime.UtcNow;

		// Update seller status to Suspended
		seller.Status = SellerStatus.Suspended;
		seller.IsActive = false;
		seller.UpdatedAt = suspendedAt;
		// Note: SuspensionReason could be stored in a separate field or audit log
		// For now we're just updating the status

		await _dataAccessLayer.SaveChanges(cancellationToken);

		var response = mapper.MapToResponse(seller, suspendedAt);
		return ArfBlocksResults.Success(response);
	}
}
