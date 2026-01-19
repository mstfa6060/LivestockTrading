namespace LivestockTrading.Workers.SmsSender.Services;

public interface ISmsService
{
    Task<bool> SendAsync(string phoneNumber, string message);
}
