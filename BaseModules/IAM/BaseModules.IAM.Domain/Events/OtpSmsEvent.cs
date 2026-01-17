namespace BaseModules.IAM.Domain.Events;

public class OtpSmsEvent
{
    public string PhoneNumber { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; }
}
