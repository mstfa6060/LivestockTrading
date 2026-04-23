namespace BaseModules.Notification.Application.RequestHandlers.Push.Commands.RevokeToken;

public class Mapper
{
	public ResponseModel MapToResponse(bool result)
	{
		return new ResponseModel { Success = result };
	}
}
