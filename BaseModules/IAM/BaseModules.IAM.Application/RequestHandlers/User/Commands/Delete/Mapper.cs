namespace BaseModules.IAM.Application.RequestHandlers.User.Commands.Delete;

public class Mapper
{
	public ResponseModel MapToResponse(Common.Definitions.Domain.Entities.User user)
	{
		return new ResponseModel
		{
			Id = user.Id,
			IsDeleted = user.IsDeleted
		};
	}
}