namespace BaseModules.IAM.Workers.SmsSender.Services;

public class FakeSmsService : ISmsService
{
    public Task SendSmsAsync(string phoneNumber, string message)
    {
        Console.WriteLine($"📲 [FakeSms] {phoneNumber} → {message}");
        return Task.CompletedTask;
    }
}