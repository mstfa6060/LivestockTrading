using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace LivestockTrading.Workers.SmsSender.Services;

public class TwilioSmsService : ISmsService
{
    private readonly string _accountSid;
    private readonly string _authToken;
    private readonly string _fromNumber;
    private readonly ILogger<TwilioSmsService> _logger;
    private readonly bool _isConfigured;

    public TwilioSmsService(IConfiguration configuration, ILogger<TwilioSmsService> logger)
    {
        _logger = logger;

        _accountSid = configuration["Twilio:AccountSid"] ?? Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID") ?? "";
        _authToken = configuration["Twilio:AuthToken"] ?? Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN") ?? "";
        _fromNumber = configuration["Twilio:FromNumber"] ?? Environment.GetEnvironmentVariable("TWILIO_FROM_PHONE") ?? "";

        _isConfigured = !string.IsNullOrEmpty(_accountSid) &&
                       !string.IsNullOrEmpty(_authToken) &&
                       !string.IsNullOrEmpty(_fromNumber);

        if (_isConfigured)
        {
            TwilioClient.Init(_accountSid, _authToken);
            _logger.LogInformation("Twilio SMS service initialized successfully");
        }
        else
        {
            _logger.LogWarning("Twilio SMS service not configured. SMS will be logged but not sent.");
        }
    }

    public async Task<bool> SendAsync(string phoneNumber, string message)
    {
        if (string.IsNullOrEmpty(phoneNumber))
        {
            _logger.LogWarning("Phone number is empty. SMS not sent.");
            return false;
        }

        if (!_isConfigured)
        {
            _logger.LogWarning("Twilio not configured. SMS not sent. To: {PhoneNumber}, Message: {Message}", phoneNumber, message);
            return false;
        }

        try
        {
            var messageResource = await MessageResource.CreateAsync(
                body: message,
                from: new PhoneNumber(_fromNumber),
                to: new PhoneNumber(phoneNumber)
            );

            _logger.LogInformation("SMS sent successfully. SID: {Sid}, To: {To}, Status: {Status}",
                messageResource.Sid, phoneNumber, messageResource.Status);

            return messageResource.Status == MessageResource.StatusEnum.Queued ||
                   messageResource.Status == MessageResource.StatusEnum.Sent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending SMS to {PhoneNumber}", phoneNumber);
            return false;
        }
    }
}
