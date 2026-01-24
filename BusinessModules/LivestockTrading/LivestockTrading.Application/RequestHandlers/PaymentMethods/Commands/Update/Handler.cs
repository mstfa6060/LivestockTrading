namespace LivestockTrading.Application.RequestHandlers.PaymentMethods.Commands.Update;

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

		var paymentMethod = await _dataAccessLayer.GetPaymentMethodById(request.Id);

		if (paymentMethod == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CommonErrors.IdNotValid));

		mapper.MapToEntity(request, paymentMethod);

		await _dataAccessLayer.SaveChanges();

		var response = mapper.MapToResponse(paymentMethod);
		return ArfBlocksResults.Success(response);
	}
}
