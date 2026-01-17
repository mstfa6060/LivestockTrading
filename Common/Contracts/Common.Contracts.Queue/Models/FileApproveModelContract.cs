namespace Common.Contracts.Queue.Models;

public class FileApproveModelContract
{
    public string BucketId { get; set; }
    public Guid UserId { get; set; }
}