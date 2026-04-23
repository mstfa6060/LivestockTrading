
using BaseModules.IAM.Workers.SmsSender.Services;
using Common.Contracts.Queue.Models;

namespace BaseModules.IAM.Workers.SmsSender.EventHandlers;

public class OtpSmsHandler
{
    private readonly ISmsService _smsService;

    public OtpSmsHandler(ISmsService smsService)
    {
        _smsService = smsService;
    }

    public async Task HandleAsync(SmsModelContract sms)
    {
        if (sms.TargetSms == null || sms.TargetSms.Length == 0)
            return;

        foreach (var phone in sms.TargetSms)
        {
            await _smsService.SendSmsAsync(phone, sms.Content);
        }
    }
}
