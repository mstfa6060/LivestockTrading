namespace BaseModules.IAM.Application.RequestHandlers.Companies.Queries.Detail;

/// <summary>
/// Şirket Detayı
/// Bu endpoint, belirli bir şirketin detaylı bilgilerini getirir.
/// Şirket bilgileri, ayarları ve ilişkili veriler döner.
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

		var company = await _dataAccessLayer.GetCompanyById(request.Id);
		var response = mapper.MapToResponse(company);

		return ArfBlocksResults.Success(response);
	}
}
