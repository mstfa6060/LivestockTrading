namespace BaseModules.IAM.Application.RequestHandlers.Auth.Commands.RefreshToken;

public class RequestModel : IRequestModel
{
	public string RefreshToken { get; set; }
	public ClientPlatforms Platform { get; set; }
}

public class ResponseModel : IResponseModel
{
	public string Jwt { get; set; }
	public DateTime SessionExpirationDate { get; set; }
	public string RefreshToken { get; set; }

}

