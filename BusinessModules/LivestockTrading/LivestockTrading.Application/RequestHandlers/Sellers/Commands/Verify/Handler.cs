using LivestockTrading.Domain.Entities;
using LivestockTrading.Domain.Errors;
using Common.Services.ErrorCodeGenerator;

namespace LivestockTrading.Application.RequestHandlers.Sellers.Commands.Verify;

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

		// Update seller status to Active and mark as verified
		seller.Status = SellerStatus.Active;
		seller.IsVerified = true;
		seller.VerifiedAt = DateTime.UtcNow;
		seller.IsActive = true;
		seller.UpdatedAt = DateTime.UtcNow;

		await _dataAccessLayer.SaveChanges(cancellationToken);

		var response = mapper.MapToResponse(seller);
		return ArfBlocksResults.Success(response);
	}
}
