namespace BaseModules.IAM.Application.RequestHandlers.Users.Commands.UpdatePassword;


public class Mapper
{
	public ResponseModel MapToResponse(Common.Definitions.Domain.Entities.User user)
	{
		return new ResponseModel()
		{
			Id = user.Id
		};
	}
}
