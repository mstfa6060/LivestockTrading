namespace BaseModules.FileProvider.Application.EventHandlers.UserImages.Commands.CreateOrReplace;

public class ResponseModel : FileProperties, IResponseModel
{ }

public class RequestModel : IRequestModel
{
	public Guid CompanyId { get; set; }
	public string Name { get; set; }
	public string Extention { get; set; }
	public byte[] Data { get; set; }
}
