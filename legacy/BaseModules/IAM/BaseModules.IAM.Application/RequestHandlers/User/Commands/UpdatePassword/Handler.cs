namespace BaseModules.IAM.Application.RequestHandlers.Users.Commands.UpdatePassword;

/// <summary>
/// Şifre Güncelleme
/// Bu endpoint, kullanıcının mevcut şifresini değiştirir.
/// Eski şifre doğrulaması yapılır.
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


	public async Task<ArfBlocksRequestResult> Handle(IRequestModel model, EndpointContext context, CancellationToken cancellationToken)
	{
		var mapper = new Mapper();
		var request = (RequestModel)model;
		var user = await _dataAccessLayer.GetUserById(request.UserId);

		if (user == null)
			throw new ArfBlocksValidationException(DomainErrors.UserErrors.UserNotFound);

		var isValid = SecurityHelper.VerifyPassword(request.OldPassword, user.PasswordHash, user.PasswordSalt);
		if (!isValid)
			throw new ArfBlocksValidationException("USER_PASSWORD_INVALID~Eski şifre hatalı.");

		var newSalt = SecurityHelper.GenerateSalt();
		var newHash = SecurityHelper.HashPassword(request.NewPassword, newSalt);
		user.PasswordHash = newHash;
		user.PasswordSalt = newSalt;

		await _dataAccessLayer.UpdateUser(user);

		var response = mapper.MapToResponse(user);

		return ArfBlocksResults.Success(response);
	}
}
