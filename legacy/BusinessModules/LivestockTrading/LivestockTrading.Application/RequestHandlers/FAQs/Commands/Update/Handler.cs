namespace LivestockTrading.Application.RequestHandlers.FAQs.Commands.Update;

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

		var faq = await _dataAccessLayer.GetFAQById(request.Id);

		if (faq == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CommonErrors.IdNotValid));

		mapper.MapToEntity(request, faq);

		await _dataAccessLayer.SaveChanges();

		var response = mapper.MapToResponse(faq);
		return ArfBlocksResults.Success(response);
	}
}
