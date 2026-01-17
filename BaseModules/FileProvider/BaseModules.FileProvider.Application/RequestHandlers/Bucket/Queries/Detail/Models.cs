namespace BaseModules.FileProvider.Application.RequestHandlers.Buckets.Queries.Detail;

public class ResponseModel : IResponseModel
{
    public string BucketId { get; set; }
    public Guid ChangeId { get; set; }
    public List<FileResponse> Files { get; set; }

    public class FileResponse
    {
        public Guid Id { get; set; }
        public string Extention { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string SecurePath { get; set; }
        public string ContentType { get; set; }
        public bool IsDefault { get; set; }
        public int Index { get; set; }
    }
}

public class RequestModel : IRequestModel
{
    public string BucketId { get; set; }
    public Guid ChangeId { get; set; }
}