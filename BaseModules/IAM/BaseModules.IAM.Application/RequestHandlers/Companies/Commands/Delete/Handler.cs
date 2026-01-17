namespace BaseModules.IAM.Application.RequestHandlers.Companies.Commands.Delete;

/// <summary>
/// Şirket Silme
/// Bu endpoint, sistemden bir şirketi siler.
/// İlişkili tüm veriler temizlenir.
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
		company.IsDeleted = request.IsDeleted;
		await _dataAccessLayer.DeleteCompany(company);

		var response = mapper.MapToResponse(company);
		return ArfBlocksResults.Success(response);
	}
}
