namespace BaseModules.Notification.Application.RequestHandlers.Push.Commands.RegisterToken;

public class Mapper
{
	public ResponseModel MapToResponse(bool result)
	{
		return new ResponseModel { Success = result };
	}
}
