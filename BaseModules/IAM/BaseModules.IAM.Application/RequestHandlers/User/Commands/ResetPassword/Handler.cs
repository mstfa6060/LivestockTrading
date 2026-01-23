using BaseModules.IAM.Infrastructure.Security;

namespace BaseModules.IAM.Application.RequestHandlers.Users.Commands.ResetPassword;

/// <summary>
/// Şifre Sıfırlama
/// Bu endpoint, şifre sıfırlama işlemini tamamlar.
/// Yeni şifre kaydedilir.
/// </summary>
public class Handler : IRequestHandler
{
	private readonly DataAccess _dataAccess;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, DataAccess dataAccess)
	{
		_dataAccess = dataAccess;
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel model, EndpointContext context, CancellationToken cancellationToken)
	{
		var request = (RequestModel)model;
		var user = await _dataAccess.GetUserByToken(request.Token);

		if (user == null)
			throw new ArfBlocksValidationException(DomainErrors.UserErrors.UserNotFound);

		var salt = SecurityHelper.GenerateSalt();
		var hash = SecurityHelper.HashPassword(request.NewPassword, salt);

		user.PasswordHash = hash;
		user.PasswordSalt = salt;
		user.PasswordResetToken = null;
		user.PasswordResetTokenExpiry = null;

		await _dataAccess.UpdateUser(user);
		var response = new ResponseModel { Id = user.Id };
		return ArfBlocksResults.Success(response);
	}
}
