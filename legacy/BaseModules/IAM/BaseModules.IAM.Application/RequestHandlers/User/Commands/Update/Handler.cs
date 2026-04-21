namespace BaseModules.IAM.Application.RequestHandlers.Users.Commands.Update;

/// <summary>
/// Kullanıcı Profil Güncelleme
/// Bu endpoint, mevcut kullanıcının profil bilgilerini günceller.
/// Güncellenebilir alanlar: firstName, surname, phoneNumber, countryId, language, preferredCurrencyCode, avatarUrl
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
		var mapper = new Mapper();
		var request = (RequestModel)payload;

		var user = await _dataAccessLayer.GetById(request.Id);
		if (user == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.UserNotFound));

		mapper.MapToEntity(request, user);

		await _dataAccessLayer.SaveChanges();

		// Country bilgisi ile birlikte tekrar çek
		var userWithCountry = await _dataAccessLayer.GetByIdWithCountry(user.Id);

		var response = mapper.MapToResponse(userWithCountry);
		return ArfBlocksResults.Success(response);
	}
}
