namespace BaseModules.IAM.Application.RequestHandlers.Companies.Queries.Pick;

/// <summary>
/// Şirket Seçim Listesi
/// Bu endpoint, dropdown veya seçim listesi için şirket listesi döner.
/// Sadece ID ve isim bilgileri içerir.
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

		var companies = await _dataAccessLayer.GetFiltered(request);
		var response = mapper.MapToResponse(companies);

		return ArfBlocksResults.Success(response);
	}
}
