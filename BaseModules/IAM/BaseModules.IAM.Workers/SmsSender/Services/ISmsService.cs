namespace BaseModules.IAM.Workers.SmsSender.Services;

public interface ISmsService
{
    Task SendSmsAsync(string phoneNumber, string message);
}