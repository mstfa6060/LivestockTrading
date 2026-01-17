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

		var message = GenerateLocalizedOtpMessage(otpCode, request.Language, request.CompanyId);


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
	private string GenerateLocalizedOtpMessage(string otpCode, string language, Guid companyId)
	{
		var isHirovo = companyId == Guid.Parse("C9D8C846-10FC-466D-8F45-A4FA4E856ABD"); // gerçek ID'yi kullan

		var hirovoMessages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["tr"] = $"[Hirovo] Giriş kodunuz: {otpCode}",
			["en"] = $"[Hirovo] Your login code: {otpCode}",
			["ar"] = $"[Hirovo] رمز الدخول الخاص بك هو: {otpCode}",
			["fr"] = $"[Hirovo] Votre code de connexion est : {otpCode}",
			["de"] = $"[Hirovo] Ihr Anmeldecode lautet: {otpCode}",
			["es"] = $"[Hirovo] Su código de acceso es: {otpCode}",
			["ru"] = $"[Hirovo] Ваш код входа: {otpCode}",
			["pt"] = $"[Hirovo] Seu código de login é: {otpCode}",
			["hi"] = $"[Hirovo] आपका लॉगिन कोड है: {otpCode}",
			["zh"] = $"[Hirovo] 您的登录验证码是：{otpCode}"
		};

		var defaultMessages = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["tr"] = $"Doğrulama kodunuz: {otpCode}",
			["en"] = $"Your verification code is: {otpCode}",
			["ar"] = $"رمز التحقق الخاص بك هو: {otpCode}",
			["fr"] = $"Votre code de vérification est : {otpCode}",
			["de"] = $"Ihr Bestätigungscode lautet: {otpCode}",
			["es"] = $"Su código de verificación es: {otpCode}",
			["ru"] = $"Ваш код подтверждения: {otpCode}",
			["pt"] = $"Seu código de verificação é: {otpCode}",
			["hi"] = $"आपका सत्यापन कोड है: {otpCode}",
			["zh"] = $"您的验证码是：{otpCode}"
		};

		if (isHirovo && hirovoMessages.TryGetValue(language, out var hirovoMessage))
			return hirovoMessage;

		return defaultMessages.TryGetValue(language, out var defaultMessage)
			? defaultMessage
			: defaultMessages["tr"];
	}


}
