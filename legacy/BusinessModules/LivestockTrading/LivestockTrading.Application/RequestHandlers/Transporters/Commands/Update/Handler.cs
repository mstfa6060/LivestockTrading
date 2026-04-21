namespace LivestockTrading.Application.RequestHandlers.Transporters.Commands.Update;

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

		var transporter = await _dataAccessLayer.GetTransporterById(request.Id);

		if (transporter == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CommonErrors.IdNotValid));

		mapper.MapToEntity(request, transporter);

		await _dataAccessLayer.SaveChanges();

		var response = mapper.MapToResponse(transporter);
		return ArfBlocksResults.Success(response);
	}
}
