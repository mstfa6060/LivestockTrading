using BaseModules.IAM.Application.Messaging;
using BaseModules.IAM.Domain.Events;
using Common.Contracts.Queue.Models;
using Common.Services.Messaging;


namespace BaseModules.IAM.Application.RequestHandlers.Auth.Commands.SendOtp;

/// <summary>
/// OTP Kodu Gönderme
/// Bu endpoint, kullanıcının telefon numarasına doğrulama kodu (OTP) gönderir.
/// Çoklu dil desteği ile SMS gönderimi yapar.
/// </summary>
public class Handler : IRequestHandler
{
	private readonly DataAccess _dataAccess;
	private readonly IJwtService _jwtService;
	private readonly IHttpContextAccessor _httpAccessor;
	private readonly IRabbitMqPublisher _publisher;

	public Handler(ArfBlocksDependencyProvider dp, object dataAccess)
	{
		_dataAccess = (DataAccess)dataAccess;
		_jwtService = dp.GetInstance<IJwtService>();
		_httpAccessor = dp.GetInstance<IHttpContextAccessor>();
		_publisher = dp.GetInstance<IRabbitMqPublisher>();
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel model, EndpointContext ctx, CancellationToken ct)
	{
		var request = (RequestModel)model;
		var mapper = new Mapper();
		var user = await _dataAccess.GetUserById(request.UserId);

		if (user != null)
		{
			user.PhoneNumber = request.PhoneNumber;
			await _dataAccess.UpdateUser(user);
		}

		var otpCode = new Random().Next(100000, 999999).ToString();

		var message = GenerateLocalizedOtpMessage(otpCode, request.Language);


		await _publisher.PublishFanout("iam.notification.sms", new SmsModelContract
		{
			TargetSms = new[] { user.PhoneNumber },
			Content = message
		});

		user.LastOtpCode = otpCode;
		user.LastOtpSentAt = DateTime.UtcNow;
		user.IsPhoneVerified = false;

		_dataAccess.UpdateUserOtp(user);

		await _dataAccess.SaveChanges();

		return ArfBlocksResults.Success(new ResponseModel
		{
			IsSuccess = true,
			OtpCode = otpCode
		});
	}
	private string GenerateLocalizedOtpMessage(string otpCode, string language)
	{
		var messages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["tr"] = $"[LivestockTrading] Giriş kodunuz: {otpCode}",
			["en"] = $"[LivestockTrading] Your login code: {otpCode}",
			["ar"] = $"[LivestockTrading] رمز الدخول الخاص بك هو: {otpCode}",
			["fr"] = $"[LivestockTrading] Votre code de connexion est : {otpCode}",
			["de"] = $"[LivestockTrading] Ihr Anmeldecode lautet: {otpCode}",
			["es"] = $"[LivestockTrading] Su código de acceso es: {otpCode}",
			["ru"] = $"[LivestockTrading] Ваш код входа: {otpCode}",
			["pt"] = $"[LivestockTrading] Seu código de login é: {otpCode}",
			["hi"] = $"[LivestockTrading] आपका लॉगिन कोड है: {otpCode}",
			["zh"] = $"[LivestockTrading] 您的登录验证码是：{otpCode}"
		};

		return messages.TryGetValue(language, out var message)
			? message
			: messages["tr"];
	}


}
