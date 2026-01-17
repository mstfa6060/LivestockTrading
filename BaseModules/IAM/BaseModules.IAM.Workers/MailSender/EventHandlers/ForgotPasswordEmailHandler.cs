using BaseModules.IAM.Domain.Events;
using Arfware.ArfBlocks.Core;
using System.Web;

namespace BaseModules.IAM.Workers.MailSender.EventHandlers;

public class ForgotPasswordEmailHandler
{
    private readonly IEmailService _emailService;
    private readonly ArfBlocksRequestOperator _requestOperator;
    private readonly ILogger<ForgotPasswordEmailHandler> _logger;

    public ForgotPasswordEmailHandler(IEmailService emailService, ArfBlocksDependencyProvider dependencyProvider, ILogger<ForgotPasswordEmailHandler> logger)
    {
        _emailService = emailService;
        _requestOperator = new ArfBlocksRequestOperator(dependencyProvider);
        _logger = logger;
    }

    public async Task HandleAsync(ForgotPasswordEvent @event)
    {
        var request = new Application.EventHandlers.Mails.Queries.SendForgotPassword.RequestModel
        {
            EventName = "SendForgotPassword",
            ModuleName = "IAM",
            ObjectId = Guid.NewGuid(), // email ID gibi benzersiz istek ID’si
            PublishedAt = DateTime.UtcNow,
            UserId = Guid.Empty,
            Version = "1.0",
            IsInternalCall = true,
            Email = @event.Email
        };
        var responseResult = await _requestOperator.OperateEvent<Application.EventHandlers.Mails.Queries.SendForgotPassword.Handler>(request);

        if (responseResult?.Payload is not Application.EventHandlers.Mails.Queries.SendForgotPassword.ResponseModel response)
        {
            _logger.LogWarning("[ForgotPasswordWorker] ❌ Response null döndü.");
            return;
        }

        foreach (var user in response.Users)
        {
            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                var subject = "🔐 Şifre Sıfırlama Bağlantısı";
                var encodedToken = HttpUtility.UrlEncode(user.Token);
                var link = $"https://mstfa6060.github.io/hirovo-link?token={encodedToken}";
                var body = $"""
Merhaba {user.DisplayName},

Şifrenizi sıfırlamak için aşağıdaki bağlantıya tıklayın:

🔗 {link}

Bu bağlantı 15 dakika boyunca geçerlidir.
""";

                _logger.LogInformation($"[ForgotPasswordWorker] 📧 E-posta gönderiliyor: {user.Email}");
                await _emailService.SendEmailAsync(user.Email, subject, body);
            }
        }
    }
}
