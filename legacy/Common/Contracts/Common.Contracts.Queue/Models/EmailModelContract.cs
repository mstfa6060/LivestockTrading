namespace Common.Contracts.Queue.Models;

public class EmailModelContract
{
    public string[] TargetEmails { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public bool IsContentHtml { get; set; }
}