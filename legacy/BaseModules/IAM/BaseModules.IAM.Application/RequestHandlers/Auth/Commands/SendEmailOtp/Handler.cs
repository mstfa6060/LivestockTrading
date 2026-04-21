using Common.Services.Messaging;

namespace BaseModules.IAM.Application.RequestHandlers.Auth.Commands.SendEmailOtp;

/// <summary>
/// E-posta OTP Kodu Gonderme
/// Kullanicinin e-posta adresine 6 haneli dogrulama kodu gonderir.
/// </summary>
public class Handler : IRequestHandler
{
    private readonly DataAccess _dataAccess;
    private readonly IRabbitMqPublisher _publisher;

    public Handler(ArfBlocksDependencyProvider dp, object dataAccess)
    {
        _dataAccess = (DataAccess)dataAccess;
        _publisher = dp.GetInstance<IRabbitMqPublisher>();
    }

    public async Task<ArfBlocksRequestResult> Handle(IRequestModel model, EndpointContext ctx, CancellationToken ct)
    {
        var request = (RequestModel)model;

        var user = await _dataAccess.GetUserById(request.UserId)
                   ?? await _dataAccess.GetUserByEmail(request.Email);

        if (user == null)
            throw new ArfBlocksValidationException(
                ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.UserNotFound));

        var otpCode = new Random().Next(100000, 999999).ToString();

        user.LastOtpCode = otpCode;
        user.LastOtpSentAt = DateTime.UtcNow;
        user.EmailConfirmed = false;

        await _dataAccess.SaveChanges();

        var subject = GenerateSubject(request.Language);
        var body = GenerateBody(otpCode, user.FirstName ?? user.UserName, request.Language);

        await _publisher.PublishFanout("iam.notification.email", new
        {
            EventType = "EmailOtp",
            Email = user.Email,
            Subject = subject,
            Body = body
        });

        return ArfBlocksResults.Success(new ResponseModel { IsSuccess = true });
    }

    private static string GenerateSubject(string language)
    {
        return language?.ToLowerInvariant() switch
        {
            "en" => "[Livestock Trading] Email Verification Code",
            "ar" => "[Livestock Trading] رمز التحقق من البريد الإلكتروني",
            "de" => "[Livestock Trading] E-Mail-Bestätigungscode",
            "fr" => "[Livestock Trading] Code de vérification par e-mail",
            _ => "[Livestock Trading] E-posta Dogrulama Kodu"
        };
    }

    private static string GenerateBody(string otpCode, string displayName, string language)
    {
        return language?.ToLowerInvariant() switch
        {
            "en" => $"Hello {displayName},\n\nYour email verification code is: {otpCode}\n\nThis code is valid for 10 minutes.\n\nIf you did not request this, please ignore this email.",
            "ar" => $"مرحبا {displayName}،\n\nرمز التحقق من بريدك الإلكتروني هو: {otpCode}\n\nهذا الرمز صالح لمدة 10 دقائق.",
            "de" => $"Hallo {displayName},\n\nIhr E-Mail-Bestätigungscode lautet: {otpCode}\n\nDieser Code ist 10 Minuten gültig.",
            "fr" => $"Bonjour {displayName},\n\nVotre code de vérification est : {otpCode}\n\nCe code est valide pendant 10 minutes.",
            _ => $"Merhaba {displayName},\n\nE-posta dogrulama kodunuz: {otpCode}\n\nBu kod 10 dakika boyunca gecerlidir.\n\nEger bu istegi siz yapmadiyseniz, bu e-postayi dikkate almayiniz."
        };
    }
}
