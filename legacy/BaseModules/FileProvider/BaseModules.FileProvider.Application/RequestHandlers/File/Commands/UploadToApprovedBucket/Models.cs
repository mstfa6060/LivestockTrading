namespace BaseModules.FileProvider.Application.RequestHandlers.Files.Commands.UploadToApprovedBucket;

public class ResponseModel : IResponseModel
{
	public Guid ChangeId { get; set; }
	public string BucketId { get; set; }
	public BucketTypes BucketType { get; set; }
	public string ModuleName { get; set; }
	public List<FileResponse> Files { get; set; }

	public class FileResponse
	{
		public Guid Id { get; set; }
		public string Extention { get; set; }
		public string Name { get; set; }
		public string Path { get; set; }
		public string ContentType { get; set; }
		public bool IsDefault { get; set; }
		public int Index { get; set; }

		// Resim varyantlari
		public List<ImageVariant> Variants { get; set; }
		public int? Width { get; set; }
		public int? Height { get; set; }
		public long? SizeBytes { get; set; }
	}

	public class ImageVariant
	{
		public string Key { get; set; }
		public string Url { get; set; }
	}
}

public class RequestModel : IRequestModel
{
	public IFormFile FormFile { get; set; }
	public string ModuleName { get; set; }
	public string BucketId { get; set; }
	public BucketTypes BucketType { get; set; }
	public Guid? ChangeId { get; set; }
	public Guid? EntityId { get; set; }
	public string FolderName { get; set; }
	public string VersionName { get; set; }
}