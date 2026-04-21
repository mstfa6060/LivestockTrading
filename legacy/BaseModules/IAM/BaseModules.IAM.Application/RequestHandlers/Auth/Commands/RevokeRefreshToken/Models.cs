namespace BaseModules.IAM.Application.RequestHandlers.Auth.Commands.RevokeRefreshToken;

public class RequestModel : IRequestModel
{
	public Guid RefreshTokenId { get; set; }
}

public class ResponseModel : IResponseModel
{
	public bool Success { get; set; }
}
