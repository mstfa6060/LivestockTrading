namespace BaseModules.FileProvider.Application.RequestHandlers.Buckets.Commands.Duplicate;

public class ResponseModel : IResponseModel
{
	public Guid ChangeId { get; set; }
	public string BucketId { get; set; }
	public BucketTypes BucketType { get; set; }

}

public class RequestModel : IRequestModel
{
	public string BucketId { get; set; }
	public Guid ChangeId { get; set; }
	public Guid? EntityId { get; set; }
	public string FolderName { get; set; }
	public string VersionName { get; set; }
}

