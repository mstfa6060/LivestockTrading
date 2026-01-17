using System.Security.Cryptography;
using BaseModules.IAM.Application.Messaging;
using BaseModules.IAM.Domain.Events;

namespace BaseModules.IAM.Application.RequestHandlers.Users.Commands.ForgotPassword;

/// <summary>
/// Şifre Sıfırlama Talebi
/// Bu endpoint, kullanıcının şifresini sıfırlama talebini başlatır.
/// E-posta ile sıfırlama linki gönderilir.
/// </summary>
public class Handler : IRequestHandler
{
	private readonly DataAccess _dataAccess;
	private readonly ForgotPasswordEmailPublisher _emailPublisher;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
	{
		_dataAccess = (DataAccess)dataAccess;
		_emailPublisher = dependencyProvider.GetInstance<ForgotPasswordEmailPublisher>();
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel model, EndpointContext context, CancellationToken cancellationToken)
	{
		var request = (RequestModel)model;
		var mapper = new Mapper();

		var user = await _dataAccess.GetUserByEmail(request.Email, request.CompanyId);
		if (user == null)
			throw new ArfBlocksValidationException(DomainErrors.UserErrors.UserNotFound);

		// Token üret
		var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)); // daha güçlü
		user.PasswordResetToken = token;
		user.PasswordResetTokenExpiry = DateTime.UtcNow.AddMinutes(15);
		await _dataAccess.UpdateUser(user);

		// Event fırlat
		var evt = new ForgotPasswordEvent
		{
			Email = user.Email,
			DisplayName = $"{user.FirstName} {user.Surname}",
			Token = token
		};
		await _emailPublisher.PublishAsync(evt);

		//  Cevap modeli
		var response = mapper.MapToResponse(user);
		return ArfBlocksResults.Success(response);
	}
}
