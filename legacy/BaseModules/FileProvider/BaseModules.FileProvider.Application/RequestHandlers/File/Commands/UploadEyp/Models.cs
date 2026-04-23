namespace BaseModules.FileProvider.Application.RequestHandlers.Files.Commands.UploadEyp;

public class ResponseModel : IResponseModel
{
	public Guid EntityId { get; set; }
	public string Extention { get; set; }
	public string Name { get; set; }
	public string Path { get; set; }
	public string ContentType { get; set; }
}

public class RequestModel : IRequestModel
{
	public IFormFile FormFile { get; set; }
	public Guid EntityId { get; set; }
	public Guid CompanyId { get; set; }
	public string FileExtention { get; set; }
}