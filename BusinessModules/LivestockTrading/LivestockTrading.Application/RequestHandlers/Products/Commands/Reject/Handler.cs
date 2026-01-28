using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.Products.Commands.Reject;

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

		var product = await _dataAccessLayer.GetProductById(request.Id);

		if (product == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CommonErrors.IdNotValid));

		// Update product status to Rejected
		product.Status = ProductStatus.Rejected;
		product.UpdatedAt = DateTime.UtcNow;

		await _dataAccessLayer.SaveChanges();

		var response = mapper.MapToResponse(product, request.Reason);
		return ArfBlocksResults.Success(response);
	}
}
