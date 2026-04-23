using Common.Services.ErrorCodeGenerator;
using LivestockTrading.Domain.Errors;

namespace LivestockTrading.Application.RequestHandlers.Products.Queries.MediaDetail;

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

		var response = await _dataAccessLayer.GetMediaAsync(request.ProductId, cancellationToken);

		if (response == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CommonErrors.IdNotValid));

		return ArfBlocksResults.Success(response);
	}
}
