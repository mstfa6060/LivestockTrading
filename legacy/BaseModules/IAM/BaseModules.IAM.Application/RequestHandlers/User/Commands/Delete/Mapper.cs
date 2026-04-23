namespace BaseModules.IAM.Application.RequestHandlers.Users.Commands.Delete;

public class Mapper
{
	public ResponseModel MapToResponse(bool success, string message)
	{
		return new ResponseModel
		{
			Success = success,
			Message = message
		};
	}
}
