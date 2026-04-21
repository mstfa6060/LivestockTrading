namespace BaseModules.IAM.Domain.Events;

public class ForgotPasswordEvent
{
    public string Email { get; set; }
    public string DisplayName { get; set; }
    public string Token { get; set; }
}
