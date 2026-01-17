namespace BaseModules.FileProvider.Application.RequestHandlers.Files.Commands.Delete;

public class ResponseModel : IResponseModel
{
    public string BucketId { get; set; }
    public Guid FileId { get; set; }
    public Guid ChangeId { get; set; }
}

public class RequestModel : IRequestModel
{
    public string BucketId { get; set; }
    public Guid FileId { get; set; }
    public Guid? ChangeId { get; set; }
}