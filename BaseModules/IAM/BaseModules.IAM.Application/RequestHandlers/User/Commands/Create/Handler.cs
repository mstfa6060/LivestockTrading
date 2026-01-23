namespace BaseModules.IAM.Application.RequestHandlers.Users.Commands.Create;

/// <summary>
/// Kullanıcı Oluşturma
/// Bu endpoint, sistemde yeni bir kullanıcı hesabı oluşturur.
/// Kullanıcı bilgileri ve kimlik doğrulama ayarları kaydedilir.
/// </summary>
public class Handler : IRequestHandler
{
	private readonly DataAccess _dataAccessLayer;
	private readonly ArfBlocksCommunicator _communicator;
	private readonly EnvironmentService _environmentService;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, DataAccess dataAccess) //  `object` yerine `DataAccess` kullanıldı
	{
		_dataAccessLayer = dataAccess;
		_communicator = dependencyProvider.GetInstance<ArfBlocksCommunicator>();
		_environmentService = dependencyProvider.GetInstance<EnvironmentService>();
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var mapper = new Mapper();
		var requestPayload = (RequestModel)payload;

		string hashedPassword = null;
		string salt = null;

		if (requestPayload.UserSource == UserSources.Manual)
		{
			// Şifre hashlemesi yap
			salt = SecurityHelper.GenerateSalt();
			hashedPassword = SecurityHelper.HashPassword(requestPayload.Password, salt);
		}

		// Kullanıcı nesnesini oluştur
		var user = mapper.MapToNewEntity(requestPayload, hashedPassword, salt);

		// Veritabanına ekle
		await _dataAccessLayer.AddUser(user);

		// Country bilgisi ile birlikte tekrar çek
		var userWithCountry = await _dataAccessLayer.GetUserWithCountry(user.Id);

		var response = mapper.MapToResponse(userWithCountry);
		return ArfBlocksResults.Success(response);
	}
}
