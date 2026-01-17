namespace BaseModules.IAM.Application.RequestHandlers.Companies.Commands.Create;

/// <summary>
/// Şirket Oluşturma
/// Bu endpoint, sistemde yeni bir şirket kaydı oluşturur.
/// Şirket bilgileri ve yapılandırmaları kaydedilir.
/// </summary>
public class Handler : IRequestHandler
{
	private readonly DataAccess _dataAccessLayer;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, DataAccess dataAccess)
	{
		_dataAccessLayer = dataAccess;
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var request = (RequestModel)payload;
		var mapper = new Mapper();

		var company = mapper.MapToEntity(request);
		await _dataAccessLayer.AddCompany(company);

		var response = mapper.MapToResponse(company);
		return ArfBlocksResults.Success(response);
	}
}
