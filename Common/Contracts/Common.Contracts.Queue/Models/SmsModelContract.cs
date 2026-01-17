namespace Common.Contracts.Queue.Models;

public class SmsModelContract
{
    public string[] TargetSms { get; set; }
    public string Content { get; set; }
}