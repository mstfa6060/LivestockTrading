using Common.Definitions.Domain.Messages;

namespace BaseModules.IAM.Application.RequestHandlers.Auth.Commands.Logout;

public class RequestModel : IRequestModel
{
	// Veri alınmayacak, boş model
}

public class ResponseModel : IResponseModel
{
	public string MessageCode => AuthMessages.LogoutSucceeded;
}
