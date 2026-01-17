namespace BaseModules.IAM.Application.RequestHandlers.Companies.Commands.Update;

/// <summary>
/// Şirket Güncelleme
/// Bu endpoint, mevcut bir şirketin bilgilerini günceller.
/// Şirket detayları ve yapılandırmaları değiştirilebilir.
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

		company.Name = request.Name;
		company.TaxNumber = request.TaxNumber;
		company.Address = request.Address;
		company.Phone = request.Phone;

		await _dataAccessLayer.UpdateCompany(company);

		var response = mapper.MapToResponse(company);
		return ArfBlocksResults.Success(response);
	}
}
